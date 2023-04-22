using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DigiSeatApi.Models;
using DigiSeatApi.Interfaces;
using DigiSeatApi.Composition;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DigiSeatApi.Controllers
{
  [AuthorizationFilter]
  [Route("api/[controller]")]
  public class WaitController : Controller
  {
    private readonly IWaitManager _waitManager;

    public WaitController(IWaitManager waitManager)
    {
      _waitManager = waitManager;
    }

    [HttpGet("all")]
    public async Task<IActionResult> Get()
    {
      var waits = await _waitManager.GetAll();
      return Ok(waits);
    }

    [HttpGet]
    public async Task<IActionResult> GetByCode([FromQuery] int code)
    {
      var wait = await _waitManager.GetByWaitCode(code);
      return Ok(wait);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Wait wait)
    {
      var result = await _waitManager.CreateWait(wait);

      return Ok(result);
    }

    // PUT api/values/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody]string value)
    {
    }

    // DELETE api/values/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWait(int id)
    {
            await _waitManager.DeleteWaitPerson(id);
            return Ok();
    }
  }
}
