using DigiSeatApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigiSeatApi.Interfaces
{
    public interface IRestaurantManager
    {
        Task<Restaurant> GetRestaurantUsingHttpContext();
        Task CreateStaff(Staff staff);
        Task UpdateStaff(Staff staff);
        Task DeleteStaff(int id);
        Task SaveSections(List<Staff> staffList);
    }
}
