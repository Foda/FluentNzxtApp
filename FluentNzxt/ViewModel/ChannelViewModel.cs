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
    internal class ChannelViewModel : ObservableObject, IChannelViewModel
    {
        private SmartDevice _model;

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
            set => SetProperty(ref _selectedEffectMode, value);
        }

        public bool HasAccessories
        {
            get { return Accessories.Count > 0; }
        }

        public string Channel { get; }
        public string Name { get { return $"Channel {Channel}"; } }

        public ChannelViewModel(SmartDevice model, string channel)
        {
            _model = model;
            Channel = channel;

            EffectModes = _model.EffectModes.Select(mode =>
            {
                return mode.MaxColors > 1
                    ? new MultiColorEffectModeViewModel(mode) as IEffectModeViewModel
                    : new FixedColorEffectModeViewModel(mode) as IEffectModeViewModel;
            }).ToList();

            SelectedEffectMode = EffectModes.First(m => m.Name == "Fixed");
        }

        public void AddAccessory(Hue2AccessoryViewModel accessoryViewModel)
        {
            Accessories.Add(accessoryViewModel);
        }

        public async Task ApplyChangesToDevice()
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
