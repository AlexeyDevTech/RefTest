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
        public event Action<MEAControllerData> OnDataRecieved;
        public MEAController() : base()
        {
            OnDataReceivedAction = OnData;
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
            OnDataRecieved?.Invoke(new MEAControllerData(data.Message));
        }
    }

    public interface IMEAController
    {
        bool PowerUp();
        void PowerDown();
        bool SetHVMAC();
        bool SetHVMDC();
        bool SetBurn();
        bool SetJoinBurn();
        bool SetHVBurn();
        bool SetReflect();
        event Action<MEAControllerData> OnDataRecieved;
    }

}
