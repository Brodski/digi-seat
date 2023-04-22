using DigiSeatApi.Interfaces;
using DigiSeatApi.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DigiSeatApi.Application
{
    public class TableManager : ITableManager
    {
        private readonly IDigiSeatRepo _repo;
        private readonly IWaitManager _waitManager;
        private readonly IRestaurantManager _restaurantManager;

        public TableManager(IDigiSeatRepo repo, IWaitManager waitManager, IRestaurantManager restaurantManager)
        {
            _repo = repo;
            _waitManager = waitManager;
            _restaurantManager = restaurantManager;
        }

        public async Task<CheckTableResponse> GetBestAvailable(int partySize, string type = "", List<int> exclusionList = null, List<int> specificList = null)
        {
            var response = new CheckTableResponse()
            {
                PartySize = partySize
            };

            var restaurant = await _restaurantManager.GetRestaurantUsingHttpContext();
            var allTables = await _repo.GetTablesByRestaurantId(restaurant.Id);
            /////// This code might have been a waste of my time. Maybe
            if (specificList != null && specificList.Any())
            {
                List<Table> auxTables = new List<Table>();
                foreach (var tableId in specificList) //Cant find LINQ way to get interesection for this situations
                {
                    Table table = allTables.First(o => o.Id == tableId);
                    if (table != null)
                    {
                        auxTables.Add(table);
                    }

                }
                allTables = auxTables;
            }
            ///////

            var openTables = allTables.Where(x => x.State == "open" && x.Capacity >= partySize && !string.IsNullOrEmpty(x.LightAddress)).OrderBy(x => x.SeatedTime).ToList();

            if (!string.IsNullOrEmpty(type))
            {
                openTables.RemoveAll(x => x.TableType != type);
            }

            if (exclusionList != null && exclusionList.Any())
            {
                openTables.RemoveAll(x => exclusionList.Contains(x.Id));
            }

            if (!openTables.Any())
            {
                List<Table> possibleButOccupiedTables;
                if (!string.IsNullOrEmpty(type))
                {
                    possibleButOccupiedTables = allTables.Where(x => x.Capacity >= partySize && x.State == "seated" && x.TableType == type).ToList();
                }
                else
                {
                    possibleButOccupiedTables = allTables.Where(x => x.Capacity >= partySize && x.State == "seated").ToList();
                }

                if (possibleButOccupiedTables.Count != 0)
                { 
                    response.MinutesWait = GetWaitMinutes(possibleButOccupiedTables, partySize);
                    response.Status = "wait";
                }
                return response;
            }

            // var bestTable = openTables.OrderBy(x => x.SeatedTime).ToList()[0]; //OrderBy(seatedTime) might make no sense, since all Open tables shouldnt have a seated time
            var bestTable = openTables.ToList()[0];
            bestTable.PartySize = partySize;
            bestTable.State = "holding";

            //Hold the best table for the user
            await _repo.SaveTables(new List<Table> { bestTable });
            response.BestTable = bestTable;
            response.OtherTables = openTables.OrderBy(x => x.SeatedTime).Skip(1).ToList();  //OrderBy(seatedTime), no sense
            response.Status = "success";
            return response;
        }

        public async Task<SitResponse> SitTable(int tableId, int partySize)
        {
            var table = await _repo.GetTable(tableId);
            table.State = "seated";
            table.PartySize = partySize;
            table.SeatedTime = DateTime.UtcNow;

            var list = new List<Table> { table };

            //Check if this was for a wait
            var wait = await _repo.GetWaitByTableId(table.Id);
            if(wait != null)
            {
                //TODO: analytics
                if(wait.State == "notified")
                await _repo.DeleteWait(wait.Id);
            }

            await _repo.SaveTables(list);

            var response = new SitResponse
            {
                LightColor = GetLightColor(table),
                SectionLocation = GetSectionLocation(table)
            };

            return response;
        }

        public async Task<List<Table>> GetAllTables()
        {
            var restaurant = await _restaurantManager.GetRestaurantUsingHttpContext();
            var tables = await _repo.GetTablesByRestaurantId(restaurant.Id);
            return tables;
        }

        public async Task SaveTables(List<Table> tables)
        {
            await _repo.SaveTables(tables);
        }

        public async Task OpenTable(int id)
        {
            var table = await _repo.GetTable(id);
            table.State = "open";
            table.PartySize = 0;
            table.SeatedTime = null;

            var list = new List<Table> { table };
            await _repo.SaveTables(list);
            await _waitManager.CheckWaitList(table.RestaurantId);
        }

        public async Task CreateTable(Table table)
        {
            var restaurant = await _restaurantManager.GetRestaurantUsingHttpContext();
            table.RestaurantId = restaurant.Id;
            await _repo.InsertTable(table);
        }

        // For a set of tables, this method returns the expected estimated wait time.
        private int GetWaitMinutes(List<Table> tables, int partySize)
        {
            int waitTime = 0;
            var bigEnoughTables = tables.OrderBy(x => x.SeatedTime).ToList();
            var AverageSeatDuration = 15; //Need to get a real AverageSeatDuration.
            var waitT = DateTime.Now - bigEnoughTables[0].SeatedTime.Value;
            waitTime = AverageSeatDuration - (int) waitT.TotalMinutes;
            
            return waitTime;
        }

        private string GetSectionLocation(Table table)
        {
            return "to your right, all the way to the back.";
        }

        private string GetLightColor(Table table)
        {
            return Enum.GetName(typeof(Color), int.Parse(table.Number.Substring(table.Number.Length - 1, 1)));
        }

        internal enum Color
        {
            Green = 1,
            Red = 2,
            Yellow = 3,
            Pink = 4,
            Teal = 5,
            Orange = 6,
            Blue = 7,
            Purple = 8,
            LightBlue = 9,
            LightGreen = 0
        }
    }
}
