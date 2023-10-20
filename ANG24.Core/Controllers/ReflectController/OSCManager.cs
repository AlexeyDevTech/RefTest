//using Avalonia.Threading;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ANG24.Core.Controllers.ReflectController
{

    public delegate void ReflectModeChangedEventHandler(ReflectMode mode);
    public delegate void ReflectDataRecivedEventHandler(RawOSCData reflectData);
    public delegate void ReflectTriggerLevelChangedEventHandler();
    public delegate void OSC_K_ChangedEventHandler();
    public delegate void OSC_L_ChangedEventHandler();
    public delegate void OSC_LenMassChangedEventHandler();
    public delegate void OSC_SRChangedEventHandler();
    public delegate void OSC_VoltDivChangedEventHandler();
    public class OSCManager
    {
        private static OSCManager _instance;
        private ReflectMode _reflectMode;
        private int _voltDiv;
        private double _lenght;
        private double _k;
        private int _triggerLevel;
        private int _pulseLen;
        private int _LenMass;
        private object _locker = new object();

        private Queue<TaskElement> Queue { get; set; }
        private Thread QueueManager { get; set; }
        private readonly BackgroundWorker ReflectWorker; //RUD: Obsolete?
        private OSCController OSC { get; set; }
        public static OSCManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new OSCManager();
                }

                return _instance;
            }
        }
        private OSCManager()
        {
            Queue = new Queue<TaskElement>();
            QueueManager = new Thread(QueueProc)
            {
                Name = "OSC_Queue",
                Priority = ThreadPriority.BelowNormal,
                IsBackground = true
            };

            _triggerLevel = 192;
            TriggerLevel = 192; // Зачем?
            _voltDiv = 9;
            VoltDiv = 9;        // Зачем?
            OSC = new OSCController();
            K_Changed += () =>
            {
                if (ReflectMode == ReflectMode.Continuous)
                {
                    PulseLen = PulseLen;
                    Console.WriteLine("PulseLen = {0}", PulseLen);
                }
            };
            L_Changed += () =>
            {
                if (ReflectMode == ReflectMode.Continuous)
                {
                    PulseLen = PulseLen;
                    Console.WriteLine("PulseLen = {0}", PulseLen);
                }
                // PulseLen = PulseLen;
            };
            ReflectModeChanged += mode =>
            {
                TriggerLevel = 192;
                if (mode == ReflectMode.Impulse)
                {
                    PulseLen = PulseLen;
                    Console.WriteLine("PulseLen = {0}", PulseLen);
                }
                else
                {
                    new Timer(state =>
                    {
                        PulseLen = PulseLen;
                        Console.WriteLine("PulseLen = {0}", PulseLen);
                    }).Change(1000, Timeout.Infinite);
                }
            };
            QueueManager.Start();
        }
        public bool BaseObserveMode { get; set; }
        public bool ConnectState => OSC.GetConnectState();
        public ReflectMode ReflectMode
        {
            get => _reflectMode;
            set
            {
                AddAction(() => OSC.ChangeReflectMode(value));
                _reflectMode = value;
                ReflectModeChanged?.Invoke(value);
            }
        }
        public int VoltDiv
        {
            get => _voltDiv;
            set
            {
                if (_voltDiv != value)
                {
                    AddAction(() => OSC.SetVoltDiv(value));
                    _voltDiv = value;
                    VoltDivChanged?.Invoke();
                }
            }
        }
        public double Lenght
        {
            get => _lenght;
            set
            {
                _lenght = value;
                L_Changed?.Invoke();
            }
        }
        public double K
        {
            get => _k;
            set
            {
                _k = value;
                K_Changed?.Invoke();
            }
        }
        public void SetSampleRate(int index_sr)
        {
            AddAction(() => OSC.SetSampleRate(index_sr + 6));
        }
        internal int LenMass
        {
            get => _LenMass;
            set => _LenMass = value;
        }
        public int TriggerLevel
        {
            get => _triggerLevel;
            set
            {
                if (_triggerLevel != value)
                {
                    AddAction(() => OSC.SetTriggerLevel(value));
                    TriggerLevelChanged?.Invoke();
                    _triggerLevel = value;
                }
            }
        }
        public int PulseLen
        {
            get => _pulseLen;
            set
            {

                var v = 0;
                switch (ReflectMode)
                {
                    case ReflectMode.Impulse:
                        _pulseLen = value;
                        v = value / 5;
                        break;

                    case ReflectMode.Continuous:
                        _pulseLen = value;
                        AddAction(() => OSC.SetFreqIDM(CalculatorDDS.FDDS));
                        Thread.Sleep(100);
                        v = (int)CalculatorDDS.M;
                        break;
                    default:
                        _pulseLen = value;
                        v = value / 5;
                        break;
                }
                //if (v == 19) v = 20;
                //if (v == 37) v = 38;
                AddAction(() => OSC.ChangePulseLen(v));
            }
        }
        public int PWave => OSC.GetWave();
        public int PPeriod => OSC.GetPeriod();

        public int StandardSR { get; internal set; }

        private Thread workThread;
        //private bool keepWorking = true;
        private CancellationTokenSource cancelTokenSource;
        private ManualResetEvent OSC_mre = new ManualResetEvent(true);


        private void DoWork(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                OSC_mre.WaitOne();
                if (Queue.Count == 0)
                {
                    AddAction(GetData);
                }
            }

            void GetData()
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                if (ReflectMode == ReflectMode.NoMode) return;

                var result = new RawOSCData();
                var gs = 0;
                try
                {
                    //IntPtr ptr1 = Marshal.AllocCoTaskMem(Marshal.SizeOf(result));                 //создаем пустой указатель с размером как у объекта ReflectData
                    //Marshal.StructureToPtr(result, ptr1, true);                                   //связываемся с указателем[?]
                    switch (ReflectMode)
                    {
                        case ReflectMode.Impulse:
                            result.IntArr = OSC.ReadOSC(out gs).IntArr;
                            break;
                        case ReflectMode.Continuous:
                            result.IntArr = OSC.ReadOSC3(out gs).IntArr;
                            break;
                    }
                    //result = (RawOSCData)Marshal.PtrToStructure(ptr1, typeof(RawOSCData));//возвращаем в указатель значения считаных данных
                    //Marshal.FreeCoTaskMem(ptr1);                                                  // и освообождаем память с указателя
                }
                catch (DllNotFoundException)
                {
                    Debug.WriteLine("Dll-файл сборки не найден(Рефлектометр)");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error: {0} ({1})", ex.Message, ex.Source);
                }
                result.GS = gs;
                // не отсылать пустоту 

                if (result.IntArr[0] != 0)
                {

                    if (ReflectMode == ReflectMode.Impulse
                        && ((gs & 0x02) != 0x02 || (gs & 0x03) != 0x03))
                        return;
                    ReflectDataRecived?.Invoke(result);
                }



            }
        }

        public void StartCollectData()
        {
            cancelTokenSource = new CancellationTokenSource();
            CancellationToken token = cancelTokenSource.Token;

            workThread = new Thread(() => DoWork(token));
            workThread.Start();
            //ReflectWorker.RunWorkerAsync();
        }
        public void PauseCollectData()
        {
            OSC_mre.Reset();
        }
        public void ResumeCollectData()
        {
            OSC_mre.Set();
        }
        public void StopCollectData()
        {
            OSC_mre.Set();
            cancelTokenSource.Cancel();

            // завершение работы потока
            workThread.Join();
            //ReflectWorker.CancelAsync();
        }

        private void ReflectWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = (BackgroundWorker)sender;
            while (!worker.CancellationPending)
            {
                lock(_locker)
                {
                    try
                    {
                        if (Queue.Count == 0)
                            GetData();
                    }
                    catch(Exception ex)
                    {
                        Debug.Write($"OSC error: {ex}");
                    }
                    //Thread.Sleep(100);
                }
            }
            e.Cancel = true;

            void GetData()
            {
                var result = new RawOSCData();
                var gs = 0;
                try
                {
                    //IntPtr ptr1 = Marshal.AllocCoTaskMem(Marshal.SizeOf(result));                 //создаем пустой указатель с размером как у объекта ReflectData
                    //Marshal.StructureToPtr(result, ptr1, true);                                   //связываемся с указателем[?]
                    switch (ReflectMode)
                    {
                        case ReflectMode.Impulse:
                            result.IntArr = OSC.ReadOSC(out gs).IntArr;
                            break;
                        case ReflectMode.Continuous:
                            result.IntArr = OSC.ReadOSC3(out gs).IntArr;
                            break;
                    }
                    //result = (RawOSCData)Marshal.PtrToStructure(ptr1, typeof(RawOSCData));//возвращаем в указатель значения считаных данных
                    //Marshal.FreeCoTaskMem(ptr1);                                                  // и освообождаем память с указателя
                }
                catch (DllNotFoundException)
                {
                    Debug.WriteLine("Dll-файл сборки не найден(Рефлектометр)");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error: {0} ({1})", ex.Message, ex.Source);
                }
                result.GS = gs;
                // не отсылать пустоту 
                
                    if (result.IntArr[0] != 0)
                    {

                        if (ReflectMode == ReflectMode.Impulse
                            && ((gs & 0x02) != 0x02 || (gs & 0x03) != 0x03))
                            return;
                        ReflectDataRecived?.Invoke(result);
                    }
                
                
                
            }

        }

        public bool Init(int DefaultVoltDiv = 9, int DefaultTriggerLevel = 190, int DefaultPulseLen = 40)
        {
            OSC = new OSCController();
            return OSC.Init(DefaultVoltDiv, DefaultTriggerLevel, DefaultPulseLen);
        }

        private void AddAction(Action action)
        {
            Queue.Enqueue(new TaskElement { task = action });
        }
        private void QueueProc()
        {
            while (true)
            {
                if (Queue.Count != 0)
                {
                    try
                    {
                        CancellationToken token = new CancellationToken();
                        var el = Queue.Peek();
                        el.task?.Invoke();
                        //var tsk = new Task(el.task, token);
                        //Stopwatch s = new Stopwatch();
                        //s.Start();
                        //tsk.Start();
                        //tsk.Wait(token);
                        //s.Stop();
                        //Console.WriteLine("Total: Count = {0}, Task Time = {1}", Queue.Count, s.ElapsedMilliseconds);
                        Queue.Dequeue();
                    }
                    catch (Exception ex)
                    {   
                        Debug.WriteLine(ex.Message);
                    }
                }
                Thread.Sleep(50);

            }
        }
        public event ReflectModeChangedEventHandler ReflectModeChanged;
        public event ReflectDataRecivedEventHandler ReflectDataRecived;
        public event OSC_K_ChangedEventHandler K_Changed;
        public event OSC_L_ChangedEventHandler L_Changed;
        public event OSC_VoltDivChangedEventHandler VoltDivChanged;
        public event ReflectTriggerLevelChangedEventHandler TriggerLevelChanged;
        //public event OSC_LenMassChangedEventHandler LenMassChanged;
        //public event OSC_SRChangedEventHandler SRChanged;
    }

    internal struct TaskElement
    {
        public Action task;
    }
}

