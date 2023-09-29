using System.ComponentModel;

namespace Ref
{
    public enum ReflectMode : int
    {
        [Description("Без режима")]
        NoMode = 0,
        [Description("Импульсный")]
        Impulse = 1,
        [Description("Ждущий")]
        Continuous = 2
    }
   
}

