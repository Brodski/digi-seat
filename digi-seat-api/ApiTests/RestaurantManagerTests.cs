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

// how to make tests: https://docs.microsoft.com/en-us/dotnet/core/tutorials/testing-library-with-visual-studio?tabs=csharp
//
namespace ApiTests
{
    [TestClass]
    public class RestaurantManagerTests
    {

        Mock<IDigiSeatRepo> repoMock;
        Mock<IHttpContextAccessor> httpMock;
        RestaurantManager RestManagerMock;

        [TestInitialize]
        public void TestInitialize()
        {
            repoMock = new Mock<IDigiSeatRepo>();
            httpMock = new Mock<IHttpContextAccessor>();
            RestManagerMock = new RestaurantManager(repoMock.Object, httpMock.Object);
        }

        [TestMethod]
        public async Task DeleteStaff_Tests()
        {
            Staff staff = new Staff { Id = 13, Name = "Staff del guy", RestaurantId = 103 };
            await RestManagerMock.DeleteStaff(staff.Id);
            repoMock.Verify(x => x.DeleteStaff(staff.Id), Times.Exactly(1));

        }

        [TestMethod]
        public async Task UpdateStaff_validData()
        {
            Staff staff = new Staff { Id = 13, Name = "Staff guy", RestaurantId = 103 };
            await RestManagerMock.UpdateStaff(staff);
            repoMock.Verify(x => x.UpdateStaff(staff), Times.Exactly(1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task UpdateStaff_staffIDis0()
        {
            Staff staff = new Staff() { Id = 0, Name = "Staff guy", RestaurantId = 103 };
            await RestManagerMock.UpdateStaff(staff);
            repoMock.Verify(x => x.UpdateStaff(staff), Times.Exactly(0));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task UpdateStaff_restIDis0()
        {
            Staff s2 = new Staff() { Id = 42, Name = "Staff guy2", RestaurantId = 0 };
            await RestManagerMock.UpdateStaff(s2);
        }


        //[TestMethod]
        //public async Task CreateStaff_Test()
        //{
        //    Setup up
        //    Mock<IDigiSeatRepo> repoMock = new Mock<IDigiSeatRepo>();
        //    Mock<IHttpContextAccessor> httpMock = new Mock<IHttpContextAccessor>();
        //    RestaurantManager RestManagerMock = new RestaurantManager(repoMock.Object, httpMock.Object);


        //    String API_KEY_HEADERmock = "my api key header";
        //    var svValue = new StringValues("expected value");
        //    var myKvPair = new KeyValuePair<string, StringValues>("expected key", svValue);
        //    httpMock.Setup(x => x.HttpContext.Request.Headers.FirstOrDefault(x2 => x2.Key == API_KEY_HEADERmock)).Returns(myKvPair);
        //    httpMock.Setup(x => x.HttpContext.Request.Headers.FirstOrDefault(x2 => x2.Key(It.IsAny < Expression<KeyValuePair<string, StringValues>, bool> pr))



        //    Staff staff = new Staff();
        //    staff.Name = "Staff man 1";
        //    staff.RestaurantId = 137;
        //    staff.State = "off";
        //    staff.DateCreated = new DateTime(2015, 3, 17);

        //    Execute
        //   await RestManagerMock.CreateStaff(staff);

        //    Assert
        //    repoMock.Verify(x => x.CreateStaff(staff), Times.Exactly(1));

        // }
        

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task CreateStaffNullStaff_Test()
        {
            Staff staff = new Staff();
            staff.Name = null;
            await RestManagerMock.CreateStaff(staff);      
        }

        /*
        [TestMethod]
        public void GetRestaurantUsingHttpContext_Test()
        {
            //Setup
            IHttpContextAccessor _httpContext;
            HttpClient myClient = new HttpClient();
            string Api_test_header = "apikey-test";
            string Api_Key_Test = "My sweet api key, test";
            myClient.DefaultRequestHeaders.Add(Api_test_header, Api_Key_Test);

            Mock<DigiSeatRepo> mockRepo = new Mock<DigiSeatRepo>();

            IHttpWebRequest request = this.WebRequestfactory;
            HttpWebRequest  req = (HttpWebRequest)WebRequest.Create()


            public RestaurantManager(IDigiSeatRepo digiSeatRepo, IHttpContextAccessor httpContext)
            {
                _repo = digiSeatRepo;
                _httpContext = httpContext;
            }

            var thingyz = new RestaurantManager(mockRepo, htt);
            //Test
            Restaurant restaurant = null;
            //var header = _httpContext.HttpContext.Request.Headers.FirstOrDefault(x => x.Key == API_KEY_HEADER);
            var header = client.HttpContext.Request.Headers.FirstOrDefault(x => x.Key == API_KEY_HEADER);
            if (!string.IsNullOrEmpty(header.Value))
            {
                restaurant = await _repo.GetRestaurantFromKey(header.Value);
            }

            return restaurant;

        }
        */


    }
}
