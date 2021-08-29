using System;

namespace timeKevin.Common.Models
{
    public class Time
    {
        public int IdEmployee { get; set; }
        public DateTime EntryDate { get; set; }
        public string Type { get; set; }
        public bool Consolidate { get; set; }
    }
}
