using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANG24.Core.Enums
{
    public enum ChainState : int
    {
        [Description("Одиночная отправка череды команд")]
        Single = 0,
        [Description("Режим сборки комманд, пока не будет вызван ExecuteChain()")]
        Chain = 1,
        [Description("Режим сборки комманд, пока не будет вызван ExecuteChain(), после выполнения состояние сбрасывается на Single")]
        ChainAuto = 2
    }
}
