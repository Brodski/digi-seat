using DigiSeatShared;
using DigiSeatShared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using static DigiSeatBack.MainPage;

namespace DigiSeatBack
{
    public class Helper
    {
        public void addOnShiftStaff(ref ObservableCollection<Staff> myStaffList, Restaurant rest)
        {
            foreach (var staff in rest.Staff)
            {
                if (staff.State == "on")
                {
                    myStaffList.Add(staff);
                }
            }
        }

        public Border GetRect(Table table)
        {
            var capacity = table.Capacity;
            var shape = table.Shape;

            var baseHeight = 70;
            var baseWidth = 80;

            //Default height and width is for an average sized table
            var item = new Border
            {
                Name = $"table{table.Id}",
//                Background = ColorHelper.GetTableColor(table.State),
                BorderThickness = new Thickness(2),
                Padding = new Thickness(6, 1, 2, 3),
                ManipulationMode = ManipulationModes.All
            };

            if (capacity >= 4)
            {
                item.Height = baseHeight * 1.4;
                item.Width = baseWidth * 1.4;
            }

            else if (capacity < 4)
            {
                item.Height = baseHeight * .8;
                item.Width = baseWidth * .8;
            }

            return item;
        }

        public string GetTableText(Table table)
        {
            string tableText;
            tableText = "Num: " + table.Number + " " + table.State;
            tableText = tableText + "\nCap: " + table.Capacity;

            if (table.SeatedTime == null)
            {
                tableText = tableText + "\nSeatedTime: null";
            }
            else //Provide a timer for how long the party has been seated
            {
                var timeDiff = DateTime.UtcNow - table.SeatedTime.Value;
                int hours = (int)timeDiff.TotalHours;
                int minutes = timeDiff.Minutes;
                int seconds = timeDiff.Seconds;

                tableText = tableText + "\n" + hours + ":" + minutes;

                Debug.WriteLine("---------------");
                Debug.WriteLine("DateTime.UtcNow " + DateTime.UtcNow + " | table.SeatedTime.Value " + table.SeatedTime.Value);
                Debug.WriteLine("TotalHours: " + (int)(DateTime.UtcNow - table.SeatedTime.Value).TotalHours);
                Debug.WriteLine("Minutes: " + (DateTime.UtcNow - table.SeatedTime.Value).Minutes);
            }
            return tableText;
        }

        public List<Table> Save_aux(List<LayoutTable> tableList)
        {
            foreach (var t in tableList)
            {
                t.Table.XCoordinate = (float)t.Transform.X;
                t.Table.YCoordinate = (float)t.Transform.Y;
            }
            var alltables = tableList.Select(x => x.Table).ToList();
            return alltables;

        }

        public List<Section> CreateSectionList(ObservableCollection<Staff> staffList, List<LayoutTable> tables)
        {

            List<Table> monitor = new List<Table>();
            List<Section> sectionList = new List<Section>();
            foreach (var staff in staffList)
            {
                Section s = new Section();
                foreach (var tablId in staff.Tables) //will crash with null
                {
                    var Ltable = tables.Find(o => o.Table.Id == tablId);
                    s.Tables.Add(Ltable.Table);

                    if (monitor.Contains(Ltable.Table))
                    {
                        Debug.WriteLine("~~~~~~~~\nBugged sections. Doing cheeky fix. Should debug it sometime\n~~~~~~~~~~~");
                        Dialogues dia = new Dialogues();
                        dia.ShowErrorDialog("Some shit when wrong with the section, two people were assigned the same table");
                        s.Tables.Remove(Ltable.Table);
                    }
                    monitor.Add(Ltable.Table);
                }

                s.server = staff;
                sectionList.Add(s);
            }

            return sectionList;
        }

        public Table doRoundRobinAssignment(ref List<Section> serverSections, Table tableOfCustomers = null)
        {
            setupRRqueue(ref serverSections);
            var table = getRRTable(ref serverSections);

            return table;
        }

        // Sorts & sets up queues
        public void setupRRqueue(ref List<Section> serverSections)
        {
            serverSections.Sort();
            serverSections.Reverse();
            for (int i = 0; i < serverSections.Count; i++)
            {
                serverSections[i].queuePosition = i + 1;
            }
        }
        public Table getRRTable(ref List<Section> sectionList)
        {
            Table retTable = new Table();
            List<Table> aSectionsTables = new List<Table>();
            int counter = 0;
            for (int i = 0; i < sectionList.Count; i ++)
            {
                aSectionsTables = sectionList[i].Tables.Where(x => x.State == "open").ToList();
                if (aSectionsTables != null && aSectionsTables.Any()) // Can add more logic here like "and if there is 2 or more open-table difference between any two servers"
                {
                    break;
                }
                counter = counter + 1;
            }

            if (aSectionsTables != null && aSectionsTables.Any()) 
            {
                this.UpdateQueue(ref sectionList);
                retTable = aSectionsTables[0];
            }
            return retTable;
        }

        public void UpdateQueue(ref List<Section> serverSections)
        {
            foreach (var sec in serverSections)
            {
                sec.queuePosition = ((sec.queuePosition + 1) % (serverSections.Count)) + 1;
            }
            

        }


    }
}
