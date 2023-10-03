using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ref.BaseClasses.Commands
{
    public class StandartCommand : CommandBase
    {
        private string command;
        private Controller reciever;
        public StandartCommand(Controller Reciever, string Command)
        {
            reciever = Reciever;
            command = Command;
        }

        public override bool Execute()
        {
            return reciever.WriteCommand(command);
        }

        public override bool Undo()
        {
            return false;
        }
    }
}
