namespace Ref
{
    public interface IBaseDevice
    {

        DeviceReadMode ReadMode { get; }
        bool Connect();
        void Disconnect();
        bool Write(string message);
        string Read();
        string ReadLine();
        void SetReadMode(DeviceReadMode mode);

        bool Busy { get; }

        Action<string> DataReceivedAction { get; set; }
        public event EventHandler OnConnected;
        public event EventHandler OnDisconnected;
    }

    public enum DeviceReadMode : int
    {
        Line = 0,
        Existing = 1
    }

}

