using Ref.BaseClasses;
using Ref.BaseClasses.Commands;
using Ref.Controllers.ReflectController.Enums;
using Ref.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Ref.Controllers.ReflectController
{
    public class ReflectController120 : Controller, IReflectController
    {
        public event Action<ReflectData> OnDataRecieved;

        public ReflectController120() : base()
        {
            OnDataReceivedAction = OnData;
        }

        private void OnData(ControllerData data)
        {
            OnDataRecieved?.Invoke(new ReflectData(data.Message));
        }

        public bool GetState()
        {
            var command = new ReqResCommand("#get_state", "State");
            return WriteCommand(command).Result;
        }

        public bool SetAmplitude(int vol)
        {
            var command = new ReqResCommand($"{TypeCommand.V}{vol:D3}#", $"{TypeCommand.V}{vol:D3}_OK");
            return WriteCommand(command).Result;
        }

        public bool SetChannel(int channel)
        {
            var command = new ReqResCommand($"{TypeCommand.C}{channel:D3}#", $"{TypeCommand.C}{channel:D3}_OK");
            return WriteCommand(command).Result;
        }

        public bool SetDelay(int delay)
        {
            var command = new ReqResCommand($"{TypeCommand.D}{delay:D3}#", $"{TypeCommand.D}{delay:D3}_OK");
            return WriteCommand(command).Result;
        }

        public bool SetImpulse(int impulse)
        {
            var command = new ReqResCommand($"{TypeCommand.I}{impulse:D3}#", $"{TypeCommand.I}{impulse:D3}_OK");
            return WriteCommand(command).Result;
        }

        public bool SetMode(int mode)
        {
            var command = new ReqResCommand($"{TypeCommand.M}{mode:D3}#", $"{TypeCommand.M}{mode:D3}_");
            return WriteCommand(command).Result;
        }

        public bool SetPulse(int pulse)
        {
            var command = new ReqResCommand($"{TypeCommand.P}{pulse:D3}#", $"{TypeCommand.P}{pulse:D3}_OK");
            return WriteCommand(command).Result;
        }

        public bool SetResistance(int resistance)
        {
            var command = new ReqResCommand($"{TypeCommand.R}{resistance:D3}#", $"{TypeCommand.R}{resistance:D3}_OK");
            return WriteCommand(command).Result;
        }

    }
}
