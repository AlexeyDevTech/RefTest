using Ref.BaseClasses.Commands;
using Ref.Enums;
using Ref.Helpers;
using Ref.Interfaces;
using System.Diagnostics;

namespace Ref.BaseClasses
{
    public class Controller : IController
    {
        public ControllerState State { get; private set; }
        public ControllerSettings Settings { get; set; } = new();
        public IBaseSerialDevice ControllerDevice { get; private set; } = new Device();
        public Action<ControllerData> OnDataReceivedAction { get; set; }
        private Action<string> DRACommandHandle { get; set; }
        public ChainState ChainState { get; private set; }
        private bool IsExecutingChain;

        private Queue<CommandBase> ChainCommands = new();

        public Controller()
        {
            State = ControllerState.NotInitialized;
        }

        private void DataReceivedAction(string message)
        {
            var data = new ControllerData() { Message = message };
            //Debug.WriteLine(data.Message);
            DRACommandHandle?.Invoke(data.Message);
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

        //public bool WriteCommand(string command)
        //{
        //    var r = false;
        //    if (ChainState == ChainState.Single)
        //    {
        //        if (ControllerDevice.Write(command))
        //        {
        //            r = true;
        //        }
        //    }
        //    else
        //    {
        //        ChainCommands.Enqueue(command);
        //        r = true;
        //    }
        //    return r;
        //}


        //public bool WriteCommand(string command, string responce, int callbackTimeout = 100)
        //{
        //    var r = false;
        //    var waitHandle = new ManualResetEvent(false);


        //    if (ChainState == ChainState.Single)
        //    {
        //        ControllerDevice.DataReceivedAction += HandleResponse;
        //        ControllerDevice.Write(command);

        //        waitHandle.WaitOne(callbackTimeout);

        //        ControllerDevice.DataReceivedAction -= HandleResponse;

        //    }
        //    else
        //    {
        //        ChainCommands.Enqueue(command);
        //        r = true;
        //    }
        //    waitHandle.Dispose();
        //    return r;

        //    void HandleResponse(string message)
        //    {
        //        if (message.Contains(responce))
        //        {
        //            r = true;
        //            waitHandle.Set();
        //        }
        //    }

        //}
        public Task<bool> WriteCommand(StandardCommand command)
        {
            return Task.Run(() =>
            {

                var r = false;
                if (ChainState == ChainState.Single)
                {
                    if (ControllerDevice.Write(command.Command))
                    {
                        r = true;
                    }
                }
                else
                {
                    ChainCommands.Enqueue(command);
                    r = true;
                }
                return r;
            });
        }
        public Task<bool> WriteCommand(ReqResCommand command)
        {
            return Task.Run(() => {
                var r = false;
                var waitHandle = new ManualResetEvent(false);


                if (ChainState == ChainState.Single || command.IsExecuting)
                {
                    //Debug.WriteLine($"Команда на выполнении {command.Command}");
                    DRACommandHandle = HandleResponse;
                    //Debug.WriteLine($"Команда отправлена {command.Command} [{}]");
                    ControllerDevice.Write(command.Command);
                    waitHandle.WaitOne(command.Timeout);

                    DRACommandHandle = null;
                    //Debug.WriteLine($"Ожидание завершено [{r}]");

                }
                else
                {
                    ChainCommands.Enqueue(command);
                    r = true;
                }
                waitHandle.Dispose();
                return r;

                void HandleResponse(string message)
                {
                    if (message.Contains(command.Response))
                    {
                        Debug.WriteLine($"[{command.Command}] - [{message.Trim()}]");
                        //Debug.WriteLine($"==========|{message.Trim()}|==========");

                        r = true;
                        waitHandle.Set();
                    }
                    else
                    {
                        Debug.WriteLine($"???[{command.Command}] - [{message.Trim()}]???");
                    }
                }
            });
            

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
                    SetDRA();
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

        public virtual bool Stop()
        {
            var res = ControllerDevice.Disconnect();
            UnSetDRA();
            ControllerDevice.OnDisconnected -= ControllerDevice_OnDisconnected;
            ControllerDevice.OnConnected -= ControllerDevice_OnConnected;

            State = ControllerState.Stopped;

            return res;
        }

        public void SetChain(ChainState c_state)
        {
            ChainState = c_state;
            Debug.WriteLine($"Chain state: [{ChainState}]");
        }

        public async Task<bool> ExecuteChain(int Delay = 150)
        {
            if (ChainState == ChainState.Single) throw new Exception("Selected not correct state of chain");

            var res = false;

            while (ChainCommands.Count > 0)
            {
                res = false;
                var c = ChainCommands.Peek();
                c.IsExecuting = true;
                if (c is StandardCommand)
                {
                    if (await WriteCommand((StandardCommand)c)) res = true;
                    else break;
                }
                if (c is ReqResCommand)
                {
                    var a = await WriteCommand((ReqResCommand)c);
                    if (a) res = true;
                    else break;
                }
                ChainCommands.Dequeue();

                Thread.Sleep(Delay);

            }
            if (ChainState == ChainState.ChainAuto) SetChain(ChainState.Single);
            ChainCommands.Clear();

            return res;
        }

        private void SetDRA()
        {
            ControllerDevice.DataReceivedAction = DataReceivedAction;
        }
        private void UnSetDRA()
        {
            ControllerDevice.DataReceivedAction = null;
        }
    }

}

