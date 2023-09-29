using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ref.Interfaces
{
    public interface IControllerData
    {
        string Message { get; set; }
        bool IsSuccess { get; set; }
    }
}
