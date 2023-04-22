using DigiSeatShared.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http.Filters;
//using Windows.Web.Http;
//using Windows.Web.Http.Filters;

namespace DigiSeatShared
{
    public class Integration
    {
        private HttpClient _client = new HttpClient();

        private const string API_KEY_HEADER = "X-ApiKey";

        public Integration()
        {
            _client = this.GetClient();
        }
        public Integration(HttpClient c)
        {
            _client = c;
        }

        public HttpClient GetClient()
        {
        //    var RootFilter = new HttpBaseProtocolFilter();
      //      var rf = new HttpRequestCachePolicy();
    //        RootFilter.CacheControl.ReadBehavior = HttpCacheReadBehavior.NoCache;
  //          RootFilter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.NoCache;
//            _client = new HttpClient(RootFilter);
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add(API_KEY_HEADER, Settings.ApiKey);
            return _client;
        }
        
        public StringContent GetContentString(object obj)
        {
            return new StringContent(JsonConvert.SerializeObject(obj), UnicodeEncoding.UTF8, "application/json");
        }

        public async Task<CheckTableResponse> GetTable(int partySize, string type = "", List<int> exclusionList = null, List<int> specificList = null)
        {
            var data = await SendHttpGet(new Uri($"{Settings.ApiBaseUrl}/api/tables?partySize={partySize}&type={type}&exclusionList={exclusionList}&specificList={specificList}"));
            var result = JsonConvert.DeserializeObject<CheckTableResponse>(data);
            if (result.BestTable != null)
            {
                result.BestTable.PartySize = partySize;
            }
            return result;
        }

        public async Task<List<Wait>> GetAllWaits()
        {
            var data = await SendHttpGet(new Uri($"{Settings.ApiBaseUrl}/api/wait/all"));
            var result = JsonConvert.DeserializeObject<List<Wait>>(data);
            return result;
        }

        public async Task<bool> DeleteWaitPerson(int id)
        {
            var result = await SendHttpDelete(new Uri($"{Settings.ApiBaseUrl}/api/wait/{id}"));
            return result.IsSuccessStatusCode;
        }

        public async Task<Restaurant> GetRestaurant()
        {
            var data = await SendHttpGet(new Uri($"{Settings.ApiBaseUrl}/api/restaurant"));
            var restaurant = JsonConvert.DeserializeObject<Restaurant>(data);
            return restaurant;
        }

        public async Task<bool> CreateStaff(Staff staff)
        {
            var result = await SendHttpPost(new Uri($"{Settings.ApiBaseUrl}/api/restaurant/staff"), staff);
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateStaff(Staff staff)
        {
            staff.Brush = null;
            var response = await SendHttpPut(new Uri($"{Settings.ApiBaseUrl}/api/restaurant/staff/{staff.Id}"), staff);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteStaff(int id)
        {
            var result = await SendHttpDelete(new Uri($"{Settings.ApiBaseUrl}/api/restaurant/staff/{id}"));
            return result.IsSuccessStatusCode;
        }

        public async Task<Wait> GetWaitWithCode(string Code)
        {
            var data = await SendHttpGet(new Uri($"{Settings.ApiBaseUrl}/api/wait?code={Code}"));
            var wait = JsonConvert.DeserializeObject<Wait>(data);
            return wait;
        }
        
        public async Task<bool> PostWait(Wait wait)
        {
            var response = await SendHttpPost(new Uri($"{Settings.ApiBaseUrl}/api/wait"), wait);
            var status = response.IsSuccessStatusCode;
            var s2 = response.StatusCode; // should be 204
            return status;
        }

        public async Task<List<Table>> GetTables()
        {
            var data = await SendHttpGet(new Uri($"{Settings.ApiBaseUrl}/api/tables/all?restaurantId=1"));
            return JsonConvert.DeserializeObject<List<Table>>(data);
        }

        public async Task<bool> SaveAllTables(List<Table> alltables)
        {
            var response = await SendHttpPut(new Uri($"{Settings.ApiBaseUrl}/api/tables/save"), alltables);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> OpenTable(int tableId)
        {
            var result = await SendHttpPut(new Uri($"{Settings.ApiBaseUrl}/api/tables/open/{tableId}"));
            return result.IsSuccessStatusCode;
        }

        public async Task<SitResponse> SitTable(int id, int partySize)
        {
            var response = await SendHttpPut(new Uri($"{Settings.ApiBaseUrl}/api/tables/sit/{id}?partySize={partySize}"));
            var data = await response.Content.ReadAsStringAsync();
            var sitResponse = new SitResponse();
            sitResponse = JsonConvert.DeserializeObject<SitResponse>(data);
            return sitResponse;
        }

        public async Task<bool> SaveStaffSections(List<Staff> staff)
        {
            staff.ForEach(x => x.Brush = null);
            var response = await SendHttpPost(new Uri($"{Settings.ApiBaseUrl}/api/restaurant/sections"), staff);
            return response.IsSuccessStatusCode;
        }

        public async Task<HttpResponseMessage> SendHttpPut(Uri uri, Object myObj = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, uri);
            if (myObj != null)
            {
                request.Content = this.GetContentString(myObj);
            }
            var response = await _client.SendAsync(request);
            return response;
        }

        public async Task<String> SendHttpGet(Uri uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            var response = await _client.SendAsync(request);
            var data = await response.Content.ReadAsStringAsync();
            return data;
        }

        public async Task<HttpResponseMessage> SendHttpPost(Uri uri, Object myObj = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = this.GetContentString(myObj);
            var response = await _client.SendAsync(request);
            return response;
        }

        public async Task<HttpResponseMessage> SendHttpDelete(Uri uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, uri);
            var response = await _client.SendAsync(request);
            return response;
        }
    }
}
