using System;

namespace ERNI.Q3D.Settings
{
    public class PrintWindowSettings
    {
        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public TimeSpan Break { get; set; }

        public TimeSpan MaximumOverfit { get; set; }
    }
}
