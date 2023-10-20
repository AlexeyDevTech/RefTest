using ANG24.Core.Controllers.ReflectController.Enums;
using ANG24.Core.Enums;
using ANG24.Core.Helpers;
using ANG24.Core.Interfaces;
using Autofac;
//using DynamicData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ANG24.Core.Controllers.ReflectController
{

    public class ReflectService : IReflectService, IOSCParameters
    {
        public event K_ChangedEventHandler K_Changed;
        public event L_ChangedEventHandler L_Changed;
        public event SR_ChangedEventHandler SR_Changed;
        public event LenMass_ChangedEventHandler LenMass_Changed;
        public event K_TriggeredControlEventHandler K_TriggeredControl;

        public event Action<ReflectOSCData> OnDataFromOscReceived;
        public event Action<ReflectData> OnDataFromReflectReceived;
        public event Action<bool> OnStartReflect;
        public event Action<bool> OnStopReflect;

        // Reflect
        // OSCManager
        private IReflectController ReflectController;
        private IReflectSmoothService ReflectSmoothService;
        private OSCManager OSCManager;

        private BackgroundWorker ChannelWorker;
        private VersionReflect reflectVersion;
        private readonly ReflectDataPrepare reflectDataPrepare;
        public bool IsConnected { get; set; }

        public List<Channels> channels { get; set; } = new();

        public void ChangeStateOfChannel(Channels channel, bool state)
        {

            if (state)
            {
                channels.Add(channel);
            }
            else
            {
                channels.Remove(channel);
            }

            if (channels.Count==0)
            {
                channels.Add(Channels.Channel1);
            }
        }

        private Mode _currentMode;
        private Mode CurrentMode
        {
            get => _currentMode;
            set
            {
                if( _currentMode == Mode.Impulse && value != Mode.Impulse)
                {
                    channels.Clear();
                    channels.Add(Channels.Channel1);
                }
                _currentMode = value;
            }
        }
        private bool IsChainWorking;

        ManualResetEvent ChainResetEvent = new(false);

        private async void ChannelWorker_DoWork(object? sender, DoWorkEventArgs e)
        {
            while (!ChannelWorker.CancellationPending)
            {

                try
                {
                    ChainResetEvent.WaitOne();

                    if (CurrentMode == Mode.Impulse)
                    {

                        if (channels.Count > 1)
                        {
                            for (int i = 0; i < channels.Count; i++)
                            {
                                ReflectController.SetChannel(channels[i]);
                                Thread.Sleep(10);
                                reflectDataPrepare.CurrentChannel = channels[i];
                                ReflectController.GetState();
                                Thread.Sleep(25);

                            }
                        }
                        else if (channels.Count == 1)
                        {
                            if (ReflectData.CurrentChannel != ((int)channels[0]))
                            {
                                ReflectController.GetState();
                                ReflectController.SetChannel(channels[0]);
                                reflectDataPrepare.CurrentChannel = channels[0];
                            }
                        }

                    }

                    Thread.Sleep(25);
                }
                catch
                {

                }

            }
        }

        #region OSC PARAMETERS
        public int VoltDiv { get => OSCManager.VoltDiv; set => OSCManager.VoltDiv = value; }
        public int TriggerLevel { get => OSCManager.TriggerLevel; set => OSCManager.TriggerLevel = value; }

        public double K
        {
            get => OSCManager.K;
            set
            {
                OSCManager.K = value;
                reflectDataPrepare.Length = (int)(L * 10 * (K / 1.5));
                reflectDataPrepare.LenMas = OSCManager.LenMass;
                K_Changed?.Invoke();
                SetSampleRate();
            }
        }

        public double L
        {
            get => OSCManager.Lenght; set
            {
                OSCManager.Lenght = value;
                reflectDataPrepare.Length = (int)(value * 10 * (K / 1.5));
                reflectDataPrepare.LenMas = OSCManager.LenMass;
                L_Changed?.Invoke();
                SetSampleRate();
            }
        }

        private double FactSR
        {
            get
            {
                if (K == 0 && L == 0)
                {
                    return (int)SampleRate.Hz_1G;
                }

                return 65536 / (L / ((3 * Math.Pow(10, 8)) / (2 * K)));
            }
        }

        private int IndexSR
        {
            get
            {
                int result = 0;
                try
                {
                    var arr = Enum.GetValues(typeof(SampleRate));
                    Array.Reverse(arr);
                    if (FactSR >= (int)arr.GetValue(0))
                    {
                        return 0;
                    }

                    for (int i = 0; i < arr.Length - 1; i++)
                    {
                        if ((int)arr.GetValue(i) > FactSR && (int)arr.GetValue(i + 1) < FactSR)
                        {
                            result = i + 1;
                            break;
                        }
                    }
                }
                catch (Exception) { }
                return result;
            }
        }

        private double oldStandardSR = 0.0;
        public double StandardSR
        {
            get
            {
                var arr = Enum.GetValues(typeof(SampleRate));
                Array.Reverse(arr);
                var f = (int)arr.GetValue(IndexSR);
                OSCManager.Instance.StandardSR = f;
                return f;
            }
        }

        public int LenMass
        {
            get
            {
                var freqD = StandardSR;
                var len = (int)((L * 2 * K * freqD) / (3 * Math.Pow(10, 8)));
                if (len == 0 || len > 65535)
                {
                    len = 65535;
                }
                OSCManager.LenMass = len;
                return len;
            }
        }

        private int _pulseLen;
        public int PulseLen
        {
            get
            {
                if (reflectVersion == VersionReflect.Reflect90) return OSCManager.PulseLen;
                else return _pulseLen;
            }
            set
            {
                if (_pulseLen != value)
                {
                    if (reflectVersion == VersionReflect.Reflect90) _pulseLen = OSCManager.PulseLen = value;
                    else SetPulseLenValue(value);
                }
            }
        }
        private void SetPulseLenValue(int value)
        {
            Task.Run(async() => _pulseLen = await SetImpulseAsync(value));
        }
        public bool IsCommandsExecuting { get; private set; }

        private void SetSampleRate()
        {
            LenMass_Changed?.Invoke();
            if (oldStandardSR != StandardSR)
            {
                OSCManager.SetSampleRate(IndexSR);
                SR_Changed?.Invoke();

                oldStandardSR = StandardSR;
            }
        }

        public void K_Triggered()
        {
            K_TriggeredControl?.Invoke();
        }
        #endregion

        public ReflectService(ILifetimeScope scope)
        {
            reflectVersion = VersionReflect.Reflect120;
            ReflectController = scope.ResolveNamed<IReflectController>(reflectVersion.ToString());
            ReflectSmoothService = scope.Resolve<IReflectSmoothService>();

            ChannelWorker = new BackgroundWorker() { WorkerSupportsCancellation = true };
            ChannelWorker.DoWork += ChannelWorker_DoWork;

            reflectDataPrepare = new ReflectDataPrepare
            {
                _Channels = channels // ссылка
            };

            channels.Add(Channels.Channel1);
        }

        private void OSCManager_ReflectDataRecived(RawOSCData data)
        {
            /*Task.Factory.StartNew(() =>
            {*/
                var channel = (Channels)Convert.ToInt32(ReflectData.CurrentChannel);
                ushort[] _data = { 0 };

                if (ReflectSmoothService.Enabled)
                {
                    _data = ReflectSmoothService.AddAndGetResult(data.IntArr, (int)channel);
                }

                var reflectData = reflectDataPrepare.GetChannelsData(ReflectSmoothService.Enabled ? _data :data.IntArr, out bool isDataUpdated);
                if (/*isDataUpdated*/true) //isDataUpdated? Для чего, на**я...
                {
                    //Debug.WriteLine("ReflectChannel = " + CurrentChannel);
                    OnDataFromOscReceived?.Invoke(new ReflectOSCData
                    {
                        RAWData = data.IntArr,
                        ReflectData = reflectData,
                        Gs = data.GS, // хз зачем клиенту GS, лучше убарть при не необходимости
                        Lehght = OSCManager.LenMass, // (длина массива) тоже хз зачем, т.к. это число - Х последнего элемента массива любого из каналов
                        Channel = channel// та же проблема, все каналы доступны в ReflectData
                    });
                }

            /*});*/
        }

        private void ReflectController_OnDataRecieved(ReflectData data)
        {
            OnDataFromReflectReceived?.Invoke(data);
        }

        public async Task ChangeMode(Mode mode)
        {
            await Task.Run(async () =>
            {
                IsChainWorking = true;

                if (mode == Mode.Impulse) ChannelWorker.RunWorkerAsync();
                else ChannelWorker.CancelAsync();

                Thread.Sleep(300);

                channels.Clear();

                ReflectController.SetChain(ChainState.ChainAuto);

                switch (mode)
                {
                    case Mode.Default:
                        ReflectController.SetMode(Mode.Default);
                        ReflectController.SetChannel(Channels.ChannelDefault);
                        //OSCManager.ReflectMode = ReflectMode.NoMode;
                        break;
                    case Mode.Impulse:
                        ReflectController.SetMode(Mode.Impulse);
                        ReflectController.SetChannel(Channels.Channel1);
                        reflectDataPrepare.CurrentChannel = Channels.Channel1;
                        ReflectController.SetAmplitude(Amplitude.V30);
                        ReflectController.SetResistance(Resistance.Ohm_50);
                        ReflectController.SetImpulse(100);
                        OSCManager.ReflectMode = ReflectMode.Impulse;
                        channels.Add(Channels.Channel1);
                        break;
                    case Mode.IDM:
                        ReflectController.SetMode(Mode.IDM);

                        reflectDataPrepare.CurrentChannel = Channels.IDMChannel;
                        channels.Add(Channels.IDMChannel);

                        if (reflectVersion == VersionReflect.Reflect90)
                        {
                            ReflectController.SetChannel(Channels.IDMChannel);
                            ReflectController.SetPulse(30);
                        }
                        ReflectController.SetDelay(3);
                        OSCManager.ReflectMode = ReflectMode.Continuous;
                        break;
                    case Mode.ICE:
                        reflectDataPrepare.CurrentChannel = Channels.ICEChannel;
                        channels.Add(Channels.ICEChannel);

                        ReflectController.SetMode(Mode.ICE);
                        ReflectController.SetChannel(Channels.ICEChannel);
                        ReflectController.SetResistance(Resistance.Ohm_600);

                        OSCManager.ReflectMode = ReflectMode.Continuous;

                        break;
                    case Mode.Decay:
                        reflectDataPrepare.CurrentChannel = Channels.DecayChannel;
                        channels.Add(Channels.DecayChannel);

                        ReflectController.SetMode(Mode.Decay);
                        ReflectController.SetChannel(Channels.DecayChannel);

                        OSCManager.ReflectMode = ReflectMode.Continuous;

                        break;
                }

                await ReflectController.ExecuteChain();
                

                CurrentMode = mode;
                IsChainWorking = false;
            });
        }

        public Mode GetCurrentMode(Mode mode) => CurrentMode;

        public VersionReflect GetVersion() => reflectVersion;

        public void Init()
        {
            Task.Run(async() =>
            {
                if (ReflectController.Start())
                {
                    await ChangeMode(Mode.Default);

                    //OSCManager = OSCManager.Instance;

                    ReflectController.Stop();
                }
            });
        }

        public void OSC_Start()
        {
            OSCManager.StartCollectData();
        }

        public void OSC_Stop()
        {
            OSCManager.StopCollectData();
        }

        public void OSC_Pause()
        {
            OSCManager.PauseCollectData();
        }

        public void OSC_Resume()
        {
            OSCManager.ResumeCollectData();
        }

        public void Run()
        {
            Task.Run(() =>
            {
                bool res;
                OSCManager = OSCManager.Instance;
                OSCManager.Init();

                K = 1.87;
                L = 1000;
                var a = LenMass;

                ReflectController.OnDataRecieved += ReflectController_OnDataRecieved;
                OSCManager.ReflectDataRecived += OSCManager_ReflectDataRecived;

                res = ReflectController.Start();
                OnStartReflect?.Invoke(res);
                IsConnected = res;

                OSC_Start();

                ChainResetEvent.Set();
            });
        }

        public void SetVersion(VersionReflect version)
        {
            reflectVersion = version;
            Debug.WriteLine($"Версия рефлектометра - {version}");
        }

        public void Stop()
        {
            Task.Run(async() =>
            {
                bool res;
                ChainResetEvent.Reset();

                Thread.Sleep(200);

                ReflectController.OnDataRecieved -= ReflectController_OnDataRecieved;
                OSCManager.ReflectDataRecived -= OSCManager_ReflectDataRecived;

                await ChangeMode(Mode.Default);

                res = ReflectController.Stop();
                OnStopReflect?.Invoke(res);
                IsConnected = res ? false : true;

                OnStopReflect?.Invoke(res);

                OSC_Stop();

                ChainResetEvent.Set();
            });
        }

        public void SetResistance(Resistance resistance)
        {
            Task.Run(() =>
            {
                ChainResetEvent.Reset();

                Thread.Sleep(350);

                ReflectController.SetResistance(resistance);

                ChainResetEvent.Set();
            });

        }

        public void SetImpulse(int impulse)
        {
            Task.Run(() =>
            {
                ChainResetEvent.Reset();

                Thread.Sleep(350);

                ReflectController.SetImpulse(impulse);

                ChainResetEvent.Set();
            });
        }

        public async Task<int> SetImpulseAsync(int impulse)
        {
            
            return await Task.Run(() =>
            {
                int res = 0;
                ChainResetEvent.Reset();
                Thread.Sleep(350);

                ReflectController.SetImpulse(impulse, out res);

                ChainResetEvent.Set();
                return res;
            });

        }

        public void SetAmplitude(Amplitude amplitude)
        {
            Task.Run(() =>
            {
                ChainResetEvent.Reset();

                Thread.Sleep(350);

                ReflectController.SetAmplitude(amplitude);

                ChainResetEvent.Set();
            });
        }

    }
}
