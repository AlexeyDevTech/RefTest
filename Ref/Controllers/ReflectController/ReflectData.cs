using Ref.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ref.Controllers.ReflectController
{
    public class ReflectData : ControllerData
    {



        public ReflectData(string message) : base()
        {
            var receivedData = message.Trim('\r', '\n').Trim();

            Message = receivedData;
        }
    }
}
