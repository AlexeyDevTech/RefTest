
using ANG24.Core.BaseClasses;
using ANG24.Core.BaseClasses.Commands;
using ANG24.Core.Enums;

namespace ANG24.Core.Interfaces
{
    public interface IController
    {
        ControllerState State { get; }
        ControllerSettings Settings { get; set; }
        IBaseSerialDevice ControllerDevice { get; }
        ChainState ChainState { get; }
        bool Start();
        bool Stop();
        Task<bool> WriteCommand(StandardCommand command);
        Task<bool> WriteCommand(ReqResCommand command);
        void SetChain(ChainState c_state);
        Task<bool> ExecuteChain(int Delay = 150);

        Action<ControllerData> OnDataReceivedAction { get; set; }
    }
}
