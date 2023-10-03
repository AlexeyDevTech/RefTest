using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ref.BaseClasses.Commands
{
    public class ReqResCommand : CommandBase
    {
        private string command;
        private string response;
        private int timeout = 0;
        private Controller reciever;
        public ReqResCommand(Controller Reciever, string Command, string Response)
        {
            reciever = Reciever;
            command = Command;
            response = Response;
        }
        public ReqResCommand(Controller Reciever, string Command, string Response, int callBackTimeout)
        {
            reciever = Reciever;
            command = Command;
            response = Response;
            timeout = callBackTimeout;
        }
        public override bool Execute()
        {
            if(timeout==0) return reciever.WriteCommand(command, response);
            else return reciever.WriteCommand(command, response, timeout);
        }

        public override bool Undo()
        {
            return false;
        }
    }
}
