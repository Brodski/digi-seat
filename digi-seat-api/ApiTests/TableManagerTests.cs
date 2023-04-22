using Microsoft.VisualStudio.TestTools.UnitTesting;
using DigiSeatApi.Application;
using DigiSeatApi.Models;
using System;
using System.Threading.Tasks;
//using Windows.Web.Http;
//using Windows.Web.Http.Filters;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Moq;
using DigiSeatApi.DataAccess;
using System.Net;
using Castle.Core.Configuration;
using DigiSeatApi.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.Extensions.Primitives;

namespace ApiTests
{
    [TestClass]
    public class TableManagerTests
    {

        Mock<IDigiSeatRepo> repoMock;
        Mock<IWaitManager> waitManagerMock;
        Mock<IRestaurantManager> restManagerMock;
        TableManager tableManagerMock;

        [TestInitialize]
        public void TestInitialize()
        {
            repoMock = new Mock<IDigiSeatRepo>();
            waitManagerMock = new Mock<IWaitManager>();
            restManagerMock = new Mock<IRestaurantManager>();
            tableManagerMock = new TableManager(repoMock.Object, waitManagerMock.Object, restManagerMock.Object);
        }
        
        [TestMethod]
        public async Task GetBestAvailableests_twoTablesAreAvailable()
        {
            //Set up
            Restaurant r1 = new Restaurant { Name = "Restaurant name", Id = 2, ApiKey = "apikey1" };
            restManagerMock.Setup(x => x.GetRestaurantUsingHttpContext()).ReturnsAsync(r1);

            List<Table> tableList = new List<Table>();
            tableList.Add(new Table { Id = 13, Number = "tableNum 1", State = "seated", Capacity = 3, RestaurantId = 2, PartySize = 5, LightAddress = "L0", SeatedTime = new DateTime(2018, 3, 16, 20, 0, 0) }); // Year,M,D,hour, min, sec
            tableList.Add(new Table { Id = 15, Number = "tableNum 2", State = "seated", Capacity = 3, RestaurantId = 2, PartySize = 3, LightAddress = "L1", SeatedTime = new DateTime(2018, 3, 16, 20, 5, 0) }); // Year,M,D,hour, min, sec });
            tableList.Add(new Table { Id = 16, Number = "tableNum 3", State = "open", Capacity = 3, RestaurantId = 2, PartySize = 0, LightAddress = "L2" }); //1
            tableList.Add(new Table { Id = 21, Number = "tableNum 4", State = "open", Capacity = 2, RestaurantId = 2, PartySize = 0, LightAddress = "L3" });
            tableList.Add(new Table { Id = 22, Number = "tableNum 5", State = "open", Capacity = 6, RestaurantId = 2, PartySize = 0, LightAddress = "L4" }); //3
            tableList.Add(new Table { Id = 25, Number = "tableNum 6", State = "open", Capacity = 6, RestaurantId = 2, PartySize = 0, LightAddress = "L5" });
            tableList.Add(new Table { Id = 27, Number = "tableNum 7", State = "holding", Capacity = 3, RestaurantId = 2, PartySize = 0, LightAddress = "L6" });
            repoMock.Setup(x => x.GetTablesByRestaurantId(r1.Id)).ReturnsAsync(tableList);

            //Do test
            CheckTableResponse response = await tableManagerMock.GetBestAvailable(4);

            //Assert
            Assert.AreEqual(4, response.BestTable.PartySize);
            Assert.AreEqual("holding", response.BestTable.State);
            Assert.AreEqual("success", response.Status);
            Assert.AreEqual(1, response.OtherTables.Count());
            Assert.IsNotNull(response.BestTable);
            repoMock.Verify(x => x.SaveTables(It.Is<List<Table>>(i => i.Contains(response.BestTable))), Times.Exactly(1));
            repoMock.Verify(x => x.SaveTables(It.IsAny<List<Table>>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task GetBestAvailableests_tablesAreNotAvailable() //Open = null, type = ""
        {
            //Set up
            Restaurant r1 = new Restaurant { Name = "Restaurant name", Id = 2, ApiKey = "apikey1" };
            restManagerMock.Setup(x => x.GetRestaurantUsingHttpContext()).ReturnsAsync(r1);

            List<Table> tableList = new List<Table>();
            tableList.Add(new Table { Id = 13, Number = "tableNum 1", State = "seated", Capacity = 3, RestaurantId = 2, PartySize = 5, LightAddress = "L0", SeatedTime = new DateTime(2018, 3, 16, 20, 0, 0) }); // Year,M,D,hour, min, sec
            tableList.Add(new Table { Id = 15, Number = "tableNum 2", State = "seated", Capacity = 3, RestaurantId = 2, PartySize = 3, LightAddress = "L1", SeatedTime = new DateTime(2018, 3, 16, 20, 5, 0) }); // Year,M,D,hour, min, sec });
            tableList.Add(new Table { Id = 16, Number = "tableNum 3", State = "open", Capacity = 2, RestaurantId = 2, PartySize = 0, LightAddress = "L2" }); //1
            tableList.Add(new Table { Id = 21, Number = "tableNum 4", State = "open", Capacity = 2, RestaurantId = 2, PartySize = 0, LightAddress = "L3" });
            tableList.Add(new Table { Id = 27, Number = "tableNum 7", State = "holding", Capacity = 3, RestaurantId = 2, PartySize = 0, LightAddress = "L6" });
            repoMock.Setup(x => x.GetTablesByRestaurantId(r1.Id)).ReturnsAsync(tableList);

            //Do test
            CheckTableResponse response = await tableManagerMock.GetBestAvailable(3);
            
            // Assert
            Assert.AreEqual("wait", response.Status);
            Assert.IsNull(response.BestTable);
            Assert.IsNull(response.OtherTables);
            repoMock.Verify(x => x.SaveTables(It.IsAny<List<Table>>()), Times.Never());

        }

        [TestMethod]
        public async Task GetBestAvailableests_tablesAreNotAvailable2() //Open = null, type = "kajfkld"
        {
            //Set up
            Restaurant r1 = new Restaurant { Name = "Restaurant name", Id = 2, ApiKey = "apikey1" };
            restManagerMock.Setup(x => x.GetRestaurantUsingHttpContext()).ReturnsAsync(r1);

            List<Table> tableList = new List<Table>();
            tableList.Add(new Table { Id = 13, Number = "tableNum 1", State = "seated", Capacity = 3, RestaurantId = 2, PartySize = 5, LightAddress = "L0", TableType = "type1", SeatedTime = new DateTime(2018, 3, 16, 20, 0, 0) }); // Year,M,D,hour, min, sec
            tableList.Add(new Table { Id = 15, Number = "tableNum 2", State = "seated", Capacity = 3, RestaurantId = 2, PartySize = 3, LightAddress = "L1", TableType = "type2", SeatedTime = new DateTime(2018, 3, 16, 20, 5, 0) }); // Year,M,D,hour, min, sec });
            tableList.Add(new Table { Id = 16, Number = "tableNum 3", State = "open", Capacity = 2, RestaurantId = 2, PartySize = 0, LightAddress = "L2", TableType = "type1", }); //1
            tableList.Add(new Table { Id = 21, Number = "tableNum 4", State = "open", Capacity = 2, RestaurantId = 2, PartySize = 0, LightAddress = "L3", TableType = "type2", });
            tableList.Add(new Table { Id = 27, Number = "tableNum 7", State = "holding", Capacity = 3, RestaurantId = 2, PartySize = 0, LightAddress = "L6", TableType = "type2", });
            repoMock.Setup(x => x.GetTablesByRestaurantId(r1.Id)).ReturnsAsync(tableList);

            //Do test
            CheckTableResponse response = await tableManagerMock.GetBestAvailable(6, "type1");
            
            //Assert
            Assert.IsNull(response.BestTable);
            Assert.IsNull(response.MinutesWait);
            Assert.IsNull(response.Status);
            Assert.AreEqual(6, response.PartySize);

        }


        [TestMethod]
        public async Task GetBestAvailableests_removeType() // Tables are open, type = "something"
        {
            //Set up
            Restaurant r1 = new Restaurant { Name = "Restaurant name", Id = 2, ApiKey = "apikey1" };
            restManagerMock.Setup(x => x.GetRestaurantUsingHttpContext()).ReturnsAsync(r1);

            List<Table> tableList = new List<Table>();
            tableList.Add(new Table { Id = 13, Number = "tableNum 1", State = "seated", Capacity = 3, RestaurantId = 2, PartySize = 5, LightAddress = "L0", TableType = "type1", SeatedTime = new DateTime(2018, 3, 16, 20, 0, 0) }); // Year,M,D,hour, min, sec
            tableList.Add(new Table { Id = 15, Number = "tableNum 2", State = "seated", Capacity = 3, RestaurantId = 2, PartySize = 3, LightAddress = "L1", TableType = "type2", SeatedTime = new DateTime(2018, 3, 16, 20, 5, 0) }); // Year,M,D,hour, min, sec });
            tableList.Add(new Table { Id = 16, Number = "tableNum 3", State = "open", Capacity = 2, RestaurantId = 2, PartySize = 0, LightAddress = "L2", TableType = "type1", }); //1
            tableList.Add(new Table { Id = 21, Number = "tableNum 4", State = "open", Capacity = 2, RestaurantId = 2, PartySize = 0, LightAddress = "L3", TableType = "type2", });
            tableList.Add(new Table { Id = 23, Number = "tableNum 4.1", State = "open", Capacity = 2, RestaurantId = 2, PartySize = 0, LightAddress = "L3.1", TableType = "type2", });
            tableList.Add(new Table { Id = 23, Number = "tableNum 4.2", State = "open", Capacity = 2, RestaurantId = 2, PartySize = 0, LightAddress = "L3.2", TableType = "type2", });
            tableList.Add(new Table { Id = 27, Number = "tableNum 7", State = "holding", Capacity = 3, RestaurantId = 2, PartySize = 0, LightAddress = "L6", TableType = "type2", });
            repoMock.Setup(x => x.GetTablesByRestaurantId(r1.Id)).ReturnsAsync(tableList);

            //Do test
            CheckTableResponse response = await tableManagerMock.GetBestAvailable(2, "type2");

            Assert.IsNotNull(response.BestTable);
            Assert.AreEqual(2, response.BestTable.PartySize);
            Assert.AreEqual("holding", response.BestTable.State);
            Assert.AreEqual("success", response.Status);
            Assert.AreEqual(2, response.OtherTables.Count());
        }

        [TestMethod]
        public async Task GetBestAvailableests_exclusionList() //exlusionList = tables with id
        {
            //Set up
            Restaurant r1 = new Restaurant { Name = "Restaurant name", Id = 2, ApiKey = "apikey1" };
            restManagerMock.Setup(x => x.GetRestaurantUsingHttpContext()).ReturnsAsync(r1);

            List<Table> tableList = new List<Table>();
            tableList.Add(new Table { Id = 16, Number = "tableNum 3", State = "open", Capacity = 2, RestaurantId = 2, PartySize = 0, LightAddress = "L2", TableType = "type1", }); //1
            tableList.Add(new Table { Id = 21, Number = "tableNum 4", State = "open", Capacity = 2, RestaurantId = 2, PartySize = 0, LightAddress = "L3", TableType = "type2", });
            tableList.Add(new Table { Id = 23, Number = "tableNum 4.1", State = "open", Capacity = 2, RestaurantId = 2, PartySize = 0, LightAddress = "L3.1", TableType = "type2", });
            tableList.Add(new Table { Id = 23, Number = "tableNum 4.2", State = "open", Capacity = 2, RestaurantId = 2, PartySize = 0, LightAddress = "L3.2", TableType = "type2", });
            repoMock.Setup(x => x.GetTablesByRestaurantId(r1.Id)).ReturnsAsync(tableList);

            List<int> exclusionList = new List<int>();
            exclusionList.Add(tableList[2].Id);
            exclusionList.Add(tableList[3].Id);

            //Do test
            CheckTableResponse response = await tableManagerMock.GetBestAvailable(2, "type2", exclusionList);

            Assert.IsNotNull(response.BestTable);
            Assert.AreEqual(2, response.BestTable.PartySize);
            Assert.AreEqual("holding", response.BestTable.State);
            Assert.AreEqual("success", response.Status);
            Assert.AreEqual(0, response.OtherTables.Count());
            Assert.AreEqual("tableNum 4", response.BestTable.Number);
        }

        [TestMethod]
        public async Task OpenTable_Test() { 

            Table myTable = new Table { Id = 15, Number = "tableNum 2", State = "seated", Capacity = 3, RestaurantId = 2, PartySize = 3, LightAddress = "L1", SeatedTime = new DateTime(2018, 3, 16, 20, 5, 0) }; // Year,M,D,hour, min, sec });
            repoMock.Setup(x => x.GetTable(It.Is<int>(i => i == 15))).ReturnsAsync(myTable);

            await tableManagerMock.OpenTable(15);
            
            repoMock.Verify(x => x.SaveTables(It.IsAny<List<Table>>()), Times.Exactly(1));
            waitManagerMock.Verify(x => x.CheckWaitList(It.IsAny<int>()), Times.Exactly(1));
        }
    }
}