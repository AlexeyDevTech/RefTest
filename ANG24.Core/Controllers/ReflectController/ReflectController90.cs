using ANG24.Core.BaseClasses;
using ANG24.Core.BaseClasses.Commands;
using ANG24.Core.Controllers.ReflectController.Enums;
using ANG24.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANG24.Core.Controllers.ReflectController
{
    public class ReflectController90 : Controller, IReflectController
    {
        public ReflectController90() : base()
        {
            Settings = new()
            {
                Request = "R120#",
                Response = "Command done",
                BaudRate = 115200
            };
            OnDataReceivedAction = OnData;
        }

        private void OnData(ControllerData data)
        {
            OnDataRecieved?.Invoke(new ReflectData(data.Message));
        }

        public event Action<ReflectData> OnDataRecieved;

        public bool GetState()
        {
            throw new NotImplementedException();
        }

        public bool SetAmplitude(Amplitude vol)
        {
            var command = new ReqResCommand($"{TypeCommand.V}{(int)vol:D3}#", "Command done");
            command.OnResponseRecieved += Command_OnResponseRecieved;
            return WriteCommand(command).Result;
        }

        private void Command_OnResponseRecieved(ReqResCommand command)
        {
            ReflectData.FillData(command.Command);

            command.OnResponseRecieved -= Command_OnResponseRecieved;
        }

        public bool SetChannel(Channels channel)
        {
            throw new NotImplementedException();
        }

        public bool SetDelay(int delay)
        {
            throw new NotImplementedException();
        }

        public bool SetImpulse(int impulse)
        {
            throw new NotImplementedException();
        }

        public bool SetMode(Mode mode)
        {
            throw new NotImplementedException();
        }

        public bool SetPulse(int pulse)
        {
            throw new NotImplementedException();
        }

        public bool SetResistance(Resistance resistance)
        {
            throw new NotImplementedException();
        }

        public bool SetImpulse(int impulse, out int value)
        {
            throw new NotImplementedException();
        }
    }
}
