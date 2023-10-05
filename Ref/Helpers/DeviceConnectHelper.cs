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

            //Console.WriteLine($"--ports count = {prts.Count}--");
            //Console.WriteLine();

            foreach (var i in prts)
            {
                if (!success)
                {
                    //Console.WriteLine($"current port: {i.PortName}");
                    i.BaudRate = BaudRate;
                    d.SetPort(i);
                    //Console.WriteLine($"device: port {i.PortName} set. <{d.port.PortName}>");
                    //Console.WriteLine();
                    if (d.Connect())
                    {
                        bool Accept = false;
                        int AttemptCount = 3;
                        d.port?.DiscardInBuffer();
                        d.port?.DiscardOutBuffer();
                        while (AttemptCount > 0 && !Accept)
                        {
                            //Console.WriteLine($"Attempts: {AttemptCount}, check.");
                            if (Check())
                            {
                                //Console.WriteLine($"+Check Accepted+");
                                Accept = true;
                                success = true;
                            }
                            else
                            {
                                //Console.WriteLine($"-Check faulted-");
                                AttemptCount--;
                            }
                            //Console.WriteLine();
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
                    //Console.WriteLine($"<Check> -> Writing [{reqest}]");

                    if (d.Write(reqest))
                    {
                        Thread.Sleep(50);
                        var s = d.Read().Replace('\r', ' ').Replace('\n', ' ').Trim();
                        //Console.WriteLine($"<Check> Reading <- [{s}]");
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

