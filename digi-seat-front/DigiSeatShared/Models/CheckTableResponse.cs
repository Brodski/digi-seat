using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiSeatShared.Models
{
    public class CheckTableResponse
    {
        public CheckTableResponse()
        {
            OtherTables = new List<Table>();
        }

        public string Status { get; set; }
        public Table BestTable { get; set; }
        public List<Table> OtherTables { get; set; }
        public int? MinutesWait { get; set; }
        public int PartySize { get; set; }
    }
}
