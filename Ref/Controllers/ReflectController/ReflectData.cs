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
        //State: [mode],[channel],[R],[V],[I]
        public static string CurrentMode;
        public static string CurrentChannel;
        public static string R;
        public static string V;
        public static string I;


        public ReflectData(string message) : base()
        {
            var data = message.Trim('\r', '\n').Trim();

            if (data.Contains("State:"))
            {
                var values = data.Split(':')[1].Split(',');
                CurrentMode = values[0];
                CurrentChannel = values[1].Trim();
                R = values[2];
                V = values[3];
                I = values[4];



            }

            Message = data;
        }
    }
}
