using System.Runtime.InteropServices;

namespace Ref
{
    internal class OSCController
    {
        public void ChangePulseLen(int PulseLen)
        {
            NativeMethods.ChangePulseLenDLL(PulseLen);
        }
        public void ChangeReflectMode(ReflectMode mode)
        {
            switch (mode)
            {
                case ReflectMode.Impulse:
                    NativeMethods.SetREFDll();
                    break;
                case ReflectMode.Continuous:
                    NativeMethods.SetIDMDLL(CalculatorDDS.FDDS, GetWave(), GetPeriod());
                    break;
            }
        }
        public bool GetConnectState()
        {
            short ver = 0;
            ver = NativeMethods.GetDeviceVersion();
            return ver != 0;

        }
        public int GetPeriod()
        {
            ushort Period = 0;
            Period = NativeMethods.GetPeriodDll();
            return Period;
        }
        public int GetWave()
        {
            ushort pWave = 0;
            pWave = NativeMethods.GetWaveDll();
            return pWave;
        }
        public bool Init(int DefaultVolDiv = 8,
                         int DefaultTriggerLevel = 190,
                         int DefaultPulseLen = 200)
        {
            bool res = false;
            res = NativeMethods.InitDLL(6, DefaultVolDiv, DefaultTriggerLevel, DefaultPulseLen);
            return res;
        }
        public ReflectDataStruct ReadOSC(out int gs)
        {
            gs = 0;
            ReflectDataStruct result = new ReflectDataStruct();
            try
            {
                ReflectDataStruct reflectData = new ReflectDataStruct();                                 //создаем объект структуры ReflectData
                IntPtr ptr1 = Marshal.AllocCoTaskMem(Marshal.SizeOf(reflectData));                       //создаем пустой указатель с размером как у объекта ReflectData
                Marshal.StructureToPtr(reflectData, ptr1, true);                                         //связываемся с указателем[?]
                gs = NativeMethods.ReadOSCDLL(ptr1);                                                 //возвращаем в указатель значения считаных данных
                reflectData = (ReflectDataStruct)Marshal.PtrToStructure(ptr1, typeof(ReflectDataStruct));//
                result = reflectData;
                Marshal.FreeCoTaskMem(ptr1);
            }
            catch (DllNotFoundException)
            {
                Console.WriteLine("Dll-файл сборки не найден(Рефлектометр)");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0} ({1})", ex.Message, ex.Source);
            }
            return result;
        }
        public ReflectDataStruct ReadOSC3(out int gs)
        {
            ReflectDataStruct result = new ReflectDataStruct();
            gs = 0;
            try
            {
                ReflectDataStruct reflectData = new ReflectDataStruct();                                 //создаем объект структуры ReflectData
                IntPtr ptr1 = Marshal.AllocCoTaskMem(Marshal.SizeOf(reflectData));                       //создаем пустой указатель с размером как у объекта ReflectData
                Marshal.StructureToPtr(reflectData, ptr1, true);                                         //связываемся с указателем[?]
                gs = NativeMethods.ReadOSC3DLL(ptr1);                                                //возвращаем в указатель значения считаных данных
                reflectData = (ReflectDataStruct)Marshal.PtrToStructure(ptr1, typeof(ReflectDataStruct));//
                result = reflectData;
                Marshal.FreeCoTaskMem(ptr1);                                                             // и освообождаем память с указателя


            }
            catch (DllNotFoundException)
            {
                Console.WriteLine("Dll-файл сборки не найден(Рефлектометр)");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0} ({1})", ex.Message, ex.Source);
            }
            return result;
        }
        public void SetFreqIDM(double freq)
        {
            NativeMethods.SetFreqIDMDll(freq);
        }
        public void SetSampleRate(int SRIndex)
        {
            NativeMethods.SetSampleRateDLL(SRIndex);
        }
        public void SetTriggerLevel(int TriggerLevel)
        {

            NativeMethods.SetTriggerLevelDLL(TriggerLevel);

        }
        public void SetVoltDiv(int voidDivIndex)
        {
            NativeMethods.SetVoltDivDLL(voidDivIndex);
        }
    }
   
}

