using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigiSeatApi.Models
{
    public class Staff
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Ranking { get; set; }
        public DateTime? ShiftStart { get; set; }
        public string State { get; set; }
        public int RestaurantId { get; set; }
        public DateTime DateCreated { get; set; }
        public bool Deleted { get; set; }
        public List<int> Tables { get; set; }
    }
}
