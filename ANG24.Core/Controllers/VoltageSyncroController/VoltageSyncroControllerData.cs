using ANG24.Core.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANG24.Core.Controllers.VoltageSyncroController
{
    public class VoltageSyncroControllerData : ControllerData
    {
        public VoltageSyncroControllerData(string message) : base()
        {
            var receivedData = message.Trim('\r', '\n').Trim();

            Message = receivedData;
        }
    }
}
