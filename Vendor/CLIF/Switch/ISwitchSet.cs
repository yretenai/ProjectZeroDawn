using System;
using System.Management.Automation;

namespace CLIF.Switch
{
    [Serializable]
    public abstract class ISwitchSet
    {
        [Switch(AllPositionals = true)]
        public string[] Positionals { get; set; }

        [Switch(Switch = "h", Default = false, Help = "Print this help text", Parser = new[] { "CLIF.Switch.SwitchConverter", "CLISwitchBoolean" })]
        [SwitchAlias("help")]
        [Parameter(HelpMessage = "Print this help text")]
        public bool Help { get; set; }

        public abstract bool Validate();
    }
}
