using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANG24.Core.BaseClasses.Commands
{
    [Serializable]
    public class StandardCommand : CommandBase
    { 
        public StandardCommand(string Command) : base(Command)
        {

        }
    }
}
