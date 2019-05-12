using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;

namespace CLIF.Switch
{
    public class SwitchParser
    {
        public static string[] AppArgs { get; set; } = Environment.GetCommandLineArgs()
                                                                  .Skip(1)
                                                                  .ToArray();

        public static string ArgFilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{AppDomain.CurrentDomain.FriendlyName}.args");

        public static T Parse<T>() where T : ISwitchSet { return Parse<T>(null, AppArgs); }

        public static void CheckCollisions(Type baseType, Action<string, string> OnCollision)
        {
            var type = typeof(ISwitchSet);
            var switchSets = Assembly.GetExecutingAssembly()
                                     .GetTypes()
                                     .Where(x => !x.IsInterface && type.IsAssignableFrom(x) && !baseType.IsAssignableFrom(x));

            var baseSwitches = new Dictionary<string, Type>();
            {
                var fields = baseType.GetProperties();
                foreach (var field in fields)
                {
                    var switchAttr = field.GetCustomAttribute<SwitchAttribute>(true);
                    if (switchAttr == null || switchAttr.AllPositionals) continue;

                    if (switchAttr.Positional > -1 && string.IsNullOrWhiteSpace(switchAttr.Switch)) continue;

                    baseSwitches.Add(switchAttr.Switch, field.DeclaringType);
                    var aliasAttrs = field.GetCustomAttributes<SwitchAliasAttribute>(true)
                                          .ToArray();
                    foreach (var aliasAttr in aliasAttrs) baseSwitches.Add(aliasAttr.Alias, field.DeclaringType);
                }
            }

            foreach (var switchSet in switchSets)
            {
                var currentSwitches = new HashSet<string>();
                var fields          = switchSet.GetProperties();
                foreach (var field in fields)
                {
                    var switchAttr = field.GetCustomAttribute<SwitchAttribute>(true);
                    if (switchAttr == null || switchAttr.AllPositionals) continue;

                    if (switchAttr.Positional > -1 && string.IsNullOrWhiteSpace(switchAttr.Switch)) continue;

                    if (baseSwitches.ContainsKey(switchAttr.Switch) && field.DeclaringType != baseSwitches[switchAttr.Switch] || !currentSwitches.Add(switchAttr.Switch)) OnCollision(switchAttr.Switch, switchSet.FullName + "::" + field.Name);

                    var aliasAttrs = field.GetCustomAttributes<SwitchAliasAttribute>(true)
                                          .ToArray();
                    foreach (var aliasAttr in aliasAttrs)
                        if (baseSwitches.ContainsKey(aliasAttr.Alias) && field.DeclaringType != baseSwitches[aliasAttr.Alias] || !currentSwitches.Add(aliasAttr.Alias))
                            OnCollision(aliasAttr.Alias, switchSet.FullName + "::" + field.Name);
                }
            }
        }

        public static void Help<T>(Dictionary<string, string> values) where T : ISwitchSet
        {
            var interfaceType = typeof(T);

            var positionalValues = new List<string>();
            var singulars        = new List<string>();
            var help             = new List<string>();
            var fields           = interfaceType.GetProperties();
            foreach (var field in fields)
            {
                var switchAttribute = field.GetCustomAttribute<SwitchAttribute>(true);
                if (switchAttribute == null || switchAttribute.AllPositionals) continue;

                if (switchAttribute.Positional > -1)
                {
                    var x                            = switchAttribute.Switch;
                    if (!switchAttribute.Required) x = $"[{x}]";

                    if (!values.TryGetValue(x, out var v)) v = x;
                    positionalValues.Add(v);
                }

                var localSwitches = new List<string> { switchAttribute.Switch };
                localSwitches.AddRange(field.GetCustomAttributes<SwitchAliasAttribute>()
                                            .Select(x => x.Alias));
                foreach (var localSwitch in localSwitches)
                {
                    if (string.IsNullOrWhiteSpace(localSwitch)) continue;
                    if (localSwitch.Length == 1)
                    {
                        singulars.Add(localSwitch);
                    }
                    else
                    {
                        var x                                    = localSwitch;
                        if (!values.TryGetValue(x, out var v)) v = "value";
                        var t                                    = field.PropertyType;
                        if (t.IsGenericType &&
                            t.GetGenericTypeDefinition()
                             .IsAssignableFrom(typeof(Dictionary<,>)))
                            x = $"[--{x} key=value]";
                        else if (t.IsGenericType &&
                                 t.GetGenericTypeDefinition()
                                  .IsAssignableFrom(typeof(List<>)))
                            x = $"[--{x} value]";
                        else if (switchAttribute.Default != null && switchAttribute.NeedsValue)
                            x = $"[--{x}[={switchAttribute.Default}]]";
                        else if (switchAttribute.NeedsValue)
                            x = $"[--{x}={v}]";
                        else
                            x = $"[--{x}]";

                        help.Add(x);
                    }
                }
            }

            var helpString                             = AppDomain.CurrentDomain.FriendlyName;
            if (singulars.Count        > 0) helpString += $" [-{string.Join("", singulars)}]";
            if (help.Count             > 0) helpString += $" {string.Join(" ",  help)}";
            if (positionalValues.Count > 0) helpString += $" {string.Join(" ",  positionalValues)}";
            Console.Out.WriteLine(helpString);
        }

        public static void FullHelp<T>(Action<bool> extraHelp, bool skipOpener = false) where T : ISwitchSet
        {
            var interfaceType = typeof(T);

            var positionalValues     = new SortedList<int, string>();
            var positionalTextLength = "positional".Length;
            var positionalStrings    = new SortedList<int, string>();
            var helpSwitches         = new List<string>();
            var switchesTextLength   = "switches".Length;
            var helpStrings          = new List<string>();
            var helpTextLength       = "help".Length;
            var helpDefaults         = new List<string>();

            var fields = interfaceType.GetProperties();

            foreach (var field in fields)
            {
                var switchAttribute = field.GetCustomAttribute<SwitchAttribute>(true);
                if (switchAttribute == null || switchAttribute.AllPositionals) continue;
                var aliasAttributes = field.GetCustomAttributes<SwitchAliasAttribute>(true)
                                           .ToArray();

                var required = "";
                if (switchAttribute.Positional == -1)
                {
                    if (switchAttribute.Required) required = " (Required)";

                    if (switchAttribute.NeedsValue) required += " (Needs Value)";
                }

                var aliases = new List<string>();

                var prefix                                      = "--";
                if (switchAttribute.Switch?.Length == 1) prefix = "-";
                if (switchAttribute.Positional     > -1) prefix = "";

                aliases.Add(prefix + switchAttribute.Switch);
                foreach (var aliasAttribute in aliasAttributes)
                {
                    prefix = "--";
                    if (aliasAttribute.Alias.Length == 1) prefix = "-";
                    if (switchAttribute.Positional  > -1) prefix = "";
                    aliases.Add(prefix + aliasAttribute.Alias);
                }

                var helpSwitch = string.Join(" ", aliases);
                switchesTextLength = Math.Max(switchesTextLength, helpSwitch.Length);
                var helpString = $"{switchAttribute.Help}{required}";
                helpTextLength = Math.Max(helpTextLength, helpString.Length);
                var helpDefault                                  = "";
                if (switchAttribute.Default != null) helpDefault = switchAttribute.Default.ToString();

                if (switchAttribute.Positional > -1)
                {
                    positionalValues.Add(switchAttribute.Positional, helpSwitch);
                    positionalTextLength = Math.Max(positionalTextLength, helpSwitch.Length);
                    positionalStrings.Add(switchAttribute.Positional, helpString);
                }
                else
                {
                    helpStrings.Add(helpString);
                    helpSwitches.Add(helpSwitch);
                    helpDefaults.Add(helpDefault);
                }
            }

            if (!skipOpener) Help<T>(new Dictionary<string, string>());
            Console.Out.WriteLine();
            if (helpStrings.Count > 0)
            {
                if (!skipOpener) Console.Out.WriteLine("Switches:");
                Console.Out.WriteLine($"  {{0, -{switchesTextLength}}} | {{1, -{helpTextLength}}} | {{2}}", "switch", "help", "default");
                Console.Out.WriteLine("".PadLeft(switchesTextLength + helpTextLength + 20, '-'));
                for (var i = 0; i < helpStrings.Count; ++i)
                {
                    var helpSwitch  = helpSwitches[i];
                    var helpString  = helpStrings[i];
                    var helpDefault = helpDefaults[i];
                    Console.Out.WriteLine($"  {{0, -{switchesTextLength}}} | {{1, -{helpTextLength}}} | {{2}}", helpSwitch, helpString, helpDefault);
                }
            }

            var max = Math.Max("index".Length, (int) Math.Floor(positionalValues.Count / 10d) + 1);
            if (positionalValues.Count > 0)
            {
                Console.Out.WriteLine();
                if (!skipOpener) Console.Out.WriteLine("Positionals:");
                Console.Out.WriteLine($"  {{0, -{max}}} | --{{1, -{positionalTextLength}}} | {{2}}", "index", "positional", "help");
                Console.Out.WriteLine("".PadLeft(max + positionalTextLength + 30, '-'));
                foreach (var (key, positional) in positionalValues)
                {
                    var positionalString = positionalStrings[key];
                    Console.Out.WriteLine($"  {{0, -{max}}} | --{{1, -{positionalTextLength}}} | {{2}}", key, positional, positionalString);
                }
            }

            extraHelp?.Invoke(false);
        }

        public static T Parse<T>(Action<bool> extraHelp) where T : ISwitchSet { return Parse<T>(extraHelp, AppArgs); }

        public static T Parse<T>(Action<bool> extraHelp, string[] args) where T : ISwitchSet
        {
            if (args.Length == 0)
            {
                FullHelp<T>(extraHelp);
                return null;
            }

            var interfaceType = typeof(T);

            if (!(Activator.CreateInstance(interfaceType) is T instance)) return null;

            var presence         = new HashSet<string>();
            var positionals      = new List<string>();
            var values           = new Dictionary<string, string>();
            var dictionaryValues = new Dictionary<string, Dictionary<string, string>>();
            var arrayValues      = new Dictionary<string, List<string>>();

            var fields = interfaceType.GetProperties();
            foreach (var field in fields)
            {
                var switchAttribute = field.GetCustomAttribute<SwitchAttribute>(true);
                if (switchAttribute == null || switchAttribute.AllPositionals) continue;
                var t = field.PropertyType;
                if (t.IsGenericType &&
                    t.GetGenericTypeDefinition()
                     .IsAssignableFrom(typeof(Dictionary<,>)))
                {
                    var aliasAttributes = field.GetCustomAttributes<SwitchAliasAttribute>(true)
                                               .ToArray();
                    if (aliasAttributes.Length > 0)
                        foreach (var aliasAttribute in aliasAttributes)
                            dictionaryValues.Add(aliasAttribute.Alias, new Dictionary<string, string>());
                    dictionaryValues.Add(switchAttribute.Switch, new Dictionary<string, string>());
                }

                if (!t.IsGenericType ||
                    !t.GetGenericTypeDefinition()
                      .IsAssignableFrom(typeof(List<>))) continue;
                {
                    var aliasAttributes = field.GetCustomAttributes<SwitchAliasAttribute>(true)
                                               .ToArray();
                    if (aliasAttributes.Length > 0)
                        foreach (var aliasAttribute in aliasAttributes)
                            arrayValues.Add(aliasAttribute.Alias, new List<string>());
                    arrayValues.Add(switchAttribute.Switch, new List<string>());
                }
            }

            for (var i = 0; i < args.Length; ++i)
            {
                var arg = args[i];
                if (arg[0] == '-')
                {
                    if (arg[1] == '-')
                    {
                        var name  = arg.Substring(2);
                        var value = "true";
                        if (name.Contains('='))
                        {
                            value = name.Substring(name.IndexOf('=') + 1);
                            name  = name.Substring(0, name.IndexOf('='));
                        }

                        presence.Add(name);
                        if (dictionaryValues.ContainsKey(name))
                        {
                            var key = args[++i];
                            if (key.Contains('='))
                            {
                                value = key.Substring(key.IndexOf('=') + 1);
                                key   = key.Substring(0, key.IndexOf('='));
                            }

                            dictionaryValues[name][key] = value;
                        }
                        else if (arrayValues.ContainsKey(name))
                        {
                            arrayValues[name]
                                .Add(args[++i]);
                        }
                        else
                        {
                            values[name] = value;
                        }
                    }
                    else
                    {
                        var letters = arg.Substring(1)
                                         .ToArray();
                        foreach (var letter in letters)
                        {
                            if (letter == '=') break;
                            if (letter == '?' || letter == 'h')
                            {
                                Help<T>(new Dictionary<string, string>());
                                extraHelp?.Invoke(true);
                                return null;
                            }

                            presence.Add(letter.ToString());

                            var value                    = "true";
                            if (arg.Contains('=')) value = arg.Substring(arg.IndexOf('=') + 1);
                            var name                     = letter.ToString();
                            if (dictionaryValues.ContainsKey(name))
                            {
                                var key = args[++i];
                                if (key.Contains('='))
                                {
                                    value = key.Substring(key.IndexOf('=') + 1);
                                    key   = key.Substring(0, key.IndexOf('='));
                                }

                                dictionaryValues[name][key] = value;
                            }
                            else if (arrayValues.ContainsKey(name))
                            {
                                arrayValues[name]
                                    .Add(args[++i]);
                            }
                            else
                            {
                                values[name] = value;
                            }
                        }
                    }
                }
                else
                {
                    positionals.Add(arg);
                }
            }

            var switchAttributes = fields.Select(x => (field: x, attribute: x.GetCustomAttribute<SwitchAttribute>(true)))
                                         .Where(x => x.attribute != null)
                                         .OrderBy(x => x.attribute.Positional)
                                         .ToArray();

            foreach (var (field, switchAttribute) in switchAttributes)
            {
                if (switchAttribute.AllPositionals)
                {
                    field.SetValue(instance, positionals.ToArray());
                    continue;
                }

                var kind = FieldKind.Default;
                var t    = field.PropertyType;
                if (t.IsGenericType &&
                    t.GetGenericTypeDefinition()
                     .IsAssignableFrom(typeof(Dictionary<,>)))
                    kind = FieldKind.Dictionary;
                else if (t.IsGenericType &&
                         t.GetGenericTypeDefinition()
                          .IsAssignableFrom(typeof(List<>))) kind = FieldKind.Array;
                var value         = switchAttribute.Default;
                var insertedValue = false;
                if (switchAttribute.Positional > -1)
                {
                    if (positionals.Count > switchAttribute.Positional) value = positionals[switchAttribute.Positional];
                    if (presence.Contains(switchAttribute.Switch)) positionals.Insert(switchAttribute.Positional, string.Empty);
                }

                if (!string.IsNullOrWhiteSpace(switchAttribute.Switch))
                {
                    if (!presence.Contains(switchAttribute.Switch))
                    {
                        var aliasAttributes = field.GetCustomAttributes<SwitchAliasAttribute>(true)
                                                   .ToArray();
                        if (aliasAttributes.Length > 0)
                            foreach (var aliasAttribute in aliasAttributes)
                                if (presence.Contains(aliasAttribute.Alias))
                                {
                                    insertedValue = true;
                                    switch (kind)
                                    {
                                        case FieldKind.Default:
                                            value = values[aliasAttribute.Alias];
                                            break;
                                        case FieldKind.Dictionary:
                                            value = dictionaryValues[aliasAttribute.Alias];
                                            break;
                                        case FieldKind.Array:
                                            value = arrayValues[aliasAttribute.Alias];
                                            break;
                                        default:
                                            throw new ArgumentOutOfRangeException();
                                    }
                                }
                    }
                    else
                    {
                        insertedValue = true;
                        switch (kind)
                        {
                            case FieldKind.Default:
                                value = values[switchAttribute.Switch];
                                break;
                            case FieldKind.Dictionary:
                                value = dictionaryValues[switchAttribute.Switch];
                                break;
                            case FieldKind.Array:
                                value = arrayValues[switchAttribute.Switch];
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }

                if (value != null)
                {
                    if (switchAttribute.Valid != null)
                        if (!switchAttribute.Valid.Contains(value.ToString()))
                        {
                            Console.Error.WriteLine($"Value {value} is invalid for switch {switchAttribute.Switch}, valid values are {string.Join(", ", switchAttribute.Valid)}");
                            FullHelp<T>(extraHelp);
                            return null;
                        }

                    if (switchAttribute.Parser != null && insertedValue && switchAttribute.Parser.Length == 2)
                    {
                        var parserClass = Type.GetType(switchAttribute.Parser[0]);
                        var method      = parserClass?.GetMethod(switchAttribute.Parser[1]);
                        if (method != null)
                        {
                            var @params = method.GetParameters();
                            switch (kind)
                            {
                                case FieldKind.Default:
                                {
                                    if (@params.Length == 1 &&
                                        @params[0]
                                            .ParameterType.Name ==
                                        "String" &&
                                        method.ReturnType.Name == "Object") value = method.Invoke(null, new object[] { (string) value });
                                    break;
                                }
                                case FieldKind.Dictionary:
                                {
                                    if (@params.Length == 1 &&
                                        @params[0]
                                            .ParameterType.Name ==
                                        "Dictionary`2" &&
                                        method.ReturnType.Name == "Object") value = method.Invoke(null, new object[] { (Dictionary<string, string>) value });
                                    break;
                                }
                                case FieldKind.Array:
                                {
                                    if (@params.Length == 1 &&
                                        @params[0]
                                            .ParameterType.Name ==
                                        "List`1" &&
                                        method.ReturnType.Name == "Object") value = method.Invoke(null, new object[] { (List<string>) value });
                                    break;
                                }
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                    }

                    field.SetValue(instance, value);
                }
                else if (switchAttribute.Required)
                {
                    Console.Error.WriteLine(string.IsNullOrWhiteSpace(switchAttribute.Switch) ? $"Positional {switchAttribute.Positional} is required" : $"Switch {switchAttribute.Switch} is required");

                    FullHelp<T>(extraHelp);
                    return null;
                }
                else if (field.PropertyType.IsClass && field.PropertyType.GetConstructor(Type.EmptyTypes) != null)
                {
                    field.SetValue(instance, Activator.CreateInstance(field.PropertyType));
                }
            }

            return instance;
        }

        public static void LoadArgs()
        {
            if (!File.Exists(ArgFilePath)) return;
            using (var file = File.OpenRead(ArgFilePath))
            {
                AppArgs = new DataContractJsonSerializer(typeof(string[])).ReadObject(file) as string[] ?? Array.Empty<string>();
            }
        }

        public static void SaveArgs(params string[] extra)
        {
            DeleteArgs();
            var args = AppArgs.Where(x => x.StartsWith("-"))
                              .Concat(extra.Where(x => !string.IsNullOrWhiteSpace(x)))
                              .Reverse()
                              .ToArray();
            using (var file = File.OpenWrite(ArgFilePath))
            {
                new DataContractJsonSerializer(typeof(string[])).WriteObject(file, args);
            }
        }

        public static void ResetArgs()
        {
            AppArgs = Environment.GetCommandLineArgs()
                                 .Skip(1)
                                 .ToArray();
        }

        public static void DeleteArgs()
        {
            if (File.Exists(ArgFilePath)) File.Delete(ArgFilePath);
        }

        private enum FieldKind
        {
            Default,
            Array,
            Dictionary
        }
    }
}
