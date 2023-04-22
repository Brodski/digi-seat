using DigiSeatApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigiSeatApi.Interfaces
{
    public interface IDigiSeatRepo
    {
        int GetCode();
        Task<IEnumerable<Restaurant>> GetRestaurants();

        //Tables
        Task<Table> GetTable(int tableId);
        Task SaveTables(List<Table> tables);
        Task<List<Table>> GetTablesByRestaurantId(int restaurantId);
        Task InsertTable(Table table);

        //Waits
        Task<Wait> InsertWait(Wait wait);
        Task<List<Wait>> GetWaitListByRestaurantId(int restaurantId);
        Task SaveWaits(List<Wait> waits);
        Task<Wait> GetWaitByCode(int code);
        Task<Wait> GetWaitByTableId(int tableId);
        Task DeleteWait(int waitId);

        //Restaurant
        Task<Restaurant> GetRestaurantFromKey(string key);
        Task CreateStaff(Staff staff);
        Task UpdateStaff(Staff staff);
        Task DeleteStaff(int id);
        Task<Section> GetSectionByStaffCount(int restaurantId, int staffCount);
        Task<int> CreateSection(Section section);
        Task InsertSectionTables(List<SectionTable> sectionTables);
        Task DeleteSectionTablesBySection(int sectionId);
        Task<List<SectionTable>> GetSectionTablesBySectionId(int sectionId);
    }
}
