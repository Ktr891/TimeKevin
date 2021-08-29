using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace timeKevin.Function.Entities
{
    public class TimeEntity : TableEntity
    {
        public int IdEmployee { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public bool Consolidate { get; set; }
    }
}
