using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Ref.BaseClasses;
using Ref.BaseClasses.Commands;

namespace Ref.Controllers.MainController
{
    public class MEAController : Controller
    {
        public MEAController() : base()
        {
            OnDataReceivedAction = OnData;
            ControllerDevice.SetReadMode(Interfaces.DeviceReadMode.Existing);
        }

        //example
        public void ReadTrial()
        {
            var command = new ReqResCommand("#ReadTrial", "Trial version");
            WriteCommand(command).Wait();
        }

        public void SpeedFast()
        {
            var command = new ReqResCommand("#SPEED_FAST", "Speed_fast");
            WriteCommand(command).Wait();
        }

        public void SetModuleBurn()
        {
            var command = new StandardCommand("#BURN:START,MANUAL;");
            WriteCommand(command);
        }

        private void OnData(ControllerData data)
        {
            var d = new MEAControllerData(data.Message);
            //Console.WriteLine(d.Message);
        }
    }

    public interface IMEAController
    {

    }

}
