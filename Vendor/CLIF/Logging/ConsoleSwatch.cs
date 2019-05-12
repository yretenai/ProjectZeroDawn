using System;
using System.Drawing;
using System.Runtime.InteropServices;

// ReSharper disable IdentifierTypo

namespace CLIF.Logging
{
    public static class ConsoleSwatch
    {
        public const  string ColorReset                         = "\x1b[0m";
        private const int    STD_OUTPUT_HANDLE                  = -11;
        private const int    ENABLE_VIRTUAL_TERMINAL_PROCESSING = 4;

        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        public static bool IsVTEnabled { get; private set; } = !Environment.OSVersion.Platform.ToString()
                                                                           .StartsWith("Win", StringComparison.InvariantCultureIgnoreCase);

        public static bool IsVTCapable { get; private set; } = Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6;

        public static ConsoleColor AsConsoleColor(this DOSColor color) { return (ConsoleColor) color; }

        public static DOSColor AsDOSColor(this ConsoleColor color) { return (DOSColor) color; }

        public static XTermColor AsXTermColor(this DOSColor color)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (color)
            {
                case DOSColor.DarkGray:
                    return XTermColor.Grey;
                case DOSColor.DarkYellow:
                    return XTermColor.Yellow;
                case DOSColor.Yellow:
                    return XTermColor.LightYellow;
                case DOSColor.Gray:
                    return XTermColor.LightSlateGrey;
                default:
                    return Enum.TryParse(color.ToString(), out XTermColor col) ? col : XTermColor.DarkSlateGray;
            }
        }

        public static string ToForeground(this XTermColor color) { return $"\x1b[38;5;{(byte) color}m"; }

        public static string ToBackground(this XTermColor color) { return $"\x1b[48;5;{(byte) color}m"; }

        public static string ToForeground(this Color color) { return $"\x1b[38;2;{color.R};{color.G};{color.B}m"; }

        public static string ToBackground(this Color color) { return $"\x1b[48;2;{color.R};{color.G};{color.B}m"; }

        public static bool EnableVT()
        {
            if (IsVTEnabled) return true;

            if (!IsVTCapable) return false;

            unsafe
            {
                var hOut = GetStdHandle(STD_OUTPUT_HANDLE);
                if (hOut == INVALID_HANDLE_VALUE)
                {
                    IsVTCapable = false;
                    return false;
                }

                var dwMode = 0;
                if (!GetConsoleMode(hOut, &dwMode))
                {
                    IsVTCapable = false;
                    return false;
                }

                dwMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;
                if (!SetConsoleMode(hOut, dwMode))
                {
                    IsVTCapable = false;
                    return false;
                }

                IsVTEnabled = true;
                return true;
            }
        }

        [DllImport("Kernel32.dll")]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("Kernel32.dll")]
        private static extern unsafe bool GetConsoleMode(IntPtr hConsoleHandle, int* lpMode);

        [DllImport("Kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, int dwMode);
    }
}
