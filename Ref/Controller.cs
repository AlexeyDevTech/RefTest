namespace Ref
{
    public class Controller : IController
    {
        public ControllerState State { get; private set; }
        public ControllerSettings Settings { get; set; } = new();
        public IBaseSerialDevice ControllerDevice { get; private set; } = new Device();
        public Action<ControllerData> OnDataReceivedAction { get; set; }

        public Controller()
        {
            State = ControllerState.NotInitialized;
        }

        private void DataReceivedAction(string message)
        {
            var data = new ControllerData() { Message = message };
            OnDataReceivedAction?.Invoke(data);
        }

        private void ControllerDevice_OnConnected(object? sender, EventArgs e)
        {
            //post-action
        }

        private void ControllerDevice_OnDisconnected(object? sender, EventArgs e)
        {
            //post-action
        }

        public bool SetCommand(string command)
        {
            var r = false;
            if (ControllerDevice.Write(command))
            {
                r = true;
            }
            return r;
        }




        public virtual bool Start()
        {
            var r = false;
            var device = DeviceConnectHelper.Instance.FindPort(Settings.Request, Settings.Response, Settings.BaudRate);
            if (device != null)
            {
                if (device.Connect())
                {
                    ControllerDevice = device;
                    ControllerDevice.DataReceivedAction = DataReceivedAction;
                    r = true;
                }
            }
            if (r)
            {
                ControllerDevice.OnDisconnected += ControllerDevice_OnDisconnected;
                ControllerDevice.OnConnected += ControllerDevice_OnConnected;
                State = ControllerState.Started;
            }
            return r;

        }

        public virtual void Stop()
        {
            ControllerDevice.Disconnect();
            ControllerDevice.DataReceivedAction = null;
            ControllerDevice.OnDisconnected -= ControllerDevice_OnDisconnected;
            ControllerDevice.OnConnected -= ControllerDevice_OnConnected;

            State = ControllerState.Stopped;
        }
    }
    public interface IController
    {
        ControllerState State { get; }
        ControllerSettings Settings { get; set; }
        IBaseSerialDevice ControllerDevice { get; }
        bool Start();
        void Stop();
        bool SetCommand(string command);

        Action<ControllerData> OnDataReceivedAction { get; set; }
    }
}

