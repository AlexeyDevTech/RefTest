using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ref.Controllers
{
    public class MOController : Controller
    {
        public MOController() : base()
        {
            OnDataReceivedAction = OnData;
        }

        private void OnData(ControllerData data)
        {
            var d = new MOControllerData(data.Message);
            Console.WriteLine(d.Message);
        }
    }

    class MOControllerData : ControllerData
    {
        public MOControllerData(string message) : base()
        {
            var receivedData = message.Trim('\r', '\n').Trim();

            Message = receivedData;
        }
    }

}
