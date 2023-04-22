using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DigiSeatApi.Application;
using DigiSeatApi.Models;
using System;
using System.Threading.Tasks;
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
using DigiSeatApi.Controllers;

namespace ApiTests
{

    [TestClass]
    public class TablesControllerTests
    {
        Mock<ITableManager> tablesManagerMock;
        TablesController tablesControllerMock;

        [TestInitialize]
        public void TestInitialize()
        {
            tablesManagerMock = new Mock<ITableManager>();
            tablesControllerMock = new TablesController(tablesManagerMock.Object);
        }

        [TestMethod]
        public async Task Get_Test()
        {
            List<Table> tablelist = new List<Table>();
            tablelist.Add(new Table() { Id = 1, Number = "Table num", RestaurantId = 10 });
            tablelist.Add(new Table() { Id = 2, Number = "Table num2", RestaurantId = 10 });
            tablelist.Add(new Table() { Id = 3, Number = "Table num3", RestaurantId = 10 });
            tablesManagerMock.Setup(x => x.GetAllTables()).ReturnsAsync(tablelist);

            OkObjectResult okResult = (OkObjectResult) await tablesControllerMock.GetAll();
            List<Table> tList = (List<Table>)okResult.Value;

            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(1, tList[0].Id);
            Assert.AreEqual(2, tList[1].Id);
            Assert.AreEqual("Table num", tList[0].Number);
            Assert.AreEqual("Table num2", tList[1].Number);
        }
    }
}