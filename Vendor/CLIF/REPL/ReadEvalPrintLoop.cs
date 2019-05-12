using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using CLIF.Logging;

namespace CLIF.REPL
{
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Local")]
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    public class ReadEvalPrintLoop
    {
        public delegate IEnumerable<string> AutoCompleteCallback(string command, int cursor);

        public delegate bool ExecuteCallback(string command);

        public ReadEvalPrintLoop(AutoCompleteCallback autoCompleteCallback,
                                 ExecuteCallback      executeCallback,
                                 string               prompt      = "$>",
                                 string               historyFile = null)
        {
            AutoCompleteHook = autoCompleteCallback;
            ExecuteHook      = executeCallback;
            Prompt           = prompt;
            if (historyFile != null)
                History = File.ReadAllText(historyFile)
                              .Split(Environment.NewLine)
                              .ToList();
        }

        private AutoCompleteCallback AutoCompleteHook { get; set; }
        private ExecuteCallback      ExecuteHook      { get; set; }
        public  string               Prompt           { get; set; }
        public  List<string>         History          { get; set; }

        public ReadEvalPrintLoop ShellLike => new ReadEvalPrintLoop(CommandParser, CommandRunner, "0>");

        private CancellationTokenSource Cancel { get; set; }

        ~ReadEvalPrintLoop() { Stop(); }

        public bool CommandRunner(string command) { throw new NotImplementedException(); }

        public IEnumerable<string> CommandParser(string command, int cursor) { throw new NotImplementedException(); }

        public void Stop() { Cancel?.Cancel(); }

        public void Start()
        {
            if (Cancel != null) return;
            Cancel = new CancellationTokenSource();
            var compiledPrompt = XTermColor.Green.ToForeground() + Prompt + XTermColor.White.ToForeground() + " ";
            Console.Out.Write(compiledPrompt);
            var stringBuilder     = new StringBuilder();
            var keyPos            = 0;
            var histPos           = History.Count;
            var histStack         = string.Empty;
            var autoCompleteStack = default(string[]);
            var autoCompleteIndex = 0;
            while (!Cancel.IsCancellationRequested)
            {
                var key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Tab)
                {
                    autoCompleteStack = null;
                    autoCompleteIndex = 0;
                }

                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (key.Key)
                {
                    case ConsoleKey.Tab:
                        autoCompleteStack = autoCompleteStack ??
                                            AutoCompleteHook?.Invoke(stringBuilder.ToString(), keyPos)
                                                            .ToArray();
                        if (autoCompleteStack == null || autoCompleteStack.Length == 0 || autoCompleteStack.Length <= autoCompleteIndex) continue;
                        stringBuilder.Clear();
                        var mod = key.Modifiers.HasFlag(ConsoleModifiers.Shift) ? -1 : 1;
                        stringBuilder.Append(autoCompleteStack[autoCompleteIndex + mod]);
                        autoCompleteIndex += mod;
                        keyPos = stringBuilder.Length;
                        break;
                    case ConsoleKey.LeftArrow:
                        keyPos -= 1;
                        break;
                    case ConsoleKey.RightArrow:
                        keyPos += 1;
                        break;
                    case ConsoleKey.UpArrow when histPos == 0:
                        break;
                    case ConsoleKey.UpArrow:
                        if (histPos == History.Count) histStack = stringBuilder.ToString();
                        stringBuilder.Clear();
                        stringBuilder.Append(History[histPos - 1]);
                        keyPos  =  stringBuilder.Length;
                        histPos -= 1;
                        break;
                    case ConsoleKey.DownArrow when histPos == History.Count + 1:
                        break;
                    case ConsoleKey.DownArrow:
                        stringBuilder.Clear();
                        stringBuilder.Append(histPos == History.Count ? histStack : History[histPos + 1]);
                        keyPos  =  stringBuilder.Length;
                        histPos += 1;
                        break;
                    case ConsoleKey.Enter:
                    {
                        var color = XTermColor.Green;
                        Console.Out.Write(Environment.NewLine);
                        if (ExecuteHook?.Invoke(stringBuilder.ToString()) == false) color = XTermColor.Red;
                        stringBuilder.Clear();
                        compiledPrompt = color.ToForeground() + Prompt + XTermColor.White.ToForeground() + " ";
                        keyPos         = 0;
                        break;
                    }
                    case ConsoleKey.Backspace:
                        stringBuilder.Remove(keyPos, 1);
                        keyPos -= 1;
                        break;
                    default:
                        stringBuilder.Insert(keyPos, key.KeyChar);
                        keyPos += 1;
                        break;
                }

                Console.Out.Write("\r");
                var top = Console.CursorTop;
                Console.Out.Write(compiledPrompt + stringBuilder);
                // ReSharper disable once PossibleLossOfFraction
                var consoleTopOffset  = (int) Math.Floor((double) ((compiledPrompt + stringBuilder).Length / Console.BufferWidth));
                var consoleLeftOffset = (compiledPrompt.Length + keyPos) % Console.BufferWidth;
                Console.SetCursorPosition(consoleLeftOffset, top + consoleTopOffset);
            }

            Cancel.Dispose();
            Cancel = null;
        }
    }
}
