﻿using Ref.BaseClasses;
using Ref.BaseClasses.Commands;
using Ref.Enums;

namespace Ref.Interfaces
{
    public interface IController
    {
        ControllerState State { get; }
        ControllerSettings Settings { get; set; }
        IBaseSerialDevice ControllerDevice { get; }
        ChainState ChainState { get; }
        bool Start();
        void Stop();
        Task<bool> WriteCommand(StandardCommand command);
        Task<bool> WriteCommand(ReqResCommand command);
        void SetChain(ChainState c_state);
        Task<bool> ExecuteChain();

        Action<ControllerData> OnDataReceivedAction { get; set; }
    }
}
