using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiSeatShared.Models
{
    public class Table
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string State { get; set; }
        public string TableType { get; set; }
        public string Shape { get; set; }
        public int Capacity { get; set; }
        public int RestaurantId { get; set; }
        public string LightAddress { get; set; }
        public float XCoordinate { get; set; }
        public float YCoordinate { get; set; }
        public int PartySize { get; set; }
        public DateTime? SeatedTime { get; set; }
        //public Staff Server { get; set; }
        public int? ServerId { get; set; }
        public DateTime? assignedServerTime { get; set; }
  //      public int? PreviousServerId { get; set; }
    }
}
