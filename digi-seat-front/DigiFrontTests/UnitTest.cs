
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using DigiSeatBack;
using DigiSeatShared;
using DigiSeatShared.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RichardSzalay.MockHttp;


namespace DigiFrontTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GetClient_Test()
        {
            Integration i = new Integration();
            var client = i.GetClient();
            var headers = client.DefaultRequestHeaders;
            var actual = "X-ApiKey: " + Settings.ApiKey + "\r\n";
            Assert.AreEqual(actual, headers.ToString());
        }

        [TestMethod]
        public async Task GetTable_Test()
        {
            //Setup
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"{Settings.ApiBaseUrl}/api/tables?partySize=3&type=myType&exclusionList=").Respond("application/json", "{\"status\":\"success\",\"bestTable\":{\"id\":2002,\"number\":\"800\",\"state\":\"holding\",\"tableType\":\"booth\",\"shape\":\"square\",\"capacity\":6,\"restaurantId\":1,\"lightAddress\":\"light09\",\"xCoordinate\":-510.1839,\"yCoordinate\":-358.1417,\"partySize\":3,\"seatedTime\":\"1900-01-01T00:00:00\",\"averageSeatDuration\":0},\"otherTables\":[],\"minutesWait\":null,\"partySize\":3}");
            var myclient = mockHttp.ToHttpClient();
            Integration i = new Integration(myclient);

            //Run method
            var table = await i.GetTable(3, "myType");

            Assert.AreEqual(table.Status, "success");
            Assert.AreEqual(table.BestTable.PartySize, 3);
            Assert.AreEqual(table.BestTable.Number, "800");
            Assert.AreEqual(table.OtherTables.Count, 0);
            Assert.AreEqual(table.PartySize, 3);
        }

        [TestMethod]
        public async Task GetAllWaits_Test()
        {
            //Setup
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"{Settings.ApiBaseUrl}/api/wait/all").Respond("application/json", "[{\"id\":3002,\"partySize\":2,\"created\":\"2018-04-14T23:05:11\",\"name\":\"WaitDude\",\"phone\":\"1111231234\",\"estimatedWait\":-16401,\"restaurantId\":1,\"code\":4119,\"state\":\"notified\",\"notifiedTime\":\"2018-04-15T07:14:13\",\"tableId\":2002,\"table\":null}, {\"id\":3002,\"partySize\":2,\"created\":\"2018-04-14T23:05:11\",\"name\":\"WaitDude2\",\"phone\":\"1111231234\",\"estimatedWait\":-16401,\"restaurantId\":1,\"code\":4119,\"state\":\"notified\",\"notifiedTime\":\"2018-04-15T07:14:13\",\"tableId\":2002,\"table\":null}]");
            var myclient = mockHttp.ToHttpClient();
            Integration iii = new Integration(myclient);

            //Run method
            var waits = await iii.GetAllWaits();

            //Assert
            Assert.AreEqual(waits.Count, 2);
            Assert.AreEqual("WaitDude", waits[0].Name);
            Assert.AreEqual("WaitDude2", waits[1].Name);
        }

        [TestMethod]
        public async Task DeleteWaitPerson_ValidRouteTest()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"{Settings.ApiBaseUrl}/api/wait/4").Respond("application/json", "Is okay");
            Integration i = new Integration(mockHttp.ToHttpClient());

            var result = await i.DeleteWaitPerson(4);

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public async Task DeleteWaitPerson_InvalidRouteTest()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"{Settings.ApiBaseUrl}/api/wait/4").Respond("application/json", "Is okay");
            Integration i = new Integration(mockHttp.ToHttpClient());

            var result = await i.DeleteWaitPerson(322);

            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public async Task GetResaurant_Test()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"{Settings.ApiBaseUrl}/api/restaurant").Respond("application/json", "{\"id\":1,\"name\":\"Red Robin\",\"apiKey\":\"a480edc7-811b-4cfa-a216-47fc31697ebe\",\"staff\":[{\"id\":1,\"name\":\"Server1\",\"ranking\":0,\"shiftStart\":null,\"state\":\"off\",\"restaurantId\":1,\"dateCreated\":\"2018-02-24T09:48:09\",\"deleted\":false},{\"id\":2,\"name\":\"ServerGuy2\",\"ranking\":0,\"shiftStart\":null,\"state\":\"on\",\"restaurantId\":1,\"dateCreated\":\"2018-03-01T04:10:00\",\"deleted\":false},{\"id\":1002,\"name\":\"serverdude\",\"ranking\":0,\"shiftStart\":null,\"state\":\"off\",\"restaurantId\":1,\"dateCreated\":\"2018-04-15T05:03:53\",\"deleted\":false}]}");
            Integration i = new Integration(mockHttp.ToHttpClient());

            var rest = await i.GetRestaurant();

            Assert.AreEqual(rest.Id, 1);
            Assert.AreEqual(rest.Name, "Red Robin");
            Assert.AreEqual(rest.Staff.Count, 3);
        }

        [TestMethod]
        public async Task CreateStaff_Test()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"{Settings.ApiBaseUrl}/api/restaurant/staff").Respond("application/json", "has been created");
            Integration i = new Integration(mockHttp.ToHttpClient());
            Staff staff = new Staff() { Id = 7, Name = "Staff Name" };

            var result = await i.CreateStaff(staff);

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public async Task UpdateStaff_Test()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"{Settings.ApiBaseUrl}/api/restaurant/staff/4").Respond("application/json", "successful operation");
            Integration i = new Integration(mockHttp.ToHttpClient());
            Staff staff = new Staff() { Id = 4, Name = "Staff Name" };

            var result = await i.UpdateStaff(staff);

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public async Task DeleteStaff_Test()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"{Settings.ApiBaseUrl}/api/restaurant/staff/6").Respond("application/json", "successful operation");
            Integration i = new Integration(mockHttp.ToHttpClient());

            var result = await i.DeleteStaff(6);

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public async Task GetWaitWithCode_Test() //The original method is buggy, stuff needs to be done to make it functional
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"{Settings.ApiBaseUrl}/api/wait?code=myCode").Respond("application/json", "{\"id\":3005,\"partySize\":8,\"created\":\"2018-04-15T19:02:13\",\"name\":\"WaitingMan\",\"phone\":\"\",\"estimatedWait\":-17599,\"restaurantId\":1,\"code\":8400,\"state\":\"waiting\",\"notifiedTime\":null,\"tableId\":0,\"table\":null}");
            Integration i = new Integration(mockHttp.ToHttpClient());

            var result = await i.GetWaitWithCode("myCode");

            Assert.AreEqual(1, 2);
        }

        [TestMethod]
        public async Task PostWait_Test()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"{Settings.ApiBaseUrl}/api/wait").Respond("application/json", "some message");
            Integration i = new Integration(mockHttp.ToHttpClient());
            var wait = new Wait() { Id = 1, Name = "wait man" };

            var result = await i.PostWait(wait);

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public async Task GetTables_Test()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"{Settings.ApiBaseUrl}/api/tables/all?restaurantId=1").Respond("application/json", "[{\"id\":1,\"number\":\"10\",\"state\":\"seated\",\"tableType\":\"table\",\"shape\":\"square\",\"capacity\":7,\"restaurantId\":1,\"lightAddress\":\"light1\",\"xCoordinate\":-533.7665,\"yCoordinate\":5.076858,\"partySize\":2,\"seatedTime\":\"2018-04-15T04:12:23\",\"averageSeatDuration\":0},{\"id\":2,\"number\":\"20\",\"state\":\"seated\",\"tableType\":\"table\",\"shape\":\"square\",\"capacity\":8,\"restaurantId\":1,\"lightAddress\":\"light2\",\"xCoordinate\":-248.9725,\"yCoordinate\":-335.2785,\"partySize\":3,\"seatedTime\":\"2018-04-03T13:27:51\",\"averageSeatDuration\":0},{\"id\":1002,\"number\":\"456\",\"state\":\"seated\",\"tableType\":\"table\",\"shape\":\"round\",\"capacity\":6,\"restaurantId\":1,\"lightAddress\":\"light9\",\"xCoordinate\":-188.1946,\"yCoordinate\":-97.22728,\"partySize\":3,\"seatedTime\":\"2018-04-03T13:37:00\",\"averageSeatDuration\":0},{\"id\":2002,\"number\":\"800\",\"state\":\"open\",\"tableType\":\"booth\",\"shape\":\"square\",\"capacity\":6,\"restaurantId\":1,\"lightAddress\":\"light09\",\"xCoordinate\":-510.1839,\"yCoordinate\":-358.1417,\"partySize\":0,\"seatedTime\":\"1900-01-01T00:00:00\",\"averageSeatDuration\":0},{\"id\":2003,\"number\":\"777\",\"state\":\"seated\",\"tableType\":\"booth\",\"shape\":\"square\",\"capacity\":8,\"restaurantId\":1,\"lightAddress\":\"light123\",\"xCoordinate\":-492.6522,\"yCoordinate\":-195.8817,\"partySize\":3,\"seatedTime\":\"2018-04-12T02:41:53\",\"averageSeatDuration\":0}]");
            Integration i = new Integration(mockHttp.ToHttpClient());

            var result = await i.GetTables();

            Assert.AreEqual(5, result.Count);
            Assert.AreEqual(7, result[0].Capacity);
            Assert.AreEqual(1, result[0].Id);
        }
        
        [TestMethod]
        public async Task SaveAllTables_Test()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"{Settings.ApiBaseUrl}/api/tables/save").Respond("application/json", "some message");
            Integration i = new Integration(mockHttp.ToHttpClient());

            List<Table> alltables = new List<Table>();
            Table t = new Table() { Id = 1, Number  = "tableNumb"};

            var result = await i.SaveAllTables(alltables);

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public async Task OpenTable_Test()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"{Settings.ApiBaseUrl}/api/tables/open/7").Respond("application/json", "some message");
            Integration i = new Integration(mockHttp.ToHttpClient());

            var result = await i.OpenTable(7);

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public async Task SitTable_Test()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"{Settings.ApiBaseUrl}/api/tables/sit/11?partySize=8").Respond("application/json", "{\"sectionLocation\":\"to your right, all the way to the back.\",\"lightColor\":\"LightGreen\"}");
            Integration i = new Integration(mockHttp.ToHttpClient());

            var result = await i.SitTable(11, 8);

            Assert.AreEqual("LightGreen", result.LightColor);
            Assert.AreEqual("to your right, all the way to the back.", result.SectionLocation);
          
        }


    }
}
