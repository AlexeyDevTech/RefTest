using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ref
{
    public class RefLogic
    {
        public RefLogic()
        {
            
        }
    }




    public class ReflectController : Controller
    {
        public ReflectController() : base()
        {

        }


    }

    public interface IReflectController
    {
        bool SetChannel(int channel);
        bool SetMode(int mode);



    }

    public class ReflectChannelSettings
    {
        
    }


}

