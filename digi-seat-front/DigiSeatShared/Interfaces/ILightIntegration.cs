using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiSeatShared.Interfaces
{
    public interface ILightIntegration
    {
        Task TurnOn(string address, string color);
        Task TurnOff(string address);
    }
}
