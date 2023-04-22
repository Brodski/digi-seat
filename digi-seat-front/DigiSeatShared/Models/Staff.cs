using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace DigiSeatShared.Models
{
    public class Staff
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Ranking { get; set; }
        public Brush Brush { get; set; }
        public DateTime? ShiftStart { get; set; }
        public string State { get; set; }
        public int RestaurantId { get; set; }
        public DateTime DateCreated { get; set; }
        public List<int> Tables { get; set; }
        public List<Table> assignedTables { get; set; }


        public Staff()
        {
            assignedTables = new List<Table>();
            Tables = new List<int>();
        }
    }
}
