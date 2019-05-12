namespace CLIF.Switch
{
    public static class SwitchConverter
    {
        public static object CLISwitchBoolean(string @in) { return @in.ToLower() == "true" || @in.ToLower() == "1" || @in.ToLower() == "y" || @in.ToLower() == "yes"; }

        public static object CLISwitchBooleanInv(string @in) { return !(bool) CLISwitchBoolean(@in); }

        public static object CLISwitchInt(string @in) { return int.Parse(@in); }

        public static object CLISwitchByte(string @in) { return byte.Parse(@in); }

        public static object CLISwitchChar(string @in)
        {
            if (@in.Length == 0) return (char) 0;
            return @in[0];
        }
    }
}
