using Ref.Controllers.ReflectController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ref.Interfaces
{
    public interface IReflectController
    {
        bool SetChannel(int channel);
        bool SetMode(int mode);
        bool SetImpulse(int impulse);
        bool SetAmplitude(int vol);
        bool SetResistance(int resistance);
        bool SetDelay(int delay);
        bool SetPulse(int pulse);
        bool GetState();
        event Action<ReflectData> OnDataRecieved;
    }
}
