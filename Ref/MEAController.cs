using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Ref
{
    public class MEAController : Controller
    {
        public MEAController() : base()
        {
            OnDataReceivedAction = OnData;
        }

        private void OnData(ControllerData data)
        {
            var d = new MEAControllerData(data.Message); 
            Console.WriteLine(d.Message);
        }
    }

    public interface IMEAController 
    { 

    }
    public class MEAControllerData : ControllerData
    {
        private int oldBatteryVoltage;
        public static bool SVI_Power_On = false;
        public static LabModules Module { get; set; }
        public static int Step_Faze { get; set; }
        public static WorkMode workMode { get; set; }
        public static int Step { get; set; }
        public static double Voltage { get; set; }
        public static double Current { get; set; }
        public static double BurnCurrent => GetBurnCurrent(Current);
        public static double BurnCurrentStep => GetBurnCurrent(Current, Step);
        public static double BurnVoltage => GetBurnVoltage(Voltage, Step, Current);
        public static GVIFireMode FireMode { get; set; }
        public static int BatteryVoltage { get; set; }
        public static GVIErrorInfo GVIErrorMessage { get; set; }
        public static double TempTransformator { get; set; }
        public static double PowerOutValue { get; set; }
        public static int CurrentProtect { get; set; }
        public static BurnPower PowerBurn { get; set; }
        public static int FireDelay { get; set; }
        public static int CountFire { get; set; }
        public static PowerInfo PowerInfoMessage { get; set; }
        public static double StableVoltage { get; set; }
        public static double StableCurrent { get; set; }
        public static int VoltageType => GetVoltageType(Voltage, StableVoltage);
        public static bool ConnectionEnable { get; set; }
        public static string ErrorCode1 { get; set; }
        public static string ErrorCode2 { get; set; }
        public static string ErrorCode3 { get; set; }
        public static string ErrorCode4 { get; set; }
        public static MKZStateInfo MKZState { get; set; }
        public static double OutRNVoltage { get; set; }
        public static double OutRNCurrent { get; set; }
        //public string Message { get; set; } = string.Empty;
        public static HVSwitchState HVSwitch1 { get; set; }
        public static HVSwitchState HVSwitch2 { get; set; }
        public static HVSwitchState HVSwitch3 { get; set; }
        public static bool Simulation { get; set; }
        public static string line { get; set; }
        public MEAControllerData(string message) : base()
        {
            var receivedData = message.Trim('\r', '\n').Trim();
            var vals = receivedData.Split(',');
            if (vals[0].IndexOf("\0") != -1)
            {
                vals[0] = vals[0].Replace("\0", "");
            }
            int lenVals = vals.Length;
            switch (lenVals)
            {
                case 4:
                    if (receivedData.StartsWith("HV_SWITCH"))
                    {
                        HVSwitch1 = GetHVSwitch(vals[1]);
                        HVSwitch2 = GetHVSwitch(vals[2]);
                        HVSwitch3 = GetHVSwitch(vals[3]);
                    }
                    else Message = receivedData;  // Данные для калибровки СВИ
                    break;
                case 19:
                    Module = GetCurrentModule(vals[0]);
                    Step_Faze = GetPhase(vals[1]);
                    workMode = GetWorkMode(vals[2]);
                    Step = GetStep(vals[3]);

                    if (!LabConfig.Config.OptionalParams.SVICalib)
                    {
                        if ((Module == LabModules.HVBurn
                         || Module == LabModules.HVMAC
                         || Module == LabModules.HVMDC
                         || Module == LabModules.JoinBurn)
                            && !SVI_Power_On)
                        {
                            Voltage = 0.0;
                            StableVoltage = 0.0;
                        }
                        else
                        {
                            Voltage = Convert.ToDouble(vals[4]);
                            StableVoltage = Convert.ToDouble(vals[10]);
                        }
                    }
                    else
                    {
                        Voltage = Convert.ToDouble(vals[4]);
                        StableVoltage = Convert.ToDouble(vals[10]);
                    }
                    Current = Convert.ToDouble(vals[5]);
                    //StableVoltage = Convert.ToDouble(vals[10]);
                    StableCurrent = Convert.ToDouble(vals[11]);
                    FireMode = GetFireMode(vals[5]);
                    BatteryVoltage = GetBatteryVoltage(Convert.ToInt32(vals[6]));
                    GVIErrorMessage = new GVIErrorInfo(vals[6]);
                    TempTransformator = Convert.ToDouble(vals[6]);
                    PowerOutValue = Convert.ToDouble(vals[7]);
                    FireDelay = Convert.ToInt32(vals[7]);
                    PowerBurn = GetPowerBurn(vals[7]);
                    CurrentProtect = Convert.ToInt32(vals[7]);
                    CountFire = Convert.ToInt32(vals[8]);
                    PowerInfoMessage = new PowerInfo(vals[9]);

                    ConnectionEnable = GetConnectionInfo(vals[12]);
                    ErrorCode1 = vals[12];
                    ErrorCode2 = vals[13];
                    ErrorCode3 = vals[14];
                    ErrorCode4 = vals[15];
                    MKZState = new MKZStateInfo(vals[16]);
                    OutRNVoltage = Convert.ToDouble(vals[17]);
                    OutRNCurrent = Convert.ToDouble(vals[18]);
                    break;
                default:
                    if (receivedData.StartsWith("VOL")) Message = receivedData; // Данные для калибровки СВИ
                    else Message = vals[0];
                    break;
            }

            line = receivedData;

        }

        private HVSwitchState GetHVSwitch(string value)
        {
            var result = HVSwitchState.Empty;
            try
            {
                value = value.Substring(4);
                if (!string.IsNullOrEmpty(value)) value = value.Substring(0, 1);
                switch (value)
                {
                    case "1":
                        result = HVSwitchState.HVM;
                        break;
                    case "2":
                        result = HVSwitchState.Burn;
                        break;
                    case "3":
                        result = HVSwitchState.GVI;
                        break;
                    case "4":
                        result = HVSwitchState.Meg;
                        break;
                    case "5":
                        result = HVSwitchState.Ground;
                        break;
                    case "0":
                        result = HVSwitchState.NoMode;
                        break;
                }
            }
            catch
            {

            }
            return result;
        }

        private LabModules GetCurrentModule(string value)
        {
            switch (value)
            {
                case "MAIN":
                    return LabModules.Main;
                case "HVMAC":
                    return LabModules.HVMAC;
                case "HVMDC":
                    return LabModules.HVMDC;
                case "BURN":
                    return LabModules.Burn;
                case "JOINTBURN":
                    return LabModules.JoinBurn;
                case "HVBURN":
                    return LabModules.HVBurn;
                case "HVPULSE":
                    return LabModules.GVI;
                case "MEAS":
                    return LabModules.Meas;
                case "GP500":
                    return LabModules.GP500;
                case "LVM":
                    return LabModules.LVMeas;
                case "Tangent2000":
                    return LabModules.Tangent2000;
                case "VLF":
                    return LabModules.VLF;
                case "SA640":
                    return LabModules.SA640;
                case "SA540_1":
                    return LabModules.SA540_1;
                case "SA540_3":
                    return LabModules.SA540_3;
                case "BRIDGE":
                    return LabModules.Bridge;
                default:
                    return LabModules.NoMode;
            }
        }

        private GVIFireMode GetFireMode(string value)
        {
            switch (value)
            {
                case "1":
                    return GVIFireMode.Single;
                case "2":
                    return GVIFireMode.Multiple;
                case "3":
                    return GVIFireMode.InfinityMultiple;
                default:
                    return GVIFireMode.NoFireMode;

            }
        }

        private BurnPower GetPowerBurn(string value)
        {
            switch (value)
            {
                case "50":
                    return BurnPower.Power50;
                case "100":
                    return BurnPower.Power100;
                default:
                    return BurnPower.NoPower;

            }
        }

        private WorkMode GetWorkMode(string value)
        {
            switch (value)
            {
                case "HAND":
                    return WorkMode.Manual;
                case "AUTO":
                    return WorkMode.Auto;
                default:
                    return WorkMode.NoMode;

            }
        }

        private int GetBatteryVoltage(int value)
        {
            int voltageBattery = value;
            int range = 0;
            if (oldBatteryVoltage > voltageBattery)
            {
                // понижение напряжения
                if (value < 600)
                {
                    range = 0;
                }
                else if (value >= 600 && value < 900)
                {
                    range = 1;
                }
                else if (value >= 850 && value < 1150)
                {
                    range = 2;
                }
                else if (value >= 1100 && value < 1350)
                {
                    range = 3;
                }
                else if (value >= 1300)
                {
                    range = 4;
                }

                oldBatteryVoltage = voltageBattery;
            }
            else
            {
                // повышение напряжения
                if (value < 700)
                {
                    range = 0;
                }
                else if (value >= 650 && value < 950)
                {
                    range = 1;
                }
                else if (value >= 900 && value < 1200)
                {
                    range = 2;
                }
                else if (value >= 1150 && value < 1450)
                {
                    range = 3;
                }
                else if (value >= 1450)
                {
                    range = 4;
                }
            }


            return range;
        }

        private bool GetConnectionInfo(string value)
        {
            bool res = false;
            try
            {
                var i = Int32.Parse(value);
                if (i == 0) res = true;
                else res = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return res;
        }

        private int GetPhase(string value)
        {
            switch (value)
            {
                case "PHASE1":
                    return 1;
                case "PHASE2":
                    return 2;
                case "PHASE3":
                    return 3;
                default:
                    return 0;
            }
        }

        private int GetStep(string value)
        {
            switch (value)
            {
                case "ST1":
                    return 1;
                case "ST2":
                    return 2;
                case "ST3":
                    return 3;
                case "ST4":
                    return 4;
                case "ST5":
                    return 5;
                case "ST6":
                    return 6;
                case "ST7":
                    return 7;
                default:
                    return 0;

            }
        }

        private static double GetBurnCurrent(double cur)
        {

            var percent = 100 * (cur / 290.0);
            return percent;

        }

        private static double GetBurnCurrent(double cur, int step)
        {
            var coef = LabConfig.Config.Parameters.BurnKoefParams;
            switch (step)
            {
                case 1:
                    return (cur / coef.BurnCurrentCoef1) * 34;
                case 2:
                    return (cur / coef.BurnCurrentCoef2) * 65;
                case 3:
                    return (cur / coef.BurnCurrentCoef3) * 130;
                case 4:
                    return (cur / coef.BurnCurrentCoef4) * 260;
                case 5:
                    return (cur / coef.BurnCurrentCoef5) * 700;
                case 6:
                    return (cur / coef.BurnCurrentCoef6) * 2400;
                case 7:
                    return (cur / coef.BurnCurrentCoef7) * 9100;
                default:
                    return 0.0;
            }
        }

        private static double GetBurnVoltage(double Uval, int step, double CurrentBurn)
        {

            double res = 0;
            switch (step)
            {
                case 1:
                    res = (Uval - (CurrentBurn * 0.431)) * 68.18;
                    if (res < 0) res = 0.0;
                    return res;
                case 2:
                    res = (Uval - (CurrentBurn * 0.431)) * 36.36;
                    if (res < 0) res = 0.0;
                    return res;
                case 3:
                    res = (Uval - (CurrentBurn * 0.431)) * 18.18;
                    if (res < 0) res = 0.0;
                    return res;
                case 4:
                    res = (Uval - (CurrentBurn * 0.431)) * 9.09;
                    if (res < 0) res = 0.0;
                    return res;
                case 5:
                    res = (Uval - (CurrentBurn * 0.788)) * 3.4;
                    if (res < 0) res = 0.0;
                    return res;
                case 6:
                    res = Uval - (CurrentBurn * 0.788);
                    if (res < 0) res = 0.0;
                    return res;
                case 7:
                    res = (Uval - (CurrentBurn * 0.788)) * 0.263;
                    if (res < 0) res = 0.0;
                    return res;
                default:
                    return 0.0;
            }

        }

        private static int GetVoltageType(double VoltageAC, double VoltageDC)
        {
            // 1 - Переменка; 2 - Постоянка; 3 - Колебания
            var minKV = LabConfig.Config.Parameters.MinKV;
            var maxKV = LabConfig.Config.Parameters.MaxKV;
            int result = 0;
            var cal = Math.Abs((VoltageAC - VoltageDC) / (VoltageAC + VoltageDC));
            if (cal > maxKV) result = 1;
            if (cal < minKV) result = 2;
            if (cal >= minKV && cal <= maxKV) result = 3;
            return result;
        }
    }

    public enum LabModules
    {
        Main,
        HVMAC,
        HVMDC,
        HVMDCHi,
        HVBurn,
        Burn,
        JoinBurn,
        Bridge,
        GVI,
        GP500,
        LVMeas,
        Meas,
        Reflect,
        NoMode,
        SA640,
        SA540,
        SA540_1,
        SA540_3,
        VLF,
        Tangent2000,
        Activation

    }
    public enum WorkMode
    {
        Manual,
        Auto,
        NoMode
    }
    public enum GVIFireMode : int
    {
        Single = 1,
        Multiple = 2,
        InfinityMultiple = 3,
        NoFireMode = 0
    }
    public enum BurnPower
    {
        Power50,
        Power100,
        NoPower
    }
    public enum FazeType : int
    {
        NoFaze = 0,
        OneFaze = 1,
        ThreeFaze = 3
    }
    public enum HVSwitchState
    {
        Empty,
        NoMode,
        HVM,
        Burn,
        GVI,
        Meg,
        Ground,
        GP500,
        Bridge,
    }

    public class GVIErrorInfo
    {
        public bool ChargerSet { get; set; }
        public bool ChargerReset { get; set; }
        public bool MeasureSet { get; set; }
        public bool MeasureReset { get; set; }
        public bool LeavingCapacitor { get; set; }
        public bool ShutingCapacitor { get; set; }
        public bool LeavingCable { get; set; }
        public bool ShutingCable { get; set; }
        public bool LeavingCap2Cab { get; set; }
        public bool ShutingCap2Cab { get; set; }
        public bool HiTemp { get; set; }
        public bool HiTemp2 { get; set; }
        public bool OverCap { get; set; }
        public bool OverReg { get; set; }
        public bool CriticalErrorCapShoot { get; set; }
        public bool CriticalErrorCabShoot { get; set; }

        public GVIErrorInfo(string InputData)
        {
            var i = Int32.Parse(InputData);
            if ((i & 0x0001) != 0) ChargerSet = true;               //зарядник не включен
            if ((i & 0x0002) != 0) ChargerReset = true;             //зарядник не выключен
            if ((i & 0x0004) != 0) MeasureSet = true;               //измеритель не включен
            if ((i & 0x0008) != 0) MeasureReset = true;             //измеритель не выключен
            if ((i & 0x0010) != 0) LeavingCapacitor = true;         //магнит замыкания конденсатора не раскоротился
            if ((i & 0x0020) != 0) ShutingCapacitor = true;         //магнит замыкания конденсатора не замкнулся
            if ((i & 0x0040) != 0) LeavingCable = true;             //магнит замыкания кабеля не раскоротился
            if ((i & 0x0080) != 0) ShutingCable = true;             //магнит замыкания кабеля не замкнулся
            if ((i & 0x0100) != 0) LeavingCap2Cab = true;           //магнит замыкания рабочего ключа не раскоротился
            if ((i & 0x0200) != 0) ShutingCap2Cab = true;           //магнит замыкания рабочего ключа не замкнулся
            if ((i & 0x0400) != 0) HiTemp = true;                   //высокая температура трансформатора 1
            if ((i & 0x0800) != 0) HiTemp2 = true;                  //высокая температура трансформатора 2
            if ((i & 0x1000) != 0) OverCap = true;                  //перенапряжение конденсатора
            if ((i & 0x2000) != 0) OverReg = true;                  //перенапряжение входное
            if ((i & 0x4000) != 0) CriticalErrorCapShoot = true;    //критическая ошибка замыкателя конденсатора
            if ((i & 0x8000) != 0) CriticalErrorCabShoot = true;    //критическая ошибка замыкателя кабеля
        }
        public GVIErrorInfo()
        {

        }
    }
    public class PowerInfo
    {
        public bool RNParking { get; set; }
        public bool RNManualEnable { get; set; }
        public bool HVMEnable { get; set; }
        public bool GVIEnable { get; set; }
        public bool BurnEnable { get; set; }
        public bool GP500Enable { get; set; }
        public bool LVMEnable { get; set; }
        public bool MeasEnable { get; set; }
        public bool JoinBurnEnable { get; set; }
        /// <summary>
        /// Рабочая земля от MKZ испытаний
        /// </summary>
        public bool KM1_MKZEnable { get; set; }
        public bool IDMEnable { get; set; }
        public bool ProtectedDrosselEnable { get; set; }
        /// <summary>
        /// Рабочая земла от MKZ ГП-500
        /// </summary>
        public bool KM3_MKZEnable { get; set; }
        public bool MVKUp { get; set; }
        public bool MSKEnable { get; set; }
        public bool SVIPowerEnable { get; set; }
        /// <summary>
        /// Указывает на что, что включена защита на 100(200)mA, если показывает false -- указывает на то, что включена защита 20mA
        /// </summary>
        public bool CurrentProtection100Enable { get; set; }
        public bool VREnable { get; set; }
        public bool BridgeEnable { get; set; }
        public bool VLFEnable { get; set; }
        public bool VoltageUpFlag { get; set; }
        public bool VoltageDownFlag { get; set; }
        public ModulePowerInfo ModulePower => new ModulePowerInfo(this);

        public bool SA540Enable { get; set; }
        public bool SA640Enable { get; set; }
        public bool Tangent2000Enable { get; set; }
        public bool SpeedFlag { get; set; }

        public PowerInfo(string InputData)
        {
            uint i = 0;
            var suc = UInt32.TryParse(InputData, out i);
            if (suc)
            {
                if ((i & 0x01) != 0) VoltageUpFlag = true;
                if ((i & 0x02) != 0) VoltageDownFlag = true;
                if ((i & 0x80) != 0) RNParking = true;
                if ((i & 0x20) != 0) RNManualEnable = true;
                //if ((i & 0x00010000) != 0) HVMEnable = true;
                //if ((i & 0x00020000) != 0) GVIEnable = true;
                //if ((i & 0x00040000) != 0) BurnEnable = true;
                if ((i & 0x00080000) != 0) GP500Enable = true;
                if ((i & 0x00100000) != 0) LVMEnable = true;
                if ((i & 0x00200000) != 0) MeasEnable = true;
                if ((i & 0x00400000) != 0) JoinBurnEnable = true;
                if (LabConfig.Config.OptionalParams.VREnable)
                {
                    VREnable = true;
                }
                if (LabConfig.Config.OptionalParams.ServiceMode)
                {
                    VREnable = true;
                    HVMEnable = true;
                    GVIEnable = true;
                    BurnEnable = true;
                    MVKUp = true;
                    SVIPowerEnable = true;
                    KM1_MKZEnable = true;
                }
                else
                {
                    if ((i & 0x00800000) != 0) VREnable = true;
                    if ((i & 0x00010000) != 0) HVMEnable = true;
                    if ((i & 0x00020000) != 0) GVIEnable = true;
                    if ((i & 0x00040000) != 0) BurnEnable = true;
                    if ((i & 0x00001000) != 0) MVKUp = true;
                    if ((i & 0x00004000) != 0) SVIPowerEnable = true;
                    if ((i & 0x01000000) != 0) KM1_MKZEnable = true;
                }
                if ((i & 0x01000000) != 0) KM1_MKZEnable = true;
                if ((i & 0x02000000) != 0) IDMEnable = true;
                if ((i & 0x04000000) != 0) ProtectedDrosselEnable = true;
                if ((i & 0x08000000) != 0) KM3_MKZEnable = true;
                //if ((i & 0x00001000) != 0) MVKUp = true;
                if ((i & 0x00002000) != 0) MSKEnable = true;
                if ((i & 0x00004000) != 0) SVIPowerEnable = true;
                if ((i & 0x00008000) != 0) CurrentProtection100Enable = true;
                if ((i & 0x10000000) != 0) BridgeEnable = true;
                if ((i & 0x20000000) != 0) SA540Enable = true;
                if ((i & 0x40000000) != 0) SA640Enable = true;
                if ((i & 0x80000000) != 0) VLFEnable = true;
                if ((i & 0x04) != 0) SpeedFlag = true;


            }
            //ModulePower = new ModulePowerInfo(this);

        }
        public PowerInfo()
        {
            //ModulePower = new ModulePowerInfo(this);
        }
    }
    public class MKZStateInfo
    {
        private bool _doorLeft;
        private bool _doorRight;
        private bool _dangerousPotencial;
        private bool _ground;
        private bool _safeKey;
        private bool _stop;

        public bool MKZError
        {
            get
            {
                if (LabConfig.Config.FazeType == FazeType.ThreeFaze)
                {
                    if (!Stop || !SafeKey || !DangerousPotencial || !Ground || !DoorLeft || !DoorRight) return true;
                    else return false;
                }
                else
                {
                    if (!Stop || !SafeKey || !DangerousPotencial || !DoorLeft || !DoorRight) return true;
                    else return false;
                }
            }
        }
        public bool DoorLeft
        {
            get => _doorLeft;
            set
            {
                _doorLeft = value;
                _ = MKZError;
            }
        }
        public bool DoorRight
        {
            get => _doorRight;
            set
            {
                _doorRight = value;
                _ = MKZError;
            }
        }
        public bool DangerousPotencial
        {
            get => _dangerousPotencial;
            set
            {
                _dangerousPotencial = value;
                _ = MKZError;
            }
        }
        public bool Ground
        {
            get => _ground;
            set
            {
                _ground = value;
                _ = MKZError;
            }
        }
        public bool SafeKey
        {
            get => _safeKey;
            set
            {
                _safeKey = value;
                _ = MKZError;
            }
        }
        public bool Stop
        {
            get => _stop;
            set
            {
                _stop = value;
                _ = MKZError;
            }
        }
        public MKZStateInfo(string InputData)
        {
            var i = Int32.Parse(InputData);
            if ((i & 0x01) != 0) Stop = true;
            if ((i & 0x02) != 0) SafeKey = true;
            if ((i & 0x04) != 0) DangerousPotencial = true;
            if ((i & 0x08) != 0) Ground = true;
            if ((i & 0x10) != 0) DoorLeft = true;
            if ((i & 0x20) != 0) DoorRight = true;
            //if (Stop || SafeKey || DangerousPotencial || Ground || DoorLeft || DoorRight) MKZError = true;
        }
        public MKZStateInfo()
        {
            //if (Stop || SafeKey || DangerousPotencial || Ground || DoorLeft || DoorRight) MKZError = true;
        }
    }
    public class ModulePowerInfo
    {
        public ModulePowerState BurnModuleEnable { get; set; }
        public ModulePowerState HVMDCModuleEnable { get; set; }
        public ModulePowerState HVMACModuleEnable { get; set; }
        public ModulePowerState GVIModuleEnable { get; set; }
        public ModulePowerState GP500ModuleEnable { get; set; }
        public ModulePowerState MeasureModuleEnable { get; set; }
        public ModulePowerState JoinBurnModuleEnable { get; set; }
        public ModulePowerState HVBurnModuleEnable { get; set; }
        public ModulePowerState LVMModuleEnable { get; set; }
        public ModulePowerState BridgeModuleEnable { get; set; }
        public ModulePowerState SA540MoudleEnable { get; set; }
        public ModulePowerState SA640MoudleEnable { get; set; }
        public ModulePowerState VLFModuleEnable { get; set; }
        public ModulePowerState Tangent2000Enable { get; set; }

        public ModulePowerInfo(PowerInfo info)
        {
            BurnModuleEnable = new ModuleWorkAnalyser(LabModules.Burn, info).WorkState;
            HVMDCModuleEnable = new ModuleWorkAnalyser(LabModules.HVMDC, info).WorkState;
            HVMACModuleEnable = new ModuleWorkAnalyser(LabModules.HVMAC, info).WorkState;
            GVIModuleEnable = new ModuleWorkAnalyser(LabModules.GVI, info).WorkState;
            GP500ModuleEnable = new ModuleWorkAnalyser(LabModules.GP500, info).WorkState;
            MeasureModuleEnable = new ModuleWorkAnalyser(LabModules.Meas, info).WorkState;
            JoinBurnModuleEnable = new ModuleWorkAnalyser(LabModules.JoinBurn, info).WorkState;
            HVBurnModuleEnable = new ModuleWorkAnalyser(LabModules.HVBurn, info).WorkState;
            LVMModuleEnable = new ModuleWorkAnalyser(LabModules.LVMeas, info).WorkState;
            BridgeModuleEnable = new ModuleWorkAnalyser(LabModules.Bridge, info).WorkState;
            SA540MoudleEnable = new ModuleWorkAnalyser(LabModules.SA540, info).WorkState;
            SA540MoudleEnable = new ModuleWorkAnalyser(LabModules.SA540_1, info).WorkState;
            SA540MoudleEnable = new ModuleWorkAnalyser(LabModules.SA540_3, info).WorkState;
            SA640MoudleEnable = new ModuleWorkAnalyser(LabModules.SA640, info).WorkState;
            VLFModuleEnable = new ModuleWorkAnalyser(LabModules.VLF, info).WorkState;
            Tangent2000Enable = new ModuleWorkAnalyser(LabModules.Tangent2000, info).WorkState;
        }
    }
    public enum ModulePowerState : int
    {
        Disable,
        EnableFail,
        Enable
    }
    public class ModuleWorkAnalyser
    {
        public ModulePowerState WorkState { get; set; }
        private List<bool> TargetFlags { get; set; }
        private bool MainTarget { get; set; }
        public ModuleWorkAnalyser(LabModules CurrentModule, PowerInfo powerInfo)
        {
            switch (CurrentModule)
            {
                case LabModules.Bridge:
                    TargetFlags = new List<bool>
                    {
                        powerInfo.BridgeEnable
                        //powerInfo.HVMEnable
                    };
                    MainTarget = powerInfo.BridgeEnable;
                    break;
                case LabModules.Burn:
                    TargetFlags = new List<bool>
                   {
                       powerInfo.BurnEnable,
                       powerInfo.MVKUp,
                       powerInfo.VREnable
                   };
                    MainTarget = powerInfo.BurnEnable;
                    break;
                case LabModules.GP500:
                    TargetFlags = new List<bool>
                    {
                        powerInfo.MVKUp,
                        powerInfo.GP500Enable,
                        powerInfo.KM3_MKZEnable
                    };
                    MainTarget = powerInfo.GP500Enable;
                    break;
                case LabModules.GVI:
                    TargetFlags = new List<bool>
                    {
                        powerInfo.MVKUp,
                        powerInfo.GVIEnable,
                        powerInfo.VREnable
                    };
                    MainTarget = powerInfo.GVIEnable;
                    break;
                case LabModules.HVBurn:
                    TargetFlags = new List<bool>
                    {
                        powerInfo.MVKUp,
                        powerInfo.SVIPowerEnable,
                        powerInfo.ProtectedDrosselEnable,
                        powerInfo.VREnable,
                        powerInfo.HVMEnable
                    };
                    MainTarget = powerInfo.HVMEnable;
                    break;
                case LabModules.HVMAC:
                    TargetFlags = new List<bool>
                    {
                        powerInfo.HVMEnable,
                        powerInfo.SVIPowerEnable,
                        powerInfo.VREnable,
                        powerInfo.KM1_MKZEnable
                    };
                    MainTarget = powerInfo.HVMEnable;
                    break;
                case LabModules.HVMDC:
                    TargetFlags = new List<bool>
                    {
                        powerInfo.HVMEnable,
                        powerInfo.MVKUp,
                        powerInfo.SVIPowerEnable,
                        powerInfo.VREnable,
                        powerInfo.KM1_MKZEnable
                    };
                    MainTarget = powerInfo.HVMEnable;
                    break;
                case LabModules.JoinBurn:
                    TargetFlags = new List<bool>
                    {
                        powerInfo.MVKUp,
                        powerInfo.SVIPowerEnable,
                        powerInfo.MSKEnable,
                        powerInfo.ProtectedDrosselEnable,
                        powerInfo.VREnable,
                        powerInfo.HVMEnable,
                        powerInfo.JoinBurnEnable
                    };
                    MainTarget = powerInfo.JoinBurnEnable;
                    break;
                case LabModules.LVMeas:
                    TargetFlags = new List<bool>
                    {
                        powerInfo.LVMEnable,
                        powerInfo.VREnable
                    };
                    MainTarget = powerInfo.LVMEnable;
                    break;
                case LabModules.Meas:
                    TargetFlags = new List<bool>
                    {
                        powerInfo.MVKUp,
                        powerInfo.MeasEnable
                    };
                    MainTarget = powerInfo.MeasEnable;
                    break;
                case LabModules.SA540:
                case LabModules.SA540_1:
                case LabModules.SA540_3:
                    TargetFlags = new List<bool>
                    {
                        powerInfo.SA540Enable
                    };
                    MainTarget = powerInfo.SA540Enable;
                    break;
                case LabModules.SA640:
                    TargetFlags = new List<bool>
                    {
                        powerInfo.SA640Enable
                    };
                    MainTarget = powerInfo.SA640Enable;
                    break;
                case LabModules.VLF:
                    TargetFlags = new List<bool>
                    {
                        powerInfo.VLFEnable
                    };
                    MainTarget = powerInfo.VLFEnable;
                    break;
                case LabModules.Tangent2000:
                    TargetFlags = new List<bool>
                    {
                        powerInfo.Tangent2000Enable
                    };
                    MainTarget = powerInfo.Tangent2000Enable;
                    break;
                default:
                    TargetFlags = new List<bool>();
                    MainTarget = false;
                    break;
            }
            if (!TargetFlags.Contains(false)) WorkState = ModulePowerState.Enable;
            else if (MainTarget == true) WorkState = ModulePowerState.EnableFail;
            else WorkState = ModulePowerState.Disable;
        }

    }

}
