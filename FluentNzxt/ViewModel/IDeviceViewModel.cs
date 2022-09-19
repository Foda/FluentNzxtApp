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
        IAsyncRelayCommand FindDeviceCommand { get; }

        string Name { get; }
        string RawName { get; }
        bool IsLoading { get; }

        ObservableCollection<IChannelViewModel> Channels { get; }
        IAsyncRelayCommand ApplyCommand { get; }
    }
}
