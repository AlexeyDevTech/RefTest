using Prism.Commands;
using Prism.Mvvm;
using Ref.BaseClasses;
using Ref.Controllers.ReflectController;
using Ref.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Threading;

namespace RefTest
{
    public class MainWindowViewModel : BindableBase
    {
        #region Хлам
        private string _command1;
        public string Command1
        {
            get => _command1;
            set => SetProperty(ref _command1, value);
        }

        private string _command2;
        public string Command2
        {
            get => _command2;
            set => SetProperty(ref _command2, value);
        }

        private string _command3;
        public string Command3
        {
            get => _command3;
            set => SetProperty(ref _command3, value);
        }

        private string _command4;
        public string Command4
        {
            get => _command4;
            set => SetProperty(ref _command4, value);
        }

        private string _command5;
        public string Command5
        {
            get => _command5;
            set => SetProperty(ref _command5, value);
        }

        private string _command6;
        public string Command6
        {
            get => _command6;
            set => SetProperty(ref _command6, value);
        }

        private string _command7;
        public string Command7
        {
            get => _command7;
            set => SetProperty(ref _command7, value);
        }

        private string _command8;
        public string Command8
        {
            get => _command8;
            set => SetProperty(ref _command8, value);
        }

        private string _command9;
        public string Command9
        {
            get => _command9;
            set => SetProperty(ref _command9, value);
        }
        #endregion

        #region Хлам2

        private string _commandParameter1;
        public string CommandParameter1
        {
            get => _commandParameter1;
            set => SetProperty(ref _commandParameter1, value);
        }

        private string _commandParameter2;
        public string CommandParameter2
        {
            get => _commandParameter2;
            set => SetProperty(ref _commandParameter2, value);
        }

        private string _commandParameter3;
        public string CommandParameter3
        {
            get => _commandParameter3;
            set => SetProperty(ref _commandParameter3, value);
        }

        private string _commandParameter4;
        public string CommandParameter4
        {
            get => _commandParameter4;
            set => SetProperty(ref _commandParameter4, value);
        }

        private string _commandParameter5;
        public string CommandParameter5
        {
            get => _commandParameter5;
            set => SetProperty(ref _commandParameter5, value);
        }

        private string _commandParameter6;
        public string CommandParameter6
        {
            get => _commandParameter6;
            set => SetProperty(ref _commandParameter6, value);
        }

        private string _commandParameter7;
        public string CommandParameter7
        {
            get => _commandParameter7;
            set => SetProperty(ref _commandParameter7, value);
        }

        private string _commandParameter8;
        public string CommandParameter8
        {
            get => _commandParameter8;
            set => SetProperty(ref _commandParameter8, value);
        }

        private string _commandParameter9;
        public string CommandParameter9
        {
            get => _commandParameter9;
            set => SetProperty(ref _commandParameter9, value);
        }

        #endregion

        #region Props

        public ReflectController120 reflect;
        private ObservableCollection<string> _messages = new();
        public ObservableCollection<string> TypesOfCommand { get; set; } = new() {"Single", "Chain", "ChainAuto"};
        public ObservableCollection<string> Messages 
        {
            get => _messages;
            set => SetProperty(ref _messages, value);
        }
        public ObservableCollection<string> AllCommands { get; set; } = new()
        {
            "SetMode", "SetChannel", "SetAmplitude", "SetDelay", "SetImpulse", "SetPulse", "SetResistance", "GetState"
        };

        public DelegateCommand ConnectCommand { get; set; }
        public DelegateCommand DisconnectCommand { get; set; }
        public DelegateCommand SendChainCommand { get; set; }
        public DelegateCommand ClearCommand { get; set; }

        private string _COM;
        public string COM
        {
            get => _COM;
            set => SetProperty(ref _COM, value);
        }

        private string _currentMode;
        public string CurrentMode
        {
            get => _currentMode;
            set => SetProperty(ref _currentMode, value);
        }

        private string _currentChannel;
        public string CurrentChannel
        {
            get => _currentChannel;
            set => SetProperty(ref _currentChannel, value);
        }

        private string _r;
        public string R
        {
            get => _r;
            set => SetProperty(ref _r, value);
        }

        private string _v;
        public string V
        {
            get => _v;
            set => SetProperty(ref _v, value);
        }

        private string _i;
        public string I
        {
            get => _i;
            set => SetProperty(ref _i, value);
        }

        private bool _isChannel1;
        public bool IsChannel1
        {
            get => _isChannel1;
            set
            {
                if (IsChannel2 || IsChannel3) SetProperty(ref _isChannel1, value);
            }
        }

        private bool _isChannel2;
        public bool IsChannel2
        {
            get => _isChannel2;
            set
            {
                if (!value && !IsChannel1 && !IsChannel3) IsChannel1 = true;
                SetProperty(ref _isChannel2, value);
            }
        }

        private bool _isChannel3;
        public bool IsChannel3
        {
            get => _isChannel3;
            set
            {
                if (!value && !IsChannel2 && !IsChannel1) IsChannel1 = true;
                SetProperty(ref _isChannel3, value);
            }
        }
        


        private bool _isRunChannels;
        public bool IsRunChannels
        {
            get => _isRunChannels;
            set
            {
                SetProperty(ref _isRunChannels, value);

                if (value)
                {
                    worker.RunWorkerAsync();
                }
                else
                {
                    worker.CancelAsync();
                }

            }
        }

        private string _selectedTypeCommand;
        public string SelectedTypeCommand
        {
            get => _selectedTypeCommand;
            set
            {
                SetProperty(ref _selectedTypeCommand, value);
                switch (value)
                {
                    case "Single":
                        reflect.SetChain(ChainState.Single);
                        break;
                    case "Chain":
                        reflect.SetChain(ChainState.Chain);
                        break;
                    case "ChainAuto":
                        reflect.SetChain(ChainState.ChainAuto);
                        break;
                }
            }
        }

        private SynchronizationContext _synchronizationContext;

        #endregion

        BackgroundWorker worker = new();

        public MainWindowViewModel()
        {

            worker.DoWork += Worker_DoWork;
            worker.WorkerSupportsCancellation = true;

            reflect = new ReflectController120()
            {
                Settings = new()
                {
                    Request = "R120#",
                    Response = "R120_OK",
                    BaudRate = 115200
                }
            };

            reflect.OnDataRecieved += Reflect_OnDataRecieved; ;

            COM = "COM23";
            ConnectCommand = new DelegateCommand(Connect);
            DisconnectCommand = new DelegateCommand(Disconnect);
            SendChainCommand = new DelegateCommand(SendChain);
            ClearCommand = new DelegateCommand(ClearItems);
            _synchronizationContext = SynchronizationContext.Current;
        }

        private async void Worker_DoWork(object? sender, DoWorkEventArgs e)
        {
            while (!worker.CancellationPending)
            {
                reflect.SetChannel(1);
                Thread.Sleep(5);
                reflect.GetState();
                Thread.Sleep(5);
                reflect.SetChannel(2);
                Thread.Sleep(5);
                reflect.GetState();
                Thread.Sleep(5);
                reflect.SetChannel(3);
                Thread.Sleep(5);
                reflect.GetState();
                Thread.Sleep(5);

                await reflect.ExecuteChain(50);

                /*                if(IsChannel1 && IsChannel2 && IsChannel3)
                                {
                                    if(CurrentChannel == "1")
                                    {
                                        reflect.SetChannel(2);
                                        Thread.Sleep(5);
                                        reflect.GetState();
                                        Thread.Sleep(5);
                                        reflect.SetChannel(3);
                                        Thread.Sleep(5);
                                        reflect.GetState();
                                        Thread.Sleep(5);
                                    }
                                    if (CurrentChannel == "2")
                                    {
                                        reflect.SetChannel(3);
                                        Thread.Sleep(5);
                                        reflect.GetState();
                                        Thread.Sleep(5);
                                    }
                                    if (CurrentChannel == "3" || CurrentChannel==null)
                                    {
                                        reflect.SetChannel(1);
                                        Thread.Sleep(5);
                                        reflect.GetState();
                                        Thread.Sleep(5);
                                        reflect.SetChannel(2);
                                        Thread.Sleep(5);
                                        reflect.GetState();
                                        Thread.Sleep(5);
                                        reflect.SetChannel(3);
                                        Thread.Sleep(5);
                                        reflect.GetState();
                                        Thread.Sleep(5);
                                    }
                                }

                                await reflect.ExecuteChain(100);

                            }*/
            }

        }

        private void ClearItems()
        {
            Messages.Clear();
        }

        private void Reflect_OnDataRecieved(ReflectData data)
        {
            _synchronizationContext.Post((s) => 
            {

                CurrentChannel = data.CurrentChannel;
                CurrentMode = data.CurrentMode;
                R = data.R;
                V = data.V;
                I = data.I;

                Messages.Add(DateTime.Now.ToString("HH:mm:ss") +" "+data.Message);
            }, null);
                
        }

        private void Connect()
        {
            Messages.Add($"Start reflect: [{reflect.Start()}]");
        }

        private void Disconnect()
        {
            Messages.Add($"Stop reflect: [{reflect.Stop()}]");
        }

        void executeSwitchMethod(string command, string parameter)
        {
            //"SetMode", "SetChannel", "SetAmplitude", "SetDelay", "SetImpulse", "SetPulse", "SetResistance"

            int param = 0;

            if(!string.IsNullOrEmpty(parameter))
                param = int.Parse(parameter);

            switch(command)
            {
                case "SetMode":
                    reflect.SetMode(param);
                    break;
                case "SetChannel":
                    reflect.SetChannel(param);
                    break;
                case "SetAmplitude":
                    reflect.SetAmplitude(param);
                    break;
                case "SetDelay":
                    reflect.SetDelay(param);
                    break;
                case "SetImpulse":
                    reflect.SetImpulse(param);
                    break;
                case "SetPulse":
                    reflect.SetPulse(param);
                    break;
                case "SetResistance":
                    reflect.SetResistance(param);
                    break;
                case "GetState":
                    reflect.GetState();
                    break;
            }
            Thread.Sleep(5);
        }

        private async void SendChain()
        {

            if(!string.IsNullOrEmpty(Command1)) executeSwitchMethod(Command1, CommandParameter1);
            if(!string.IsNullOrEmpty(Command2)) executeSwitchMethod(Command2, CommandParameter2);
            if(!string.IsNullOrEmpty(Command3)) executeSwitchMethod(Command3, CommandParameter3);
            if(!string.IsNullOrEmpty(Command4)) executeSwitchMethod(Command4, CommandParameter4);
            if(!string.IsNullOrEmpty(Command5)) executeSwitchMethod(Command5, CommandParameter5);
            if(!string.IsNullOrEmpty(Command6)) executeSwitchMethod(Command6, CommandParameter6);
            if(!string.IsNullOrEmpty(Command7)) executeSwitchMethod(Command7, CommandParameter7);
            if(!string.IsNullOrEmpty(Command8)) executeSwitchMethod(Command8, CommandParameter8);
            if(!string.IsNullOrEmpty(Command9)) executeSwitchMethod(Command9, CommandParameter9);

            var res = await reflect.ExecuteChain();
            Debug.WriteLine(res ? "Chain completed" : "Chain NOT completed");
        }

    }
}
