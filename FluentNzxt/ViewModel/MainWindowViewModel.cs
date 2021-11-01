using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FluentNzxt.ViewModel
{
    public class MainWindowViewModel : ObservableObject
    {
        private IDeviceViewModel _selectedDevice;
        public IDeviceViewModel SelectedDevice
        {
            get => _selectedDevice;
            set => SetProperty(ref _selectedDevice, value);
        }

        private ObservableCollection<IDeviceViewModel> _devices = new();
        public ObservableCollection<IDeviceViewModel> Devices
        {
            get => _devices;
            private set => SetProperty(ref _devices, value);
        }

        public MainWindowViewModel()
        {
        }

        public async Task FindDevices()
        {
            List<IDeviceViewModel> devices = await DeviceFinder.FindDevices();
            foreach (IDeviceViewModel device in devices)
            {
                Devices.Add(device);
            }

            SelectedDevice = Devices.FirstOrDefault();
            await SelectedDevice.FindDeviceCommand.ExecuteAsync(null);
        }
    }
}
