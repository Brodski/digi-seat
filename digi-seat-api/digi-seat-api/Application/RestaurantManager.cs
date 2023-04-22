using DigiSeatApi.Interfaces;
using DigiSeatApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DigiSeatApi.Application
{
    public class RestaurantManager : IRestaurantManager
    {
        private readonly IDigiSeatRepo _repo;
        private readonly IHttpContextAccessor _httpContext;
        private const string API_KEY_HEADER = "X-ApiKey";

        public RestaurantManager(IDigiSeatRepo digiSeatRepo, IHttpContextAccessor httpContext)
        {
            _repo = digiSeatRepo;
            _httpContext = httpContext;
        }

        public async Task<Restaurant> GetRestaurantUsingHttpContext()
        {
            //Yo, could we rewrite the "var header = httpcontext...." ? Could we get the headers w/o FirstOrDefault() method? Annoymous fucntions cant be used in MOQ, something with Static methods.
            //Perhaps do this?? https://msdn.microsoft.com/en-us/library/system.web.httprequest.headers(v=vs.110).aspx 
            // cant mock: https://stackoverflow.com/questions/11470313/how-to-mock-objectresult-firstordefault-using-moq
            // cant mock: https://stackoverflow.com/questions/34271466/moq-mocking-linq-when-firstordefault
            // cant mock: https://stackoverflow.com/questions/6645645/moq-testing-linq-where-queries
            Restaurant restaurant = null;
            var header = _httpContext.HttpContext.Request.Headers.FirstOrDefault(x => x.Key == API_KEY_HEADER);
            if (!string.IsNullOrEmpty(header.Value))
            {
                restaurant = await _repo.GetRestaurantFromKey(header.Value);
            }

            var workingStaff = restaurant.Staff.Where(x => x.State == "on").ToList();
            var staffCount = workingStaff.Count();
            var section = await _repo.GetSectionByStaffCount(restaurant.Id, staffCount);

            if (section != null)
            {
                //TODO: get the section tables and attach them to the staff in the list
                var sectionTables = await _repo.GetSectionTablesBySectionId(section.Id);
                if (sectionTables == null || !sectionTables.Any())
                {
                    return restaurant;
                }

                for (var i = 0; i < staffCount; i++)
                {
                    workingStaff[i].Tables = new List<int>(
                        sectionTables
                        .Where(x => x.StaffIndex == i)
                        .Select(x => x.TableId)
                        );
                }
            }

            return restaurant;
        }

        public async Task CreateStaff(Staff staff)
        {
            if (string.IsNullOrEmpty(staff.Name))
            {
                throw new Exception("A name must be provided to create staff");
            }
            var restaurant = await GetRestaurantUsingHttpContext();
            if (restaurant != null)
            {
                staff.RestaurantId = restaurant.Id;
            }
            staff.State = "off";
            staff.DateCreated = DateTime.UtcNow;
            await _repo.CreateStaff(staff);
        }

        public async Task UpdateStaff(Staff staff)
        {
            if (staff.Id == 0 || staff.RestaurantId == 0)
            {
                throw new ArgumentException("Id and RestaurantId are required for UpdateStaff");
            }

            await _repo.UpdateStaff(staff);
        }

        public async Task DeleteStaff(int id)
        {
            await _repo.DeleteStaff(id);
        }

        public async Task SaveSections(List<Staff> staffList)
        {
            var clockedIn = staffList.Where(x => x.State == "on").ToList();
            var count = clockedIn.Count;
            var restaurant = await GetRestaurantUsingHttpContext();
            var section = await _repo.GetSectionByStaffCount(restaurant.Id, count);

            //Create a new section because this is a new configuration
            if (section == null)
            {
                var newSection = new Section
                {
                    DateCreated = DateTime.UtcNow,
                    RestaurantId = restaurant.Id,
                    StaffCount = count
                };

                var id = await _repo.CreateSection(newSection);
                newSection.Id = id;
                section = newSection;
            }

            //The section is already there, but we need to remove the sectiontables that are already there to insert the new ones
            else
            {
                await _repo.DeleteSectionTablesBySection(section.Id);
            }

            var sectionTablesToInsert = new List<SectionTable>();

            foreach (var staff in clockedIn)
            {
                var staffIndex = clockedIn.IndexOf(staff);

                if (staff.Tables != null)
                {
                    foreach (var table in staff.Tables)
                    {
                        sectionTablesToInsert.Add(new SectionTable
                        {
                            SectionId = section.Id,
                            StaffIndex = staffIndex,
                            TableId = table
                        });
                    }
                }
            }

            if (sectionTablesToInsert.Any())
            {
                await _repo.InsertSectionTables(sectionTablesToInsert);
            }
        }
    }
}
