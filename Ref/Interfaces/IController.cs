using Ref.BaseClasses;
using Ref.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        bool SetCommand(string command);
        void SetChain(ChainState c_state);
        bool ExecuteChain();

        Action<ControllerData> OnDataReceivedAction { get; set; }
    }
}
