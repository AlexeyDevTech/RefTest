using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANG24.Core.Interfaces
{
    public interface IControllerData
    {
        string Message { get; set; }
        bool IsSuccess { get; set; }
    }
}
