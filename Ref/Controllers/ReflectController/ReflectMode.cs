using System.ComponentModel;

namespace Ref.Controllers.ReflectController
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

