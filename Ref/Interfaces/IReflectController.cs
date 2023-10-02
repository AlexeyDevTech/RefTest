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
        void GetState();

    }
}
