using ANG24.Core.BaseClasses;
using ANG24.Core.BaseClasses.Commands;
using ANG24.Core.Controllers.ReflectController.Enums;
using ANG24.Core.Interfaces;

namespace ANG24.Core.Controllers.ReflectController
{
    public class ReflectController120 : Controller, IReflectController
    {
        public event Action<ReflectData> OnDataRecieved;

        public ReflectController120() : base()
        {
            Settings = new()
            {
                Request = "R120#",
                Response = "R120_OK",
                BaudRate = 115200
            };
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

        public bool SetAmplitude(Amplitude vol)
        {
            var command = new ReqResCommand($"{TypeCommand.V}{(int)vol:D3}#", $"{TypeCommand.V}{(int)vol:D3}_OK");
            return WriteCommand(command).Result;
        }

        public bool SetChannel(Channels channel)
        {
            var command = new ReqResCommand($"{TypeCommand.C}{(int)channel:D3}#", $"{TypeCommand.C}{(int)channel:D3}_OK");
            return WriteCommand(command).Result;
        }

        public bool SetDelay(int delay)
        {
            var command = new ReqResCommand($"{TypeCommand.D}{delay:D3}#", $"{TypeCommand.D}{delay:D3}_OK");
            return WriteCommand(command).Result;
        }

        public bool SetImpulse(int impulse, out int value)
        {
            var _impulse = (impulse - 14) / 14 < 1 ? 1 : (impulse - 14) / 14;

            value = impulse;

            var command = new ReqResCommand($"{TypeCommand.I}{_impulse:D3}#", $"{TypeCommand.I}{_impulse:D3}_OK");
            return WriteCommand(command).Result;
        }

        public bool SetImpulse(int impulse)
        {
            var _impulse = (impulse - 14) / 14 < 1 ? 1 : (impulse - 14) / 14;

            var command = new ReqResCommand($"{TypeCommand.I}{_impulse:D3}#", $"{TypeCommand.I}{_impulse:D3}_OK");
            return WriteCommand(command).Result;
        }

        public bool SetMode(Mode mode)
        {
            var command = new ReqResCommand($"{TypeCommand.M}{(int)mode:D3}#", $"{TypeCommand.M}{(int)mode:D3}_");
            return WriteCommand(command).Result;
        }

        public bool SetPulse(int pulse)
        {
            var command = new ReqResCommand($"{TypeCommand.P}{pulse:D3}#", $"{TypeCommand.P}{pulse:D3}_OK");
            return WriteCommand(command).Result;
        }

        public bool SetResistance(Resistance resistance)
        {
            var command = new ReqResCommand($"{TypeCommand.R}{(int)resistance:D3}#", $"{TypeCommand.R}{(int)resistance:D3}_OK");
            return WriteCommand(command).Result;
        }

    }
}
