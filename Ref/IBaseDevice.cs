namespace Ref
{
    public interface IBaseDevice
    {
        
        bool Connect();
        void Disconnect();
        bool Write(string message);
        string Read();
        string ReadLine();
        bool Busy { get; }

        Action<object, EventArgs> DataReceivedAction { get; set; }
        public event EventHandler OnConnected;
        public event EventHandler OnDisconnected;
    }

}

