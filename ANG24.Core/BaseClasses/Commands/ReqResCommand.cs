using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANG24.Core.BaseClasses.Commands
{
    [Serializable]
    public class ReqResCommand : CommandBase
    {
        //req - res
        public event Action<ReqResCommand> OnResponseRecieved;
        public void InvokeCompletedCommand(ReqResCommand command) => OnResponseRecieved?.Invoke(command);
        public string Response { get; set; } = string.Empty;
        public int Timeout { get; set; }
        public ReqResCommand(string Command, string Response, int Timeout = 100) : base(Command)
        {
            this.Response = Response;
            this.Timeout = Timeout;
        }


    }

}
