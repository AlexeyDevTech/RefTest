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
        }

        private void OnData(ControllerData data)
        {
            var d = new MEAControllerData(data.Message);
            Console.WriteLine(d.Message);
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
    }

}
