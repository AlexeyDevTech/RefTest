using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ref.BaseClasses.Commands
{
    public class Invoker
    {
        CommandBase command;
        public Invoker SetCommand(CommandBase c)
        {
            command = c;
            return this;
        }
        public Invoker ExecuteCommand()
        {
            command.Execute();
            return this;
        }
        public Invoker CancelCommand()
        {
            command.Undo();
            return this;
        }
    }
}
