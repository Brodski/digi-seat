using DigiSeatApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigiSeatApi.Interfaces
{
    public interface IWaitManager
    {
        Task<Wait> CreateWait(Wait wait);
        Task CheckWaitList(int restaurantId);
        Task<Wait> GetByWaitCode(int code);
        Task<List<Wait>> GetAll();
        Task DeleteWaitPerson(int id);
    }

}
