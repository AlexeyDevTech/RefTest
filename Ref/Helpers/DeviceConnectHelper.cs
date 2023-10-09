using System.Diagnostics;
using System.IO.Ports;
using Ref.BaseClasses;

namespace Ref.Helpers
{
    public class DeviceConnectHelper
    {

        public static DeviceConnectHelper Instance { get; set; } = new();
        private DeviceConnectHelper()
        {

        }

        public List<SerialPort> GetPortList()
        {
            var ports = new List<SerialPort>();
            var p = SerialPort.GetPortNames();
            foreach (var d in p)
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

            Debug.WriteLine($"--ports count = {prts.Count}--");
            Debug.WriteLine("");

            foreach (var i in prts)
            {
                if (!success)
                {
                    Debug.WriteLine($"current port: {i.PortName}");
                    i.BaudRate = BaudRate;
                    d.SetPort(i);
                    Debug.WriteLine($"device: port {i.PortName} set. <{d.port.PortName}>");
                    Debug.WriteLine("");
                    if (d.Connect())
                    {
                        bool Accept = false;
                        int AttemptCount = 3;
                        d.port?.DiscardInBuffer();
                        d.port?.DiscardOutBuffer();
                        while (AttemptCount > 0 && !Accept)
                        {
                            Debug.WriteLine($"Attempts: {AttemptCount}, check.");
                            if (Check())
                            {
                                Debug.WriteLine($"+Check Accepted+");
                                Accept = true;
                                success = true;
                            }
                            else
                            {
                                Debug.WriteLine($"-Check faulted-");
                                AttemptCount--;
                            }
                            Debug.WriteLine("");
                        }
                        d.Disconnect();

                    }
                }
            }
            if (success)
            {
                d.IsFinded = true;
                return d;
            }
            else return null;

            bool Check()
            {
                var r = false;
                try
                {
                    Debug.WriteLine($"<Check> -> Writing [{reqest}]");

                    if (d.Write(reqest))
                    {
                        Thread.Sleep(50);
                        var s = d.Read().Replace('\r', ' ').Replace('\n', ' ').Trim();
                        Debug.WriteLine($"<Check> Reading <- [{s}]");
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

}

