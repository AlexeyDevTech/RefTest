using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Ref.BaseClasses;
using Ref.Interfaces;

namespace Ref.Controllers.ReflectController
{
    public class RefLogic
    {
        public RefLogic()
        {

        }
    }




    public class ReflectController120 : Controller
    {
        public ReflectController120() : base()
        {

        }


    }
    public class ReflectController90 : Controller, IReflectController
    {


        public void GetState()
        {
        }

        public bool SetAmplitude(int vol)
        {
            throw new NotImplementedException();
        }

        public bool SetChannel(int channel)
        {
            throw new NotImplementedException();
        }

        public bool SetImpulse(int impulse)
        {
            throw new NotImplementedException();
        }

        public bool SetMode(int mode)
        {
            throw new NotImplementedException();
        }
    }

    public class ReflectChannelSettings
    {

    }


}

