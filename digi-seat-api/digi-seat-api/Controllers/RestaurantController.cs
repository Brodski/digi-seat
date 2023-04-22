using DigiSeatApi.Composition;
using DigiSeatApi.Interfaces;
using DigiSeatApi.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigiSeatApi.Controllers
{
    [AuthorizationFilter]
    [Route("api/[controller]")]
    public class RestaurantController : Controller
    {
        private readonly IRestaurantManager _restaurantManager;

        public RestaurantController(IRestaurantManager restaurantManager)
        {
            _restaurantManager = restaurantManager;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _restaurantManager.GetRestaurantUsingHttpContext();
            return Ok(result);
        }

        [HttpPost]
        [Route("staff")]
        public async Task<IActionResult> CreateStaff([FromBody]Staff staff)
        {
            await _restaurantManager.CreateStaff(staff);
            return Ok();
        }

        [HttpPut]
        [Route("staff/{id}")]
        public async Task<IActionResult> UpdateStaff(int id, [FromBody] Staff staff) //I think we dont need "int id" in paramater
        {
            await _restaurantManager.UpdateStaff(staff);
            return Ok();
        }

        [HttpDelete("staff/{id}")]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            await _restaurantManager.DeleteStaff(id);
            return Ok();
        }

        [HttpPost]
        [Route("sections")]
        public async Task<IActionResult> SaveSections([FromBody] List<Staff> staffList)
        {
            await _restaurantManager.SaveSections(staffList);
            return Ok();
        }
    }
}
