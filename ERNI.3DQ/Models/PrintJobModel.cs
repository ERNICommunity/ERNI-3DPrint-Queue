using System;

namespace ERNI.Q3D.Models
{
    public class PrintJobModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public DateTime CreatedAt { get; set; }

        public long SizeInBytes { get; set; }

        public string Owner { get; set; }

        public double FilementLength { get; set; }

        public TimeSpan PrintTime { get; set; }

        public DateTime? PrintStartedAt { get; set; }

        public string Link { get; set; }
    }
}
