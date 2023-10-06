using Prism.Commands;
using Prism.Mvvm;
using Ref.BaseClasses;
using Ref.Controllers.ReflectController;
using Ref.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;

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

        public ReflectController120 reflect;
        private ObservableCollection<string> _messages = new();
        public ObservableCollection<string> Messages 
        {
            get => _messages;
            set => SetProperty(ref _messages, value);
        }
        public ObservableCollection<string> AllCommands { get; set; } = new()
        {
            "SetMode", "SetChannel", "SetAmplitude", "SetDelay", "SetImpulse", "SetPulse", "SetResistance"
        };

        public DelegateCommand ConnectCommand { get; set; }
        public DelegateCommand DisconnectCommand { get; set; }
        public DelegateCommand SendChainCommand { get; set; }

        private string _COM;
        public string COM
        {
            get => _COM;
            set => SetProperty(ref _COM, value);
        }
        public MainWindowViewModel()
        {

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

        }

        private void Reflect_OnDataRecieved(ReflectData data)
        {
            Messages.Add(data.Message);
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
            var param = int.Parse(parameter);
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
            }
            Thread.Sleep(5);
        }

        private async void SendChain()
        {
            reflect.SetChain(ChainState.ChainAuto);

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
