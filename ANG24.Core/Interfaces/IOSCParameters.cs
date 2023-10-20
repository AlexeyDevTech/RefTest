using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANG24.Core.Interfaces
{

    public delegate void K_ChangedEventHandler();
    public delegate void L_ChangedEventHandler();
    public delegate void SR_ChangedEventHandler();
    public delegate void LenMass_ChangedEventHandler();
    public delegate void K_TriggeredControlEventHandler();

    public interface IOSCParameters
    {
        event K_ChangedEventHandler K_Changed;
        event L_ChangedEventHandler L_Changed;
        event SR_ChangedEventHandler SR_Changed;
        event LenMass_ChangedEventHandler LenMass_Changed;
        event K_TriggeredControlEventHandler K_TriggeredControl;

        double K { get; set; }
        double L { get; set; }
        int PulseLen { get; set; }
        double StandardSR { get; }
        int LenMass { get; }
        int VoltDiv { get; set; }
        int TriggerLevel { get; set; }
        void K_Triggered();
    }
}
