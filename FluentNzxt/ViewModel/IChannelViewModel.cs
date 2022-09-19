using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FluentNzxt.ViewModel
{
    public interface IChannelViewModel
    {
        string Name { get; }
        string Channel { get; }
        List<IEffectModeViewModel> EffectModes { get; }
        IEffectModeViewModel SelectedEffectMode { get; set; }
        ObservableCollection<Hue2AccessoryViewModel> Accessories { get; }
        bool HasAccessories { get; }
        void AddAccessory(Hue2AccessoryViewModel accessoryViewModel);
        Task ApplyChangesToDevice();
    }
}
