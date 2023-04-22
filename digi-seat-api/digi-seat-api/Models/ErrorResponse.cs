using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigiSeatApi.Models
{
  public class ErrorResponse
  {
    public string Message { get; set; }
    public string StackTrace { get; set; }
  }
}
