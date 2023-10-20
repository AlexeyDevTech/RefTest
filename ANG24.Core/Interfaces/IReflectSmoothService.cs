using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANG24.Core.Interfaces
{
    public interface IReflectSmoothService
    {
        int WindowSize { get; set; }
        bool Enabled { get; set; }
        ushort[] AddAndGetResult(ushort[] data, int channel);
        void Clear();
        ushort[] GetResult();
    }
}
