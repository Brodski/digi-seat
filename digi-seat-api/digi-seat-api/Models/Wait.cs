using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigiSeatApi.Models
{
    public class Wait
    {
        public int Id { get; set; }
        public int PartySize { get; set; }
        public DateTime Created { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public int EstimatedWait { get; set; }
        public int RestaurantId { get; set; }
        public int Code { get; set; }
        public string State { get; set; }
        public DateTime? NotifiedTime { get; set; }
        public int TableId { get; set; }
        public Table Table { get; set; }
    }
}
