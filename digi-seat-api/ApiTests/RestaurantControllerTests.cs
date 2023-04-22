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
    public class RestaurantControllerTests
    {
        Mock<IRestaurantManager> RestManagerMock;
        RestaurantController RestControllerMock;

        [TestInitialize]
        public void TestInitialize()
        {
            RestManagerMock = new Mock<IRestaurantManager>();
            RestControllerMock = new RestaurantController(RestManagerMock.Object);
        }

        // Helpfull; using  bryans hogan's blog https://nodogmablog.bryanhogan.net/2017/09/unit-testing-net-core-2-web-api/
        [TestMethod]
        public async Task Get_Test()
        {   
            Restaurant r1 = new Restaurant { Name = "Restaurant name", Id = 2, ApiKey = "apikey1" };
            RestManagerMock.Setup(x => x.GetRestaurantUsingHttpContext()).ReturnsAsync(r1);

            var theOk = await RestControllerMock.Get();
            OkObjectResult okResult = (OkObjectResult) theOk;
            Restaurant myrest = (Restaurant) okResult.Value;

            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual("Restaurant name", myrest.Name);
            Assert.AreEqual(2, myrest.Id );
            Assert.AreEqual("apikey1", myrest.ApiKey);
        }

        [TestMethod]
        public async Task CreateStaff_Test()
        {
            Staff staff = new Staff { Name = "staff name", Id = 3, State = "some state" };
            OkResult okResult = (OkResult)await RestControllerMock.CreateStaff(staff);

            RestManagerMock.Verify(x => x.CreateStaff(staff), Times.Exactly(1));
            Assert.AreEqual(200, okResult.StatusCode);
        }


        [TestMethod]
        public async Task UpdateStaff_Test()
        {
            Staff staff = new Staff { Name = "staff name", Id = 3, State = "some state" };
            OkResult okResult = (OkResult) await RestControllerMock.UpdateStaff(1, staff);

            RestManagerMock.Verify(x => x.UpdateStaff(staff), Times.Exactly(1));
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task DeleteStaff_Test()
        {
            OkResult okResult = (OkResult) await RestControllerMock.DeleteStaff(1);

            RestManagerMock.Verify(x => x.DeleteStaff(1), Times.Exactly(1));
            Assert.AreEqual(200, okResult.StatusCode);
        }
    }
    
}
