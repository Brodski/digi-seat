using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigiSeatApi.Models
{
    public class Section
    {
        public int Id { get; set; }
        public int RestaurantId { get; set; }
        public int StaffCount { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
