using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ref
{
    public class RefLogic
    {
    }




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

    public class DeviceConnectHelper
    {
        public DeviceConnectHelper()
        {
            
        }

        public List<SerialPort> GetPortList()
        {
            var ports = new List<SerialPort>();
            var p = SerialPort.GetPortNames();
            foreach(var d in p)
            {
                var s = new SerialPort(d);
                ports.Add(s);
            }

            return ports;
        }

        public Device? FindPort(string reqest, string responce, int BaudRate = 9600)
        {
            var d = new Device();
            var prts = GetPortList();
            var success = false;

            Console.WriteLine($"--ports count = {prts.Count}--");
            Console.WriteLine();

            foreach (var i in prts)
            {
                if (!success)
                {
                    Console.WriteLine($"current port: {i.PortName}");
                    i.BaudRate = BaudRate;
                    d.SetPort(i);
                    Console.WriteLine($"device: port {i.PortName} set. <{d.port.PortName}>");
                    Console.WriteLine();
                    if (d.Connect())
                    {
                        bool Accept = false;
                        int AttemptCount = 3;

                        while (AttemptCount > 0 && !Accept)
                        {
                            Console.WriteLine($"Attempts: {AttemptCount}, check.");
                            if (Check())
                            {
                                Console.WriteLine($"+Check Accepted+");
                                Accept = true;
                                success = true;
                            }
                            else
                            {
                                Console.WriteLine($"-Check faulted-");
                                AttemptCount--;
                            }
                            Console.WriteLine();
                        }
                        d.Disconnect();
                    }
                }
            }
            if (success)
                return d;
            else return null;

            bool Check()
            {
                var r = false;
                try
                {
                    Console.WriteLine($"<Check> -> Writing [{reqest}]");
                    if (d.Write(reqest))
                    {
                        Thread.Sleep(50);
                        var s = d.Read().Replace('\r', ' ').Replace('\n', ' ').Trim();
                        Console.WriteLine($"<Check> Reading <- [{s}]");
                        if (s.Contains(responce)) r = true;

                    }
                }
                catch (InvalidOperationException)
                {
                    r = false;
                }
                return r;
            }
        }

    }

    public interface IBaseDevice
    {

        bool Connect();
        void Disconnect();
        bool Write(string message);
        string Read();
        string ReadLine();
        bool Busy { get; }

        Action<object, EventArgs> DataReceivedAction { get; set; }
    }

    public interface IBaseSerialDevice : IBaseDevice
    {
        SerialPort? port { get; }
        void SetPort(SerialPort? port);
        void SetPort(string portName, int BaudRate = 9600);

    }
}

