using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DigiSeatApi.Interfaces;
using DigiSeatApi.Models;
using DigiSeatApi.Composition;

namespace DigiSeatApi.Controllers
{
  [AuthorizationFilter]
  [Route("api/[controller]")]
  public class TablesController : Controller
  {
    private readonly ITableManager _tableManager;

    public TablesController(ITableManager tableManager)
    {
      _tableManager = tableManager;
    }

    // POST api/values
    [HttpPut("open/{id}")]
    public async Task Open(int id)
    {
      await _tableManager.OpenTable(id);
    }

    [HttpPut("sit/{tableId}")]
    public async Task<IActionResult> Sit(int tableId, [FromQuery] int partySize)
    {
      var response = await _tableManager.SitTable(tableId, partySize);
      return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int partySize, string type = "", List<int> exclusionList = null, List<int> specificList = null)
    {
      var table = await _tableManager.GetBestAvailable(partySize, type, exclusionList, specificList);
      return Ok(table);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
      var tables = await _tableManager.GetAllTables();
      return Ok(tables);
    }

    [HttpPut("save")]
    public async Task<IActionResult> Save([FromBody] List<Table> tables)
    {
      await _tableManager.SaveTables(tables);
      return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Table table)
    {
      await _tableManager.CreateTable(table);
      return Ok();
    }
  }
}
