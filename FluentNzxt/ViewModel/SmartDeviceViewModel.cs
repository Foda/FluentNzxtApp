using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using NzxtLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.UI;

namespace FluentNzxt.ViewModel
{
    public class SmartDeviceViewModel : ObservableObject, IDeviceViewModel
    {
        private SmartDevice _model;

        public IAsyncRelayCommand ApplyCommand { get; }
        public IAsyncRelayCommand FindDeviceCommand { get; }

        public IRelayCommand AddColorCommand { get; }
        public IRelayCommand<int> RemoveColorCommand { get; }

        private Color _color = Color.FromArgb(255, 255, 255, 255);
        public Color Color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }

        private ObservableCollection<Color> _colorSequence = new();
        public ObservableCollection<Color> ColorSequence
        {
            get => _colorSequence;
            set => SetProperty(ref _colorSequence, value);
        }

        private ObservableCollection<Hue2AccessoryViewModel> _accessories = new();
        public ObservableCollection<Hue2AccessoryViewModel> Accessories
        {
            get => _accessories;
            private set => SetProperty(ref _accessories, value);
        }

        public List<INzxtEffectMode> EffectModes => _model.EffectModes;

        private INzxtEffectMode _selectedEffectMode;
        public INzxtEffectMode SelectedEffectMode
        {
            get => _selectedEffectMode;
            set
            {
                SetProperty(ref _selectedEffectMode, value);
                OnPropertyChanged(nameof(CanAddNewColor));
                AddColorCommand?.NotifyCanExecuteChanged();
            }
        }

        public List<Hue2EffectSpeed> EffectSpeeds => _model.EffectSpeeds;

        private Hue2EffectSpeed _selectedEffectSpeed;
        public Hue2EffectSpeed SelectedEffectSpeed
        {
            get => _selectedEffectSpeed;
            set => SetProperty(ref _selectedEffectSpeed, value);
        }

        public string Name => _model.Name;
        public string RawName => _model.RawName;
        public DeviceThumbnail Thumbnail => _model.Thumbnail;

        private bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            private set => SetProperty(ref _isLoading, value);
        }

        public bool CanAddNewColor => ColorSequence.Count < SelectedEffectMode.MaxColors &&
                                      SelectedEffectMode.HasColorModes;

        public SmartDeviceViewModel(SmartDevice model)
        {
            _model = model;

            SelectedEffectMode = EffectModes.First(m => m.Name == "Fixed");
            SelectedEffectSpeed = EffectSpeeds.First(n => n.Name == "Normal");

            ApplyCommand = new AsyncRelayCommand(SetDeviceModeAndColor, () => !IsLoading);
            FindDeviceCommand = new AsyncRelayCommand(FindDevice);

            AddColorCommand = new RelayCommand(() =>
            {
                Random r = new();
                byte[] buffer = new byte[3];
                r.NextBytes(buffer);
                ColorSequence.Add(Color.FromArgb(255, buffer[0], buffer[1], buffer[2]));

                OnPropertyChanged(nameof(CanAddNewColor));
                AddColorCommand.NotifyCanExecuteChanged();
            }, () => CanAddNewColor);

            AddColorCommand.Execute(null);
        }

        public async Task FindDevice()
        {
            IsLoading = true;

            ApplyCommand.NotifyCanExecuteChanged();

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

            ApplyCommand.NotifyCanExecuteChanged();
        }

        private async Task SetDeviceModeAndColor()
        {
            bool didApply = await _model.Apply(1,
                new List<System.Drawing.Color>(
                    ColorSequence.Select(color => System.Drawing.Color.FromArgb(255, color.R, color.G, color.B))
                ), SelectedEffectMode, SelectedEffectSpeed);

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
