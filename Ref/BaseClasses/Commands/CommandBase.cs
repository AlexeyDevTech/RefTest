using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ref.BaseClasses.Commands
{
    [Serializable]
    public abstract class CommandBase
    {
        public bool IsExecuting = false;
        public string Command { get; set; } = string.Empty;
        public CommandBase(string Command)
        {
            this.Command = Command;
        }
    }
}
