using System;

namespace CLIF.Switch
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    // ReSharper disable once ClassNeverInstantiated.Global
    public class SwitchAliasAttribute : Attribute
    {
        public SwitchAliasAttribute() { }

        public SwitchAliasAttribute(string alias) { Alias = alias; }

        public string Alias { get; }

        public new string ToString() { return Alias; }
    }
}
