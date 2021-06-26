using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using NzxtLib;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI;

namespace FluentNzxt.ViewModel
{
    public class SmartDeviceViewModel : ObservableObject
    {
        private SmartDevice _model;

        public IAsyncRelayCommand ApplyCommand { get; }
        public IAsyncRelayCommand FindDeviceCommand { get; }

        private Color _color;
        public Color Color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }

        private ObservableCollection<Hue2AccessoryViewModel> _accessories;
        public ObservableCollection<Hue2AccessoryViewModel> Accessories
        {
            get => _accessories;
            private set => SetProperty(ref _accessories, value);
        }

        public List<Hue2EffectMode> EffectModes => _model.EffectModes;

        public string Name => _model.Name;

        private bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            private set => SetProperty(ref _isLoading, value);
        }

        public SmartDeviceViewModel(SmartDevice model)
        {
            _model = model;

            Accessories = new ObservableCollection<Hue2AccessoryViewModel>();

            ApplyCommand = new AsyncRelayCommand(SetDeviceModeAndColor, () => !IsLoading);
            FindDeviceCommand = new AsyncRelayCommand(FindDevice);
        }

        public async Task FindDevice()
        {
            IsLoading = true;

            ApplyCommand.NotifyCanExecuteChanged();

            Accessories.Clear();

            bool didFind = await _model.FindDevice();
            if (didFind)
            {
                foreach (Hue2Accessory accessory in _model.LedAccessories)
                {
                    Accessories.Add(new Hue2AccessoryViewModel(accessory));
                }
            }

            IsLoading = false;

            ApplyCommand.NotifyCanExecuteChanged();
        }

        private async Task SetDeviceModeAndColor()
        {
            bool didApply = await _model.ApplyFixedColor(
                System.Drawing.Color.FromArgb(255, Color.R, Color.G, Color.B));

            if (didApply)
            {
                foreach (Hue2AccessoryViewModel accessory in Accessories)
                {
                    accessory.SetColor(Color);
                }
            }
        }
    }
}
