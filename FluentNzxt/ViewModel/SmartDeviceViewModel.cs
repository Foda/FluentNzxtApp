using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using NzxtLib;
using RBGLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;

namespace FluentNzxt.ViewModel
{
    public class SmartDeviceViewModel : ObservableObject, IDeviceViewModel
    {
        private SmartDevice _model;

        public IAsyncRelayCommand ApplyCommand { get; }
        public IAsyncRelayCommand FindDeviceCommand { get; }

        private ObservableCollection<Hue2AccessoryViewModel> _accessories = new();
        public ObservableCollection<Hue2AccessoryViewModel> Accessories
        {
            get => _accessories;
            private set => SetProperty(ref _accessories, value);
        }

        public List<IEffectModeViewModel> EffectModes { get; }

        private IEffectModeViewModel _selectedEffectMode;
        public IEffectModeViewModel SelectedEffectMode
        {
            get => _selectedEffectMode;
            set
            {
                SetProperty(ref _selectedEffectMode, value);
            }
        }

        public string Name => _model.Name;
        public string RawName => _model.RawName;

        private bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            private set
            {
                SetProperty(ref _isLoading, value);
                ApplyCommand.NotifyCanExecuteChanged();
            }
        }

        public SmartDeviceViewModel(SmartDevice model)
        {
            _model = model;

            EffectModes = _model.EffectModes.Select(mode =>
            {
                if (mode.MaxColors > 1)
                    return new MultiColorEffectModeViewModel(mode) as IEffectModeViewModel;
                else
                    return new FixedColorEffectModeViewModel(mode) as IEffectModeViewModel;
            }).ToList();

            SelectedEffectMode = EffectModes.First(m => m.Name == "Fixed");

            ApplyCommand = new AsyncRelayCommand(SetDeviceModeAndColor, () => !IsLoading);
            FindDeviceCommand = new AsyncRelayCommand(FindDevice);
        }

        public async Task FindDevice()
        {
            IsLoading = true;

            Accessories.Clear();

            bool didFind = await _model.FindDevice();
            if (didFind)
            {
                foreach (INzxtAccessory accessory in _model.LedAccessories)
                {
                    Accessories.Add(new Hue2AccessoryViewModel(accessory));
                }
            }

            IsLoading = false;
        }

        private async Task SetDeviceModeAndColor()
        {
            bool didApply = await _model.Apply(SelectedEffectMode.GetColors(), SelectedEffectMode.Model);

            if (didApply)
            {
                foreach (Hue2AccessoryViewModel accessory in Accessories)
                {
                    accessory.SetColor(SelectedEffectMode.GetColors().FirstOrDefault());
                }
            }
        }
    }
}
