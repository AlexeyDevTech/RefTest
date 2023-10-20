using ANG24.Core.BaseClasses.Commands;
using ANG24.Core.Enums;
using ANG24.Core.Helpers;
using ANG24.Core.Interfaces;
using Microsoft.VisualBasic;
using System.Diagnostics;

namespace ANG24.Core.BaseClasses
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

        protected virtual void DataReceivedAction(string message)
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
        public virtual Task<bool> WriteCommand(StandardCommand command)
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
        public virtual Task<bool> WriteCommand(ReqResCommand command)
        {
            Thread.Sleep(5);
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
                        command.InvokeCompletedCommand(command);
                        //Debug.WriteLine($"[{command.Command}] - [{message.Trim()}]");

                        r = true;
                        waitHandle.Set();
                    }
                    else
                    {
                        //Debug.WriteLine($"???[{command.Command}] - [{message.Trim()}]???");
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
            if (ChainState == ChainState.Single) return false;

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

            Debug.WriteLine(res ? "======Chain completed======" : "======Chain NOT completed======");
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

