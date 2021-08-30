using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace timeKevin.Function.Entities
{
    public class ConsolidatedEntity : TableEntity
    {
        public int IdEmployee { get; set; }
        public DateTime Date { get; set; }
        public int MinutesWork { get; set; }
    }
}
