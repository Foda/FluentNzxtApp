using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentNzxt.ViewModel
{
    public interface IDeviceViewModel : INotifyPropertyChanged
    {
        IAsyncRelayCommand ApplyCommand { get; }
        IAsyncRelayCommand FindDeviceCommand { get; }
        List<IEffectModeViewModel> EffectModes { get; }
        IEffectModeViewModel SelectedEffectMode { get; set; }
        bool HasAccessories { get; }
        string Name { get; }
        string RawName { get; }
        bool IsLoading { get; }

        // TODO: fix
        ObservableCollection<Hue2AccessoryViewModel> Accessories { get; }

    }
}
