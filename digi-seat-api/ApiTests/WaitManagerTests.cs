using DigiSeatApi.Application;
using DigiSeatApi.Interfaces;
using DigiSeatApi.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTests
{
    [TestClass]
    public class WaitManagerTests
    {
        Mock<IDigiSeatRepo> repoMock;
        Mock<IRestaurantManager> restManagerMock;
        WaitManager WaitManagerMock;

        [TestInitialize]
        public void TestInitialize()
        {
            repoMock = new Mock<IDigiSeatRepo>();
            restManagerMock = new Mock<IRestaurantManager>();
            WaitManagerMock = new WaitManager(repoMock.Object, restManagerMock.Object);
        }

        /*
         
            tableList.Add(new Table { Id = 13, Number = "tableNum 1", State = "seated", Capacity = 7, RestaurantId = 2, LightAddress = "Light1", PartySize = 5, SeatedTime = new DateTime(2018, 1, 22) });
            tableList.Add(new Table { Id = 15, Number = "tableNum 2", State = "seated", Capacity = 3, RestaurantId = 2, LightAddress = "Light2", PartySize = 3, SeatedTime = new DateTime(2018, 1, 25) });
            tableList.Add(new Table { Id = 16, Number = "tableNum 3", State = "open", Capacity = 4, RestaurantId = 2, LightAddress = "Light3", PartySize = 0  });
            tableList.Add(new Table { Id = 21, Number = "tableNum 4", State = "open", Capacity = 7, RestaurantId = 2, LightAddress = "Light4", PartySize = 0  });

            tableList.Add(new Table { Id = 255, Number = "tableNum 10", State = "seated", Capacity = 3, RestaurantId = 5, LightAddress = "Light20", PartySize = 3, SeatedTime = new DateTime(2018, 2, 10) });
            tableList.Add(new Table { Id = 224, Number = "tableNum 11", State = "open", Capacity = 6, RestaurantId = 5, LightAddress = "Light21", PartySize = 0 });
            tableList.Add(new Table { Id = 122, Number = "tableNum 12", State = "open", Capacity = 3, RestaurantId = 5, LightAddress = "Light22", PartySize = 0 });
                  
      //      List<Restaurant> restlist = new List<Restaurant>();
//            restlist.Add(new Restaurant { Name = "Rest name 1", Id = 13, ApiKey = "API 1" });
  //          restlist.Add(new Restaurant { Name = "Rest name 2", Id = 211, ApiKey = "API 2" });
    //        restlist.Add(new Restaurant { Name = "Rest name 3", Id = 312, ApiKey = "API 3" });
        */

        [TestMethod]
        public async Task CheckWaitList_mixedData() 
        {
            // Setup
            int restId = 2;
            List<Wait> waitList = new List<Wait>();
            waitList.Add(new Wait { Name = "Wait Person 0",   Id = 13, RestaurantId = 2,   State = "waiting",     PartySize = 3 });
            waitList.Add(new Wait { Name = "Wait Person 1",   Id = 15, RestaurantId = 2,   State = "waiting",     PartySize = 7 });
            waitList.Add(new Wait { Name = "Wait Person 1.1", Id = 17, RestaurantId = 2,   State = "not waiting", PartySize = 4 });
//            waitList.Add(new Wait { Name = "Wait Person 2",   Id = 23, RestaurantId = 5,   State = "not waiting", PartySize = 14 });
  //          waitList.Add(new Wait { Name = "Wait Person 3",   Id = 30, RestaurantId = 5,   State = "waiting",     PartySize = 11 });

            repoMock.Setup(x => x.GetWaitListByRestaurantId(restId)).ReturnsAsync(waitList);


            List<Table> tableList = new List<Table>();
            tableList.Add(new Table { Id = 13, Number = "tableNum 1", State = "seated", Capacity = 7, RestaurantId = 2, PartySize = 5 });
            tableList.Add(new Table { Id = 15, Number = "tableNum 2", State = "seated", Capacity = 3, RestaurantId = 2, PartySize = 3 });
            tableList.Add(new Table { Id = 16, Number = "tableNum 3", State = "open",   Capacity = 4, RestaurantId = 2, PartySize = 0  });
            tableList.Add(new Table { Id = 21, Number = "tableNum 4", State = "open",   Capacity = 7, RestaurantId = 2, PartySize = 0  });
            tableList.Add(new Table { Id = 22, Number = "tableNum 5", State = "open",   Capacity = 3, RestaurantId = 2, PartySize = 0 });
            //            tableList.Add(new Table { Id = 255, Number = "tableNum 10", State = "seated", Capacity = 3, RestaurantId = 5, PartySize = 3 });
            //          tableList.Add(new Table { Id = 224, Number = "tableNum 11", State = "open", Capacity = 6, RestaurantId = 5, PartySize = 0 });
            //        tableList.Add(new Table { Id = 122, Number = "tableNum 12", State = "open", Capacity = 3, RestaurantId = 5, PartySize = 0 });

            repoMock.Setup(x => x.GetTablesByRestaurantId(restId)).ReturnsAsync(tableList);

            // Do test
            await WaitManagerMock.CheckWaitList(restId);
            
            // Assert
            repoMock.Verify(x => x.SaveWaits(It.IsAny<List<Wait>>()), Times.AtLeastOnce());
            repoMock.Verify(x => x.SaveTables(It.IsAny<List<Table>>()), Times.AtLeastOnce());
//            repoMock.Verify(x => x.SaveWaits(It.IsAny<List<Wait>>()), Times.Exactly(waitList.Count()));
        }


        
        [TestMethod]
        public async Task CheckWaitList_allShouldBeMatched() // 4 Parties of 1, 5 Open Tables
        {
            // Setup
            int restId = 2;
            List<Wait> waitList = new List<Wait>();
            waitList.Add(new Wait { Name = "Wait Person 0",     Id = 13,    RestaurantId = 2, State = "waiting", PartySize = 1 }); //1
            waitList.Add(new Wait { Name = "Wait Person 1",     Id = 15,    RestaurantId = 2, State = "waiting", PartySize = 1 });
            waitList.Add(new Wait { Name = "Wait Person 1.1",   Id = 17,    RestaurantId = 2, State = "waiting", PartySize = 1 }); //3
            waitList.Add(new Wait { Name = "Wait Person 2",     Id = 19,    RestaurantId = 2, State = "waiting", PartySize = 1 });
            waitList.Add(new Wait { Name = "not Waiting Person 3",   Id = 21, RestaurantId = 2, State = "not waiting", PartySize = 1 }); //5
            waitList.Add(new Wait { Name = "not Waiting Person 4",   Id = 22, RestaurantId = 2, State = "not waiting", PartySize = 1 });
            repoMock.Setup(x => x.GetWaitListByRestaurantId(restId)).ReturnsAsync(waitList);
            
            List<Table> tableList = new List<Table>();
            tableList.Add(new Table { Id = 13, Number = "tableNum 1", State = "seated", Capacity = 8, RestaurantId = 2, PartySize = 5 }); 
            tableList.Add(new Table { Id = 15, Number = "tableNum 2", State = "seated", Capacity = 8, RestaurantId = 2, PartySize = 3 }); 
            tableList.Add(new Table { Id = 16, Number = "tableNum 3", State = "open",   Capacity = 8, RestaurantId = 2, PartySize = 0 }); //1
            tableList.Add(new Table { Id = 21, Number = "tableNum 4", State = "open",   Capacity = 8, RestaurantId = 2, PartySize = 0 }); 
            tableList.Add(new Table { Id = 22, Number = "tableNum 5", State = "open",   Capacity = 8, RestaurantId = 2, PartySize = 0 }); //3
            tableList.Add(new Table { Id = 24, Number = "tableNum 5", State = "open",   Capacity = 8, RestaurantId = 2, PartySize = 0 });
            tableList.Add(new Table { Id = 25, Number = "tableNum 5", State = "open",   Capacity = 8, RestaurantId = 2, PartySize = 0 }); //5
            repoMock.Setup(x => x.GetTablesByRestaurantId(restId)).ReturnsAsync(tableList);

            // Do test
            await WaitManagerMock.CheckWaitList(restId);

            // Assert
            repoMock.Verify(x => x.SaveWaits(It.IsAny<List<Wait>>()), Times.Exactly(4));
            repoMock.Verify(x => x.SaveTables(It.IsAny<List<Table>>()), Times.Exactly(4));
        }

        [TestMethod]
        public async Task CheckWaitList_allShouldBeMatched2() // 4 parties of 1, 3 Open Tables
        {
            // Setup
            int restId = 2;
            List<Wait> waitList = new List<Wait>();
            waitList.Add(new Wait { Name = "Wait Person 0",         Id = 13, RestaurantId = 2, State = "waiting", PartySize = 1 }); //1
            waitList.Add(new Wait { Name = "Wait Person 1",         Id = 15, RestaurantId = 2, State = "waiting", PartySize = 1 });
            waitList.Add(new Wait { Name = "Wait Person 1.1",       Id = 17, RestaurantId = 2, State = "waiting", PartySize = 1 }); //3
            waitList.Add(new Wait { Name = "Wait Person 2",         Id = 19, RestaurantId = 2, State = "waiting", PartySize = 1 });
            waitList.Add(new Wait { Name = "not Waiting Person 3", Id = 21, RestaurantId = 2, State = "not waiting", PartySize = 1 }); //5
            waitList.Add(new Wait { Name = "not Waiting Person 4", Id = 22, RestaurantId = 2, State = "not waiting", PartySize = 1 });
            repoMock.Setup(x => x.GetWaitListByRestaurantId(restId)).ReturnsAsync(waitList);

            List<Table> tableList = new List<Table>();
            tableList.Add(new Table { Id = 13, Number = "tableNum 1", State = "seated", Capacity = 8, RestaurantId = 2, PartySize = 5 });
            tableList.Add(new Table { Id = 15, Number = "tableNum 2", State = "seated", Capacity = 8, RestaurantId = 2, PartySize = 3 });
            tableList.Add(new Table { Id = 16, Number = "tableNum 3", State = "open", Capacity = 8, RestaurantId = 2, PartySize = 0 }); //1
            tableList.Add(new Table { Id = 21, Number = "tableNum 4", State = "open", Capacity = 8, RestaurantId = 2, PartySize = 0 });
            tableList.Add(new Table { Id = 22, Number = "tableNum 5", State = "open", Capacity = 8, RestaurantId = 2, PartySize = 0 }); //3
            repoMock.Setup(x => x.GetTablesByRestaurantId(restId)).ReturnsAsync(tableList);

            // Do test
            await WaitManagerMock.CheckWaitList(restId);

            // Assert
            repoMock.Verify(x => x.SaveWaits(It.IsAny<List<Wait>>()), Times.Exactly(3));
            repoMock.Verify(x => x.SaveTables(It.IsAny<List<Table>>()), Times.Exactly(3));
        }

        [TestMethod]
        public async Task CheckWaitList_TableCapacityFitsNone() // 2 parties of 7, 4 Open Tables w/ capacity of 3
        {
            // Setup
            int restId = 2;
            List<Wait> waitList = new List<Wait>();
            waitList.Add(new Wait { Name = "Wait Person 0", Id = 13, RestaurantId = 2, State = "waiting", PartySize = 8 }); //1
            waitList.Add(new Wait { Name = "Wait Person 1", Id = 15, RestaurantId = 2, State = "waiting", PartySize = 7 });
            waitList.Add(new Wait { Name = "not Waiting Person 3", Id = 21, RestaurantId = 2, State = "not waiting", PartySize = 1 }); //3
            waitList.Add(new Wait { Name = "not Waiting Person 4", Id = 22, RestaurantId = 2, State = "not waiting", PartySize = 1 });
            repoMock.Setup(x => x.GetWaitListByRestaurantId(restId)).ReturnsAsync(waitList);

            List<Table> tableList = new List<Table>();
            tableList.Add(new Table { Id = 13, Number = "tableNum 1", State = "seated", Capacity = 3, RestaurantId = 2, PartySize = 5 });
            tableList.Add(new Table { Id = 15, Number = "tableNum 2", State = "seated", Capacity = 3, RestaurantId = 2, PartySize = 3 });
            tableList.Add(new Table { Id = 16, Number = "tableNum 3", State = "open", Capacity = 3, RestaurantId = 2, PartySize = 0 }); //1
            tableList.Add(new Table { Id = 21, Number = "tableNum 4", State = "open", Capacity = 3, RestaurantId = 2, PartySize = 0 });
            tableList.Add(new Table { Id = 22, Number = "tableNum 5", State = "open", Capacity = 3, RestaurantId = 2, PartySize = 0 }); //3
            tableList.Add(new Table { Id = 25, Number = "tableNum 6", State = "open", Capacity = 3, RestaurantId = 2, PartySize = 0 });
            repoMock.Setup(x => x.GetTablesByRestaurantId(restId)).ReturnsAsync(tableList);

            // Do test
            await WaitManagerMock.CheckWaitList(restId);

            // Assert
            repoMock.Verify(x => x.SaveWaits(It.IsAny<List<Wait>>()), Times.Never());
            repoMock.Verify(x => x.SaveTables(It.IsAny<List<Table>>()), Times.Never());
        }

        [TestMethod]
        public async Task CheckWaitList_noneWaiting() // All parties are "not waiting"
        {
            // Setup
            int restId = 2;
            List<Wait> waitList = new List<Wait>();
            waitList.Add(new Wait { Name = "not Waiting Person 3", Id = 21, RestaurantId = 2, State = "not waiting", PartySize = 1 }); //3
            waitList.Add(new Wait { Name = "not Waiting Person 4", Id = 22, RestaurantId = 2, State = "not waiting", PartySize = 1 });
            repoMock.Setup(x => x.GetWaitListByRestaurantId(restId)).ReturnsAsync(waitList);

            List<Table> tableList = new List<Table>();
            tableList.Add(new Table { Id = 13, Number = "tableNum 1", State = "seated", Capacity = 8, RestaurantId = 2, PartySize = 5 });
            tableList.Add(new Table { Id = 15, Number = "tableNum 2", State = "seated", Capacity = 8, RestaurantId = 2, PartySize = 3 });
            tableList.Add(new Table { Id = 16, Number = "tableNum 3", State = "open", Capacity = 8, RestaurantId = 2, PartySize = 0 }); //1
            tableList.Add(new Table { Id = 21, Number = "tableNum 4", State = "open", Capacity = 8, RestaurantId = 2, PartySize = 0 });
            tableList.Add(new Table { Id = 22, Number = "tableNum 5", State = "open", Capacity = 8, RestaurantId = 2, PartySize = 0 }); //3
            tableList.Add(new Table { Id = 25, Number = "tableNum 6", State = "open", Capacity = 8, RestaurantId = 2, PartySize = 0 });
            repoMock.Setup(x => x.GetTablesByRestaurantId(restId)).ReturnsAsync(tableList);

            // Do test
            await WaitManagerMock.CheckWaitList(restId);

            // Assert
            repoMock.Verify(x => x.SaveWaits(It.IsAny<List<Wait>>()), Times.Never());
            repoMock.Verify(x => x.SaveTables(It.IsAny<List<Table>>()), Times.Never());
        }

        [TestMethod]
        public async Task GetByWaitCode_Tests()
        {
            Wait w0 = new Wait { Name = "Wait Person 0", Id = 13, Created = new DateTime(1990, 12, 9) };
            Wait w1 = new Wait { Name = "Wait Person 1", Id = 15, Created = new DateTime(1990, 12, 9) };
            Wait w2 = new Wait { Name = "Wait Person 2", Id = 23, Created = new DateTime(1991, 9, 15) };
            Wait w3 = new Wait { Name = "Wait Person 3", Id = 41, Created = new DateTime(1992, 1, 19) };
            repoMock.Setup(x => x.GetWaitByCode(13)).ReturnsAsync(w0);
            repoMock.Setup(x => x.GetWaitByCode(15)).ReturnsAsync(w1);
            repoMock.Setup(x => x.GetWaitByCode(23)).ReturnsAsync(w2);

            // Get a desired Wait object
            Wait result = await WaitManagerMock.GetByWaitCode(13);
            
            Assert.AreSame(w0, result);
            Assert.AreEqual(w0, result);
            Assert.AreEqual(w0.Name, result.Name);
            Assert.AreEqual(w0.Id, result.Id);
            Assert.AreEqual("Wait Person 0", result.Name); // expected, actual
            Assert.AreEqual(13, result.Id);
            Assert.AreEqual(w0.Created, result.Created);

            Assert.AreNotEqual("some name", result.Name);
            Assert.AreNotEqual("WAIT PERSON 0", result.Name);
            Assert.AreNotEqual(-13, result.Id);
            Assert.AreNotEqual("13", result.Id);
            Assert.AreNotEqual(0, result.Id);
            Assert.AreNotEqual(w2.Id, result.Id);

            // Get (and expect) a null
            Wait result2 = await WaitManagerMock.GetByWaitCode(53);
            Wait result22 = await WaitManagerMock.GetByWaitCode(123);
            Assert.IsNull(result2);
            Assert.IsNull(result22);

            // Get another Wait object
            Wait result3 = await WaitManagerMock.GetByWaitCode(15);
            Assert.AreNotEqual(result, result3);
            Assert.AreEqual(15, result3.Id);
            Assert.AreEqual("Wait Person 1", result3.Name);
            Assert.AreNotEqual("Wait Person 0", result3.Name);
        }
    }
}
