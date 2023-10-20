using ANG24.Core.BaseClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANG24.Core.Controllers.ReflectController
{
    public class ReflectData : ControllerData
    {
        //State: [mode],[channel],[R],[V],[I]

        /// <summary>
        /// Текущий мод рефлектометра
        /// </summary>
        public static string CurrentMode;
        /// <summary>
        /// Текущий канал рефлектометра
        /// </summary>
        public static int CurrentChannel;
        /// <summary>
        /// Текущее сопротивление рефлектометра
        /// </summary>
        public static string R;
        /// <summary>
        /// Текущая амплитуда рефлектометра
        /// </summary>
        public static string V;
        /// <summary>
        /// Текущий импульс рефлектометра
        /// </summary>
        public static string I;
        /// <summary>
        /// Текущая задержка (Delay) рефлектометра
        /// </summary>
        public static string D;

        public static void FillData(string data)
        {
            //parsing data on request
        }

        public ReflectData(string message) : base()
        {
            var data = message.Trim('\r', '\n').Trim();

            int newChannel = CurrentChannel;

            if (data.Contains("ArcSend"))
            {
                CurrentChannel = 4;
            }

            if (data.Contains("State:"))
            {
                var values = data.Split(':')[1].Split(',');
                CurrentMode = values[0];
                newChannel = Convert.ToInt16(values[1].Trim());
                R = values[2];
                V = values[3];
                I = values[4];
            }
            if (data.Contains("OK"))
            {
                var letter = data[0].ToString();
                var res = int.Parse(data.Split('_')[0].Substring(1)).ToString();
                if (letter == "M") CurrentMode = res;
                if (letter == "C") newChannel = int.Parse(res);
                if (letter == "R") R = res;
                if (letter == "V") V = res;
                if (letter == "I") I = res;
                if (letter == "D") D = res;
            }

            if (newChannel != CurrentChannel)
                CurrentChannel = newChannel;

            Message = data;

        }
    }
}
