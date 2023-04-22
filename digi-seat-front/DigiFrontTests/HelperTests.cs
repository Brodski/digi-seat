using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigiSeatBack;
using DigiSeatShared;
using DigiSeatShared.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DigiFrontTests
{
    [TestClass]
    public class HelperTests
    {
        [TestMethod]
        public void AddOnShiftStaff_Test()
        {
            ObservableCollection<Staff> myStaffList = new ObservableCollection<Staff>();

            Staff s1 = new Staff() { Id = 1, Name = "staff 1", State = "on" };
            Staff s2 = new Staff() { Id = 11, Name = "staff 2", State = "off" };
            Staff s3 = new Staff() { Id = 20, Name = "staff 3", State = "off" };
            Staff s4 = new Staff() { Id = 24, Name = "staff 4", State = "on" };
            Staff s10 = new Staff() { Id = 911, Name = "staff in list 1", State = "on" };
            Staff s11 = new Staff() { Id = 900, Name = "staff in list 2", State = "off" };
            Restaurant rest = new Restaurant();
            rest.Staff = new List<Staff>();
            rest.Name = "Test rest";
            rest.Staff.Add(s1);
            rest.Staff.Add(s2);
            rest.Staff.Add(s3);
            rest.Staff.Add(s4);
            myStaffList.Add(s11);
            myStaffList.Add(s10);

            Helper h = new Helper();
            h.addOnShiftStaff(ref myStaffList, rest);

            Assert.AreEqual(4, myStaffList.Count);
            Assert.AreEqual(true, myStaffList.Contains(s1));
            Assert.AreEqual(true, myStaffList.Contains(s4));
            Assert.AreEqual(false, myStaffList.Contains(s2));

        }

        [TestMethod]
        public void GetRect_Test()
        {
            Table t = new Table() { Id = 11, Capacity = 5, Shape = "square", State = "open" };
            Helper h = new Helper();
            var myBorder = h.GetRect(t); //cannot create UWP object Border.

            Assert.AreEqual("table11", myBorder.Name);
//            Assert.AreEqual(70 * 1.4, myBorder.Height);
//            Assert.AreEqual(80 * 1.4, myBorder.Width);
//            Assert.AreEqual("table11", myBorder.Name);
        }

        [TestMethod]
        public void GetTableText_Test()
        {
            Table t = new Table() { Id = 11, Capacity = 5, Number="432", Shape = "square", State = "open" };
            Table t2 = new Table() { Id = 001, Capacity = 2, Number = "234", Shape = "square", State = "open" };
            t.SeatedTime = DateTime.UtcNow;
            t2.SeatedTime = null;

            Helper h = new Helper();

            var ret = h.GetTableText(t);
            var ret2 = h.GetTableText(t2);
            Assert.AreEqual("Num: 432 open\nCap: 5\n0:0", ret);
            Assert.AreEqual("Num: 234 open\nCap: 2\nSeatedTime: null", ret2);
        }

        [TestMethod]
        public void SaveAux_Test()
        {
            List<LayoutTable> tables = new List<LayoutTable>();
            LayoutTable LT1 = new LayoutTable();
            LayoutTable LT2 = new LayoutTable();
            LayoutTable LT3 = new LayoutTable();
            LayoutTable LT4 = new LayoutTable();
            LT1.Table = new Table();
            LT2.Table = new Table();
            LT3.Table = new Table();
            LT4.Table = new Table();

            LT1.Table.XCoordinate = 123;
            LT1.Transform = new Windows.UI.Xaml.Media.TranslateTransform(); //breaks here

            LT1.Transform.X = 123;
            LT1.Transform.Y = 123;

            LT2.Transform.X = 100;
            LT2.Transform.Y = 200;

            LT3.Transform.X = 1.5;
            LT3.Transform.Y = 7.5;

            LT4.Transform.X = 222;
            LT4.Transform.Y = 555;

            tables.Add(LT1);
            tables.Add(LT2);
            tables.Add(LT3);
            tables.Add(LT4);

            Helper h = new Helper();
            var listTables = h.Save_aux(tables);

        }

        [TestMethod]
        public void RoundRobin_Tests()
        {
            Staff s1 = new Staff() { Id = 11, ShiftStart = new DateTime(2018, 4, 1) };
            Staff s2 = new Staff() { Id = 16, ShiftStart = new DateTime(2018, 4, 5) };
            Staff s3 = new Staff() { Id = 19, ShiftStart = new DateTime(2018, 4, 8) };
            Staff s4 = new Staff() { Id = 19, ShiftStart = new DateTime(2018, 4, 14) };

            var staffList = new List<Staff>();

            Table t = new Table() { Id = 11, Capacity = 5, Number = "432", Shape = "square", State = "open" };
            Table t2 = new Table() { Id = 1001, Capacity = 2, Number = "234", Shape = "square", State = "open" };

            Table t3 = new Table() { Id = 22, Capacity = 3, Number = "4", Shape = "square", State = "open" };
            Table t4 = new Table() { Id = 12, Capacity = 4, Number = "2", Shape = "square", State = "open" };

            Table t5 = new Table() { Id = 55, Capacity = 4, Number = "4404", Shape = "square", State = "open" };
            Table t6 = new Table() { Id = 501, Capacity = 5, Number = "222", Shape = "square", State = "open" };

            var subList = new List<Table>();
            var subList2 = new List<Table>();
            var subList3 = new List<Table>();

            subList.Add(t);
            subList.Add(t2);
            subList2.Add(t3);
            subList2.Add(t4);
            subList3.Add(t5);
            subList3.Add(t6);

            Helper h = new Helper();

            Section sect1 = new Section();
            Section sect2 = new Section();
            Section sect3 = new Section();
            Section sect4 = new Section();
            sect1.server = s1;
            sect2.server = s2;
            sect3.server = s3;
            sect4.server = s4;
            sect1.Tables = subList;
            sect2.Tables = subList2;
            sect3.Tables = subList3;

            var sectionsList = new List<Section>();
            sectionsList.Add(sect3);
            sectionsList.Add(sect1);
            sectionsList.Add(sect4);
            sectionsList.Add(sect2);

            var tableAssign = new Table();
            h.doRoundRobinAssignment(ref sectionsList, tableAssign);
            
            //doRoundRobingAssignment will add a table to the next section in the queue. Then put that section at the end of the queue/list
            Assert.AreEqual(sectionsList.Last().Tables.Last(), tableAssign);
        }

    }
}
