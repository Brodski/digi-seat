using DigiSeatApi.Interfaces;
using DigiSeatApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace DigiSeatApi.Application
{
    public class WaitManager : IWaitManager
    {
        private readonly IDigiSeatRepo _repo;
        private readonly IRestaurantManager _restaurantManager;

        public WaitManager(IDigiSeatRepo repo, IRestaurantManager restaurantManager)
        {
            _repo = repo;
            _restaurantManager = restaurantManager;
        }

        public async Task<Wait> CreateWait(Wait wait)
        {
            var restaurant = await _restaurantManager.GetRestaurantUsingHttpContext();
            wait.RestaurantId = restaurant.Id;
            wait.Created = DateTime.Now;
            wait.Code = _repo.GetCode();
            wait.State = "waiting";

            var createdWait = await _repo.InsertWait(wait);
            return createdWait;
        }

        // TODO: improve intelligence. Could seat a party of 2 at a table with 10 capacity
        public async Task CheckWaitList(int restaurantId)
        {
            var allWaits = await _repo.GetWaitListByRestaurantId(restaurantId);
            var waitingWaits = allWaits.Where(x => x.State == "waiting");
            if (!waitingWaits.Any())
            {
                return;
            }

            var allTables = await _repo.GetTablesByRestaurantId(restaurantId);
            var openTables = allTables.Where(x => x.State == "open" );
            foreach (var wait in waitingWaits)
            {
                var table = openTables.FirstOrDefault(x => x.Capacity >= wait.PartySize);
                if (table != null)
                {
                    SendSms(wait);
                    wait.State = "notified";
                    wait.TableId = table.Id;
                    wait.NotifiedTime = DateTime.Now;

                    _repo.SaveWaits(new List<Wait> { wait });

                    table.State = "holding";

                    _repo.SaveTables(new List<Table> { table });
                }
            }
        }

        public async Task<Wait> GetByWaitCode(int code)
        {
            var wait = await _repo.GetWaitByCode(code);
            if (wait != null)
            {
                wait.Table = await _repo.GetTable(wait.TableId);
            }
            return wait;
        }

        static async Task SendSms(Wait wait)
        {
            // Your Account SID from twilio.com/console
            var accountSid = "AC10a480c76234816d141a5cdabf9c5405";
            // Your Auth Token from twilio.com/console
            var authToken = "dc239c35dff4e1b985b1ccb6dbafcdc4";

            TwilioClient.Init(accountSid, authToken);
            try
            {
                var message = await MessageResource.CreateAsync(
                    to: new PhoneNumber(wait.Phone),
                    from: new PhoneNumber("+17204086372"),
                    body: $"Your table is ready. Please enter {wait.Code} at our host stand to claim your table in the next 5 minutes.");
                Console.WriteLine(message.Sid);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public async Task<List<Wait>> GetAll()
        {
            var restaurant = await _restaurantManager.GetRestaurantUsingHttpContext();
            return await _repo.GetWaitListByRestaurantId(restaurant.Id);
        }

        public async Task DeleteWaitPerson(int id)
        {
            await _repo.DeleteWait(id);
        }

    }
}
