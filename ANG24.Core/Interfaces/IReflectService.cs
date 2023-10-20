using ANG24.Core.Controllers.ReflectController;
using ANG24.Core.Controllers.ReflectController.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ANG24.Core.Interfaces
{
    public interface IReflectService
    {
        public int VoltDiv { get; set; }
        public int TriggerLevel { get; set; }
        public int PulseLen { get; set; }
        public double K { get; set; }
        public double L { get; set; }
        public double StandardSR { get;}


        event Action<ReflectOSCData> OnDataFromOscReceived;
        event Action<ReflectData> OnDataFromReflectReceived;
        event Action<bool> OnStartReflect;
        event Action<bool> OnStopReflect;

        public bool IsConnected { get; set; }

        List<Channels> channels { get; set; }
        public void ChangeStateOfChannel(Channels channel, bool state);
        public void SetVersion(VersionReflect version);
        public VersionReflect GetVersion();
        public Task ChangeMode(Mode mode);
        public Mode GetCurrentMode(Mode mode);
        /// <summary>
        /// Запускает поиск рефлектометра, сбрасывает режим на default, инициализирует осцилограф, закрывает порт рефлектометра
        /// Run() => [StateChain = Chain] => ChangeMode() => OSC_Instance => Stop() ...
        /// </summary>
        public void Init();
        /// <summary>
        /// Запускает поиск рефлектометра Start() => ...
        /// </summary>
        public void Run();
        /// <summary>
        /// Сбрасывает режим на default, закрывает порт рефлектометра, 
        /// ChangeMode() => Stop()
        /// </summary>
        public void Stop();

        public void OSC_Pause();
        public void OSC_Resume();

        public void SetAmplitude(Amplitude amplitude);
        public void SetResistance(Resistance resistance);
        public void SetImpulse(int impulse);
        public Task<int> SetImpulseAsync(int impulse);


    }
}
