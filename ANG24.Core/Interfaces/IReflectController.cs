using ANG24.Core.Controllers.ReflectController;
using ANG24.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANG24.Core.Interfaces
{
    public interface IReflectController : IController
    {
        bool Start();
        bool Stop();
        bool SetChannel(Channels channel);
        bool SetMode(Mode mode);
        bool SetImpulse(int impulse, out int value);
        bool SetImpulse(int impulse);
        bool SetAmplitude(Amplitude vol);
        bool SetResistance(Resistance resistance);
        bool SetDelay(int delay);
        bool SetPulse(int pulse);
        bool GetState();
        event Action<ReflectData> OnDataRecieved;
    }
}
