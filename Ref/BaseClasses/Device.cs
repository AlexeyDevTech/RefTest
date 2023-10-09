using System.IO.Ports;
using Ref.Interfaces;

namespace Ref.BaseClasses
{
    public class Device : IBaseSerialDevice
    {


        public event EventHandler OnConnected;
        public event EventHandler OnDisconnected;
        public Action<string> DataReceivedAction { get; set; }

        public SerialPort? port { get; set; }

        public bool Busy { get; set; }



        public DeviceReadMode ReadMode { get; private set; }

        public bool IsFinded { get; set; }

        public Device()
        {

        }

        public bool Connect()
        {
            if (port == null) return false;
            if (string.IsNullOrEmpty(port.PortName)) return false;
            try
            {
                if (port.IsOpen) return false;
                port.Open();
                Thread.Sleep(20);
                if (!port.IsOpen) return false;

            }
            catch (Exception)
            {
                return false;
            }
            if (IsFinded)
                ConnectLogic();
            return true;
        }
        public bool Disconnect()
        {
            if (port == null) return false;
            if (string.IsNullOrEmpty(port.PortName)) return false;
            try
            {
                port.Close();
            }
            catch (Exception) { return false; }
            finally
            {
                DisconnectLogic();
            }
            return true;
        }
        public bool Write(string message)
        {
            if (port == null) return false;
            try
            {
                Busy = true;
                port.Write(message);
            }
            catch (InvalidOperationException)
            {
                Disconnect();
                return false;
            }
            catch (TimeoutException)
            {
                return false;
            }
            catch (Exception) { return false; }

            finally
            {
                Busy = false;
            }
            Busy = false;

            return true;
        }
        public string Read()
        {
            var s = string.Empty;
            try
            {
                if (port != null)
                {
                    //вариант чтения до конца
                    s = port.ReadExisting();
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
            return s;
        }
        public string ReadLine()
        {
            var s = string.Empty;
            try
            {
                if (port != null)
                {
                    //вариант чтения до конца строки
                    s = port.ReadLine();
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
            return s;
        }
        public void SetPort(SerialPort? port)
        {
            if (port != null)
            {
                this.port = port;
                this.port.WriteTimeout = 100;
                this.port.ReadTimeout = 100;
            }
        }
        public void SetPort(string portName, int BaudRate = 9600)
        {
            var p = new SerialPort();
            p.PortName = portName;
            p.BaudRate = BaudRate;
            p.WriteTimeout = 100;
            p.ReadTimeout = 100;
            port = p;
        }
        public void SetReadMode(DeviceReadMode mode) => ReadMode = mode;
        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                var message = string.Empty;
                if (port != null && port.BytesToRead > 0)
                {
                    if (ReadMode == DeviceReadMode.Existing)
                        message = port.ReadExisting();
                    if (ReadMode == DeviceReadMode.Line)
                        message = port.ReadLine();

                    DataReceivedAction?.Invoke(message);

                    //Console.WriteLine($"==========|{message.Trim()}|==========");
                }

            }
            catch
            {

            }
        }


        void ConnectLogic()
        {
            if (port != null)
                port.DataReceived += Port_DataReceived;
            IsFinded = true;
            OnConnected?.Invoke(this, new EventArgs());
        }
        void DisconnectLogic()
        {
            if (port != null)
                port.DataReceived -= Port_DataReceived;
            IsFinded = false;
            OnDisconnected?.Invoke(this, new EventArgs());
        }


    }

}


