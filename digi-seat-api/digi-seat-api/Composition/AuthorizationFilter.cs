using DigiSeatApi.DataAccess;
using DigiSeatApi.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigiSeatApi.Composition
{
    public class AuthorizationFilter : ActionFilterAttribute
    {
        private IRestaurantManager _restaurantManager;
        private IDigiSeatRepo _repo;

        public async override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _repo = context.HttpContext.RequestServices.GetService(typeof(IDigiSeatRepo)) as IDigiSeatRepo;
            _restaurantManager = context.HttpContext.RequestServices.GetService(typeof(IRestaurantManager)) as IRestaurantManager;

            var restaurant = _restaurantManager.GetRestaurantUsingHttpContext();

            if (restaurant == null)
            {
                context.HttpContext.Response.StatusCode = 401;
                context.Result = new JsonResult("Unauthorized");
            }
            else
            {
                await base.OnActionExecutionAsync(context, next);
            }
        }
    }
}
