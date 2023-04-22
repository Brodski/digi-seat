using DigiSeatApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigiSeatApi.Interfaces
{
    public interface ITableManager
    {
        Task<CheckTableResponse> GetBestAvailable(int partySize, string type = "", List<int> exclusionList = null, List<int> specificList = null);
        Task<SitResponse> SitTable(int tableId, int partySize);
        Task<List<Table>> GetAllTables();
        Task SaveTables(List<Table> tables);
        Task OpenTable(int id);
        Task CreateTable(Table table);
    }
}
