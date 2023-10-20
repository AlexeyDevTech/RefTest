using ANG24.Core.BaseClasses.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANG24.Core.BaseClasses
{
    public class RS485Controller : Controller
    {
        public RS485Controller() : base()
        {
            
        }
        public override bool Start()
        {
            return base.Start();
        } 
        public override bool Stop()
        {
            return base.Stop();
        }
        public override Task<bool> WriteCommand(ReqResCommand command)
        {
            return base.WriteCommand(command);
        }
        public override Task<bool> WriteCommand(StandardCommand command)
        {
            return base.WriteCommand(command);

        }
    }
}
