using System;
using System.Diagnostics.CodeAnalysis;

namespace CLIF.Switch
{
    [AttributeUsage(AttributeTargets.Property)]
    [Serializable]
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    public class SwitchAttribute : Attribute
    {
        public string   Switch         { get; set; }
        public string   Help           { get; set; }
        public bool     Required       { get; set; } = false;
        public int      Positional     { get; set; } = -1;
        public object   Default        { get; set; }
        public bool     NeedsValue     { get; set; } = false;
        public string[] Parser         { get; set; }
        public string[] Valid          { get; set; } = null;
        public bool     AllPositionals { get; set; }

        public new string ToString() { return Switch; }
    }
}
