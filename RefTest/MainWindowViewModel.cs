using Prism.Mvvm;
using Ref;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefTest
{
    public class MainWindowViewModel : BindableBase
    {
        private Device _refDevice;

        public Device RefDevice
        {
            get => _refDevice;
            set => SetProperty(ref _refDevice, value);
        }

        public MainWindowViewModel()
        {
            RefDevice = new Device();
        }


    }
}
