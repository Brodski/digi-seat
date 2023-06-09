using DigiSeatApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigiSeatApi.Composition
{
    public class GlobalExceptionFilter : IExceptionFilter
  {
    private readonly ILogger _logger;

    public GlobalExceptionFilter(ILoggerFactory logger)
    {
      if (logger == null)
      {
        throw new ArgumentNullException(nameof(logger));
      }

      this._logger = logger.CreateLogger("Global Exception Filter");
    }

    public void OnException(ExceptionContext context)
    {
      var response = new ErrorResponse()
      {
        Message = context.Exception.Message,
        StackTrace = context.Exception.StackTrace
      };

      context.Result = new ObjectResult(response)
      {
        StatusCode = 500,
        DeclaredType = typeof(ErrorResponse)
      };

      this._logger.LogError(context.Exception, "Unhandled Exception Occurred");
    }
  }
}
