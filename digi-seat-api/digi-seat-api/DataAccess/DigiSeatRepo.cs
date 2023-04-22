using Dapper;
using DigiSeatApi.Interfaces;
using DigiSeatApi.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DigiSeatApi.DataAccess
{
    public class DigiSeatRepo : IDigiSeatRepo
    {
        private readonly string _connectionString;
        private readonly Random _random;
        private readonly IConfiguration _configuration;

        public DigiSeatRepo(IConfiguration configuration)
        {
            _connectionString = @"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog = digi; Integrated Security = True; Connect Timeout = 30; Encrypt = False; TrustServerCertificate = True; ApplicationIntent = ReadWrite; MultiSubnetFailover = False";
            //Desktop
            //_connectionString = @"Data Source=(localdb)\ProjectsV13;Initial Catalog=DigiSeat;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            //Laptop
            //_connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=digiseat;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            //Luke desktop
            //_connectionString = @"Data Source=(localdb)\ProjectsV13;Initial Catalog=Digiseat;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            //Azure
            //_connectionString = @"Server=tcp:digiseat.database.windows.net,1433;Initial Catalog=digiseat;Persist Security Info=False;User ID=aorlowski;Password=Digiseat1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            _configuration = configuration;
            _connectionString = _configuration["ConnectionString"]; 
            _random = new Random();
        }

        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_connectionString);
            }
        }

        public int GetCode()
        {
            return _random.Next(1000, 9999);
        }

        public async Task<IEnumerable<Restaurant>> GetRestaurants()
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return await dbConnection.QueryAsync<Restaurant>("select * from restaurants");
            }
        }

        public async Task<Table> GetTable(int tableId)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var result = await dbConnection.QueryAsync<Table>($"select * from tables where id = {tableId}");
                return result.FirstOrDefault();
            }
        }

        public async Task SaveTables(List<Table> tables)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();

                foreach (var table in tables)
                {
                    var address = table.LightAddress;
                    var insertQuery = $@"update Tables set " +
                        $"Number = {table.Number}, " +
                        $"[State] = '{table.State}', " +
                        $"TableType = '{table.TableType}', " +
                        $"Capacity = {table.Capacity}, " +
                        $"RestaurantId = {table.RestaurantId}, " +
                        @"LightAddress = @address, " +
                        $"XCoordinate = {table.XCoordinate}, " +
                        $"YCoordinate = {table.YCoordinate}, " +
                        $"SeatedTime = '{table.SeatedTime}', " +
                        $"Shape = '{table.Shape}', " +
    //                  $"assignedServerTime = '{table.assignedServerTime}', " +
    //                  $"ServerId = {table.ServerId}, " +
                        $"PartySize = {table.PartySize} " +
                        $"where Id = {table.Id}";
                    await dbConnection.ExecuteAsync(insertQuery, new { address });
                }
            }
        }

        public async Task<List<Table>> GetTablesByRestaurantId(int restaurantId)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var result = await dbConnection.QueryAsync<Table>($"select * from tables where restaurantid = {restaurantId}");
                return result.ToList();
            }
        }
        public async Task InsertTable(Table table)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var address = table.LightAddress;

                //TODO: Allow for insert of coordinates?
                var insertQuery = $"insert into Tables " +
                    $"(Number, [State], TableType, Capacity, RestaurantId, Shape, LightAddress) values " +
                    $"({table.Number}, " +
                    $"'{table.State}', " +
                    $"'{table.TableType}', " +
                    $"{table.Capacity}, " +
                    $"{table.RestaurantId}, " +
                    $"'{table.Shape}', " +
                    $"@address)";
                await dbConnection.ExecuteAsync(insertQuery, new { address });
            }
        }

        public async Task<Wait> InsertWait(Wait wait)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();

                //TODO: Allow for insert of coordinates?
                var insertQuery = $"insert into Waits " +
                    $"(Created, PartySize, Name, Phone, EstimatedWait, RestaurantId, Code, [State]) values " +
                    $"('{wait.Created}', " +
                    $"{wait.PartySize}, " +
                    $"'{wait.Name}', " +
                    $"'{wait.Phone}', " +
                    $"{wait.EstimatedWait}, " +
                    $"{wait.RestaurantId}," +
                    $"{wait.Code}," +
                    $"'{wait.State}')";
                int id = await dbConnection.ExecuteAsync(insertQuery);
                var results = await dbConnection.QueryAsync<Wait>($"select * from Waits where id = {id}");
                return results.FirstOrDefault();
            }
        }

        public async Task SaveWaits(List<Wait> waits)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();

                foreach (var wait in waits)
                {
                    var insertQuery = $"update Waits set " +
                        $"NotifiedTime = '{wait.NotifiedTime}', " +
                        $"[State] = '{wait.State}', " +
                        $"[TableId] = {wait.TableId}, " +
                        $"PartySize = {wait.PartySize} " +
                        $"where Id = {wait.Id}";
                    await dbConnection.ExecuteAsync(insertQuery);
                }
            }
        }

        public async Task<List<Wait>> GetWaitListByRestaurantId(int restaurantId)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var result = await dbConnection.QueryAsync<Wait>($"select * from waits where restaurantid = {restaurantId}");
                return result.ToList();
            }
        }

        public async Task<Wait> GetWaitByCode(int code)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var result = await dbConnection.QueryAsync<Wait>($"select * from waits where code = {code}");
                return result.FirstOrDefault();
            }
        }

        public async Task<Wait> GetWaitByTableId(int tableId)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var result = await dbConnection.QueryAsync<Wait>($"select * from waits where tableid = {tableId}");
                return result.FirstOrDefault();
            }
        }

        public async Task DeleteWait(int waitId)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var query = $"delete from waits where id = {waitId}";
                var result = await dbConnection.ExecuteAsync(query);
            }
        }

        public async Task<Restaurant> GetRestaurantFromKey(string key)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();

                //TODO: Query with a join instead of two queries
                var result = await dbConnection.QueryAsync<Restaurant>($"select * from restaurants where apikey = '{key}'");
                var restaurant = result.FirstOrDefault();
                var staff = await dbConnection.QueryAsync<Staff>($"select * from staff where restaurantid = '{restaurant.Id}'");
                restaurant.Staff = staff.Where(x => !x.Deleted).ToList();
                return restaurant;
            }
        }

        public async Task<Section> GetSectionByStaffCount(int restaurantId, int staffCount)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();

                var result = await dbConnection.QueryAsync<Section>(
                    $"select * from section " +
                    $"where restaurantid = @restaurantId " +
                    $"and staffcount = @staffCount",
                    new { restaurantId, staffCount }
                    );
                var section = result.FirstOrDefault();
                return section;
            }
        }

        public async Task<int> CreateSection(Section section)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();

                var query =
                    $"insert into section (restaurantId, staffCount, dateCreated) values (@restaurantId, @staffCount, @dateCreated)";
                int id = await dbConnection.ExecuteAsync(query, 
                    new {
                        restaurantId = section.RestaurantId,
                        staffCount = section.StaffCount,
                        dateCreated = section.DateCreated
                    });
                return id;
            }
        }

        public async Task InsertSectionTables(List<SectionTable> sectionTables)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                foreach (var sectionTable in sectionTables)
                {
                    var query =
                        $"insert into sectiontables (sectionId, staffIndex, tableId) values (@sectionId, @staffIndex, @tableId)";
                    int id = await dbConnection.ExecuteAsync(query,
                        new
                        {
                            sectionId = sectionTable.SectionId,
                            staffIndex = sectionTable.StaffIndex,
                            tableId = sectionTable.TableId
                        });
                }
            }
        }

        public async Task DeleteSectionTablesBySection(int sectionId)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var query = $"delete from sectiontables where sectionid = {sectionId}";
                var result = await dbConnection.ExecuteAsync(query);
            }
        }

        public async Task<List<SectionTable>> GetSectionTablesBySectionId(int sectionId)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();

                var result = await dbConnection.QueryAsync<SectionTable>(
                    $"select * from sectiontables " +
                    $"where sectionId = @sectionId",
                    new { sectionId}
                    );
                return result.AsList();
            }
        }

        public async Task CreateStaff(Staff staff)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();

                //TODO: Allow for insert of coordinates?
                var insertQuery = $"insert into Staff " +
                    $"(Name, State, DateCreated, RestaurantId) values " +
                    $"('{staff.Name}', " +
                    $"'{staff.State}', " +
                    $"'{staff.DateCreated}', " +
                    $"{staff.RestaurantId})";
                int id = await dbConnection.ExecuteAsync(insertQuery);
            }
        }

        public async Task UpdateStaff(Staff staff)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();

                var insertQuery = $"update Staff set " +
                    $"Name = '{staff.Name}', " +
                    $"Ranking = {staff.Ranking}, " +
                    $"ShiftStart = '{staff.ShiftStart}', " +
                    $"State = '{staff.State}' " +
                    $"where Id = {staff.Id}";
                await dbConnection.ExecuteAsync(insertQuery);
            }
        }

        public async Task DeleteStaff(int id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();

                var insertQuery = $"update Staff set " +
                    $"Deleted = 1 " +
                    $"where Id = {id}";
                await dbConnection.ExecuteAsync(insertQuery);
            }
        }
    }
}
