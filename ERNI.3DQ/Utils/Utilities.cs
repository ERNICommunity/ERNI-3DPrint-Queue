using System.Collections.Generic;

namespace ERNI.Q3D.Utils
{
    public static class Utilities
    {
        private static readonly List<string> _units = new List<string> { "B", "kB", "MB", "GB" };

        public static string BytesToUnits(long bytes)
        {
            var index = 0;

            while (bytes > 1024)
            {
                ++index;
                bytes = bytes / 1024;
            }

            return $"{bytes:N0} {_units[index]}";
        }
    }
}
