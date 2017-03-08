using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4Stateless.Tests.Models
{
    public class Smartphone
    {
        public int BatteryLevel { get; set; }

        public bool SimInserted { get; set; }

        public bool Locked { get; set; }

        public SmartphoneState State { get; set; } = SmartphoneState.Off;
    }
}
