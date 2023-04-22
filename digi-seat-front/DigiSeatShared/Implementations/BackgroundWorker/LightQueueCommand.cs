using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiSeatShared.Implementations.BackgroundWorker
{
    public class LightQueueCommand
    {
        public string Address { get; set; }
        public string Color { get; set; }
        public bool TurnOn { get; set; }
    }
}
