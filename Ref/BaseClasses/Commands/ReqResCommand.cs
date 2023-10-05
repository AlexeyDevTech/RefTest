using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ref.BaseClasses.Commands
{
    public class ReqResCommand : CommandBase
    {
        public string Response { get; set; } = string.Empty;
        public int Timeout { get; set; }
        public ReqResCommand(string Command) : base(Command)
        {

        }


    }
    public class ReqResFailCallbackCommand : CommandBase
    {
        public string Response { get; set; } = string.Empty;
        public string FailResponse { get; set; } = string.Empty;


        public int Timeout { get; set; }
        public ReqResFailCallbackCommand(string Command) : base(Command)
        {
        }
    }

}
