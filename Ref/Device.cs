using System.IO.Ports;

namespace Ref
{
    public class Device : IBaseSerialDevice
    {


        public event EventHandler OnConnected;
        public event EventHandler OnDisconnected;
        public Action<object, EventArgs> DataReceivedAction { get; set; }

        public SerialPort? port { get; set; }

        public bool Busy { get; set; }

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
                if(!port.IsOpen) return false;

            }
            catch (Exception) { 
                return false; 
            }
            ConnectLogic();
            return true;
        }
        public void Disconnect()
        {
            if (port == null) return;
            if (string.IsNullOrEmpty(port.PortName)) return;
            try
            {
                port.Close();
            }
            catch (Exception) { return; }
            finally
            {
                DisconnectLogic();
            }
        }
        public bool Write(string message)
        {
            if (port == null) return false;
            try
            {
                Busy = true;
                port.Write(message);
            } 
            catch(InvalidOperationException)
            {
                Disconnect();
                return false;
            }
            catch(TimeoutException)
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
            this.port = port;
            port.WriteTimeout = 100;
            port.ReadTimeout = 100;
            port.DataReceived += Port_DataReceived;
        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e) => DataReceivedAction?.Invoke(sender, e);

        public void SetPort(string portName, int BaudRate = 9600)
        {
            var p = new SerialPort();
            p.PortName = portName;
            p.BaudRate = BaudRate;
            port = p;
            port.DataReceived += Port_DataReceived;
        }

        void ConnectLogic()
        {
            if(port != null)
                port.DataReceived += Port_DataReceived;
            OnConnected?.Invoke(this, new EventArgs());
        }
        void DisconnectLogic()
        {
            if (port != null)
                port.DataReceived -= Port_DataReceived;
            OnDisconnected?.Invoke(this, new EventArgs());
        }
    }

}

