using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentNzxt.ViewModel
{
    public class MainWindowViewModel : ObservableObject
    {
        private SmartDeviceViewModel _selectedDevice;
        public SmartDeviceViewModel SelectedDevice
        {
            get => _selectedDevice;
            set => SetProperty(ref _selectedDevice, value);
        }

        private ObservableCollection<SmartDeviceViewModel> _devices = new();
        public ObservableCollection<SmartDeviceViewModel> Devices
        {
            get => _devices;
            private set => SetProperty(ref _devices, value);
        }

        public MainWindowViewModel()
        {
            PropertyChanged += async (s, e) =>
            {
                if (e.PropertyName == nameof(SelectedDevice))
                {
                    await SelectedDevice.FindDeviceCommand.ExecuteAsync(null);
                }
            };

            SmartDeviceViewModel device = new SmartDeviceViewModel(new NzxtLib.SmartDevice());
            Devices.Add(device);

            SelectedDevice = device;
        }
    }
}
