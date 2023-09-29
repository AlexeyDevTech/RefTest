using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static Ref.Config.ExternalSoftwareParams.Mods;
using System.Xml.Linq;
using Newtonsoft.Json;
using Ref.Controllers;

namespace Ref
{
    public delegate void FazeTypeChanged(FazeType fazeType);
    public delegate void ThermoVisionChanged(bool ThermoVision);
    public delegate void DebugModeEnabledEventHandler();
    public delegate void LabCompositionParsed();
    public static class LabConfig
    {
        public static bool StartBlock = false;

        public static event FazeTypeChanged OnFazeChanged;
        public static event ThermoVisionChanged OnThermoVisionChanged;
        public static event LabCompositionParsed OnLabCompositionParsed;
        public static event DebugModeEnabledEventHandler DebugModeEnabled;
        public static event Action OnOptionsChanged;
        public static event Action OnKTChanged;

        public static Config Config { get; set; } = new Config();
        public static bool ReflectOnline { get; set; }
        public static void EventDebugMode() => DebugModeEnabled?.Invoke();
        public static void OptionsChanged() => OnOptionsChanged?.Invoke();
        public static void KTChanged() => OnKTChanged?.Invoke();
        public static void Deserialize()
        {
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
                Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(@"Config.json"));
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Файл конфигурации не найден. Будут выполнены настройки по умолчанию.");
                OldDeserialize();
            }
            catch (FormatException fex)
            {
                Console.WriteLine(fex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла непредвиденная ошибка: {0}", ex.Message);
            }
        }
        public static void InitEvent()
        {
            EventDebugMode();
            OnFazeChanged?.Invoke(Config.FazeType);
            OnThermoVisionChanged?.Invoke(Config.ThermoVision);
            OnLabCompositionParsed?.Invoke();
        }
        /// <summary>
        /// Выполнение сериализации настроек лаборатории
        /// </summary>
        public static void Serialize() => File.WriteAllText(@"Config.json", JsonConvert.SerializeObject(Config));
        private static void OldDeserialize()
        {
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
                Config = new Config();
                XDocument doc = XDocument.Load("Config.xml");
                XElement root = doc.Element("Config");
                if (root == null) return;
                XElement Params = root.Element("Params");
                XElement ExternalSoftwareParams = root.Element("ExternalSoftwareParams");


                Config.SerialNumber = Convert.ToInt32(root.Element("SerialNumber")?.Value);
                Config.FazeType = (FazeType)Convert.ToInt32(root.Element("FazeType")?.Value);
                Config.MaxACVoltage = Convert.ToInt32(root.Element("MaxACVoltage")?.Value);
                Config.MaxDCVoltage = Convert.ToInt32(root.Element("MaxDCVoltage")?.Value);
                Config.CompensationIsPresent = Convert.ToBoolean(root.Element("CompensationIsPresent")?.Value);
                Config.DebugMode = Convert.ToBoolean(root.Element("DebugMode")?.Value);
                Config.LVMThreePhaze = Convert.ToBoolean(root.Element("LVMThreePhaze")?.Value);
                Config.ThermoVision = Convert.ToBoolean(root.Element("ThermoVision")?.Value ?? bool.FalseString);
                Config.MethodsBurn = Convert.ToBoolean(root.Element("MethodsBurn")?.Value ?? bool.FalseString);
                Config.ManualIDM = Convert.ToBoolean(root.Element("ManualIDM")?.Value ?? bool.FalseString);
                Config.IsMDPOn = Convert.ToBoolean(root.Element("IsMDPOn")?.Value ?? bool.FalseString);
                Config.RemoteConnection = Convert.ToBoolean(root.Element("RemoteConnection")?.Value ?? bool.FalseString);

                if (Params != null)
                {
                    XElement CurrentProtectedParams = Params.Element("CurrentProtectedParams");

                    Config.Parameters.CurrentProtectedParams.CurrentProtectText = Convert.ToInt16(Params.Element("CurrentProtectText")?.Value ?? "100");
                    Config.Parameters.MinKV = Convert.ToDouble(Params.Element("MinKV")?.Value ?? "0.4");
                    Config.Parameters.MaxKV = Convert.ToDouble(Params.Element("MaxKV")?.Value ?? "0.6");

                    Config.Parameters.HVPULSESTEP1 = Convert.ToDouble(Params.Element("HVPULSESTEP1")?.Value ?? "1.12");
                    Config.Parameters.HVPULSESTEP2 = Convert.ToDouble(Params.Element("HVPULSESTEP2")?.Value ?? "2.28");
                    Config.Parameters.HVPULSESTEP3 = Convert.ToDouble(Params.Element("HVPULSESTEP3")?.Value ?? "4.96");

                    Config.Parameters.KJSTEP1 = Convert.ToDouble(Params.Element("KJSTEP1")?.Value ?? "1.12");
                    Config.Parameters.KJSTEP2 = Convert.ToDouble(Params.Element("KJSTEP2")?.Value ?? "2.28");
                    Config.Parameters.KJSTEP3 = Convert.ToDouble(Params.Element("KJSTEP3")?.Value ?? "4.96");
                }
                if (Enum.TryParse(root.Element("Protocol")?.Value, out ProtocolType protocol)) Config.Protocol = protocol;
                foreach (var item in root.Element("LabComposition")?.Elements())
                {
                    var module = LabModules.NoMode;
                    bool resParse = Enum.TryParse(item.Value, out module);
                    if (resParse)
                        Config.LabComposition.Add(module);
                    else Console.Error.WriteLine("Указанный в конфигурации элемент \"{0}\" не найден в списке существующих.", item.Value);
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("file Config.xml not found");
            }
        }
    }
    public class Config
    {
        private TransformatorVoltage _transVolType;
        public int SerialNumber { get; set; } = 0;
        public bool DebugMode { get; set; } = false;
        public bool LVMThreePhaze { get; set; } = false;
        public bool ThermoVision { get; set; } = false;
        /// <summary>
        /// определяет тип лаборатории: 0 -- безфазная(по умолчанию), 1 -- однофазная, 3 -- трехфазная
        /// </summary>
        public FazeType FazeType { get; set; } = 0;
        /// <summary>
        /// Определяет максимальное напряжение в режиме испытаний переменным напряжением [кВ] (по умолчанию -- 0)
        /// </summary>
        public int MaxACVoltage { get; set; } = 0;
        /// <summary>
        /// Определяет максимальное напряжение в режиме испытаний постоянным напряжением [кВ] (по умолчанию -- 0)
        /// </summary>
        public int MaxDCVoltage { get; set; } = 0;
        /// <summary>
        /// Указывает, присутствует ли контроллер компенсации (по умолчанию -- false).
        /// </summary>
        public bool CompensationIsPresent { get; set; } = false;
        /// <summary>
        /// Указывает отображать ли методы прожига (по умолчанию -- false)
        /// </summary>
        public bool MethodsBurn { get; set; }
        /// <summary>
        /// Указывает отображать ручное использование ИДМ (по умолчанию -- false)
        /// </summary>
        public bool ManualIDM { get; set; } = false;
        public bool IsMDPOn { get; set; } = false;
        /// <summary>
        /// Указывает, отоброжать переключение скорости регулятора (по умолчанию -- true)
        /// </summary>
        public bool SpeedRegulator { get; set; } = true;
        /// <summary>
        /// Указывает наличие ИПРН в лаборатории (по умолчанию -- false)
        /// </summary>
        public bool IPRN { get; set; } = false;
        /// <summary>
        /// Указывает, отоброжать ли регуляторы напряжения в отдельном окне (по умолчанию -- false)
        /// </summary>
        public bool RNTangent2000 { get; set; } = false;
        /// <summary>
        /// Указывает отображение регулятора напряжения в отдельном окне (по умолчанию -- false)
        /// </summary>
        public bool RNLVMeas { get; set; } = false;
        /// <summary>
        /// Определяет тип протокола (По умолчанию default);
        /// </summary>
        public ProtocolType Protocol { get; set; } = ProtocolType.Default;
        /// <summary>
        /// Определяет версию рефлектометра: 1 -- КР90, 2 --КР120
        /// </summary>
        public int VersionReflect { get; set; } = 1;
        public TransformatorVoltage TransVolType
        {
            get => _transVolType;
            set
            {
                _transVolType = value;
                switch (_transVolType)
                {
                    case TransformatorVoltage.kv_50:
                        KT = 353.5;
                        break;

                    case TransformatorVoltage.kv_100:
                        KT = 707.1;
                        break;
                }
            }
        }
        /// <summary>
        /// Коэффициент трансформации (для расчета ЕРН)
        /// </summary>
        public double KT { get; private set; } = 353.5;

        /// <summary>
        /// Отображает удаленное подключение в настроках (по умолчанию -- false)
        /// </summary>
        public bool RemoteConnection { get; set; } = false;
        public List<LabModules> LabComposition { get; set; } = new List<LabModules>();
        public Params Parameters { get; set; } = new Params();
        public Options OptionalParams { get; set; } = new Options();
        public ExternalSoftwareParams ExternalSoftwareParameters { get; set; } = new ExternalSoftwareParams();
        public Config()
        {
            TransVolType = TransformatorVoltage.kv_50;
        }
        public class Options
        {
            public bool SVICalib { get; set; }
            public bool ReflectAsIntegratedModule { get; set; }
            public bool AutoBurnIsOn { get; set; }
            public bool AutoRise { get; set; } = true;
            public bool ObserveIDM { get; set; }
            public bool ServiceMode { get; set; }
            public bool VREnable { get; set; }
        }
        public class Params
        {
            public double MinKV { get; set; } = 0.4;
            public double MaxKV { get; set; } = 0.6;
            public double HVPULSESTEP1 { get; set; } = 1.12;
            public double HVPULSESTEP2 { get; set; } = 2.28;
            public double HVPULSESTEP3 { get; set; } = 4.96;
            public double KJSTEP1 { get; set; } = 1.12;
            public double KJSTEP2 { get; set; } = 2.28;
            public double KJSTEP3 { get; set; } = 4.96;
            public CurrentProtected CurrentProtectedParams { get; set; } = new CurrentProtected();
            public VoltageProtected VoltageProtectedParams { get; set; } = new VoltageProtected();
            public AutoBurn AutoBurnParams { get; set; } = new AutoBurn();
            public BurnKoef BurnKoefParams { get; set; } = new BurnKoef();

            public class CurrentProtected
            {
                /// <summary>
                /// Текст кнопки "защита по току" макс
                /// </summary>
                public short CurrentProtectText { get; set; } = 100;
                public int MaxCurrentRN { get; set; } = 260;
                public int MaxCurrentRN_BURN { get; set; } = 260;
                public int MaxCurrentRN_HVBURN { get; set; } = 260;
                public int MaxCurrentRN_HVPULSE { get; set; } = 260;
                public int MaxCurrentRN_JOINTBURN { get; set; } = 260;
                public int MaxCurrentRN_HVMDCHi { get; set; } = 260;
            }
            public class VoltageProtected
            {
                public int MaxVoltageRN { get; set; } = 220;
                public int MaxVoltageRN_BURN { get; set; } = 220;
                public int MaxVoltageRN_HVBURN { get; set; } = 220;
                public int MaxVoltageRN_HVPULSE { get; set; } = 220;
                public int MaxVoltageRN_JOINTBURN { get; set; } = 220;
                public int MaxVoltageRN_HVMDCHi { get; set; } = 220;
                public int MaxVoltageRN_LVM { get; set; } = 220;
                public int MaxVoltageDirect_Bridge { get; set; } = 220;
                public int MaxVoltageReverse_Bridge { get; set; } = 220;
            }
            public class AutoBurn
            {
                public int BurnTime { get; set; } = 15;
                public int BurnIStepUpPercent { get; set; } = 40;
                public int BurnIStepDownPercent { get; set; } = 10;
                public int BurnVlotagePercent { get; set; } = 50;
            }
            public class BurnKoef
            {
                public double BurnCurrentCoef1 { get; set; } = 290;
                public double BurnCurrentCoef2 { get; set; } = 290;
                public double BurnCurrentCoef3 { get; set; } = 290;
                public double BurnCurrentCoef4 { get; set; } = 290;
                public double BurnCurrentCoef5 { get; set; } = 290;
                public double BurnCurrentCoef6 { get; set; } = 290;
                public double BurnCurrentCoef7 { get; set; } = 290;
            }
        }
        public class ExternalSoftwareParams
        {
            public Paths SoftwarePaths { get; set; } = new Paths();
            public Mods SoftwareMods { get; set; } = new Mods();
            public class Paths
            {
                public string SA7100SoftFilePath { get; set; } = "C:\\Program Files\\CA7100\\hvbridge7100.exe";
                public string SA540SoftFilePath { get; set; } = @"C:\Program Files (x86)\CA540_ETL\CA540_ETL.exe";
                public string SA640SoftFilePath { get; set; } = @"C:\Program Files (x86)\CA640_ETL\CA640_ETL.exe";
                public string VLFSoftFilePath { get; set; } = @"C:\b2CC v3.65 ServicePartner\ControlCenter.App.exe";
                public string LVMeasSoftFilePath { get; set; } = "";
                public string MeasSoftFilePath { get; set; } = "";
            }
            public class Mods
            {
                public SoftMode SA7100Mode { get; set; } = SoftMode.External;
                public SoftMode SA540Mode { get; set; } = SoftMode.External;
                public SoftMode SA640Mode { get; set; } = SoftMode.External;
                public SoftMode VLFMode { get; set; } = SoftMode.External;
                public SoftMode LVMeasMode { get; set; } = SoftMode.None;
                public SoftMode MeasMode { get; set; } = SoftMode.None;
                public enum SoftMode
                {
                    None,
                    External,
                    Internal
                }
                public enum TransformatorVoltage
                {
                    kv_50,
                    kv_100
                }
            }
        }
    }
    public enum ProtocolType
    {
        Default,
        Manual,
        Auto
    }
}
