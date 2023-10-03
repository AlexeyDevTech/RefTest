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
        
        public void GetMode()
        {
            var command = new StandartCommand(this, "#GET_VOLTAGE");
            SetCommand(command).ExecuteCommand();
        }

        private void OnData(ControllerData data)
        {
            var d = new VoltageSyncroControllerData(data.Message);
            Console.WriteLine(d.Message);
        }

    }
}
