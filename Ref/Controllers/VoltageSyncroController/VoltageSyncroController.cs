using Ref.BaseClasses;
using Ref.BaseClasses.Commands;
using Ref.Controllers.MainController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ref.Controllers.VoltageSyncroController
{
    public class VoltageSyncroController : Controller
    {
        public VoltageSyncroController() : base()
        {
            OnDataReceivedAction = OnData;   
        }

        public void GetVoltage()
        {
            var command = new ReqResCommand("#GET_VOLTAGE", "RNSVOL");
            WriteCommand(command);
        }

        public void GetMode()
        {
            var command = new ReqResCommand("#GETMODE", "RNSMODE");
            WriteCommand(command);
        }

        public void GetDevice()
        {
            var command = new ReqResCommand("#LAB?", "Voltage");
            WriteCommand(command);
        }

        private void OnData(ControllerData data)
        {
            var d = new VoltageSyncroControllerData(data.Message);
            /*Console.WriteLine(d.Message);*/
        }

    }
}
