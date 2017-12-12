using System;
using System.Collections.Generic;

namespace ERNI.Q3D.Models
{
    public class PrintJobListModel
    {
        public ICollection<PrintJobModel> Jobs { get; set; }

        public DateTime IntervalStart { get; set; }

        public bool IsAdmin { get; set; }
    }
}
