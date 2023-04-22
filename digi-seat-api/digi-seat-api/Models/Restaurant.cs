using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DigiSeatApi.Models
{
    public class Restaurant
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string ApiKey { get; set; }
        public List<Staff> Staff { get; set; }
    }
}
