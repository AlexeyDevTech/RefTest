using Prism.Commands;
using Prism.Mvvm;
using System.Diagnostics;

namespace RefTest
{
    public class MainWindowViewModel : BindableBase
    {
        public DelegateCommand ConnectCommand { get; set; }
        public DelegateCommand DisconnectCommand { get; set; }
        private string _COM;
        public string COM
        {
            get => _COM;
            set => SetProperty(ref _COM, value);
        }
        public MainWindowViewModel()
        {
            COM = "COM23";
            ConnectCommand = new DelegateCommand(Connect);
            DisconnectCommand = new DelegateCommand(Disconnect);

        }

        private void Connect()
        {
            Debug.WriteLine("Connected");
        }

        private void Disconnect()
        {
            Debug.WriteLine("Connected");
        }

    }
}
