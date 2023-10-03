using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ref.BaseClasses.Commands
{
    public abstract class CommandBase
    {
        public abstract bool Execute();
        public abstract bool Undo();
    }
}
