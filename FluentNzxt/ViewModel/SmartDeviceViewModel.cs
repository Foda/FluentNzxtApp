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

        private ObservableCollection<IChannelViewModel> _channels = new();
        public ObservableCollection<IChannelViewModel> Channels
        {
            get => _channels;
            private set => SetProperty(ref _channels, value);
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
            FindDeviceCommand = new AsyncRelayCommand(FindDevice);
            ApplyCommand = new AsyncRelayCommand(Apply, () => !IsLoading);
        }

        private async Task FindDevice()
        {
            IsLoading = true;

            Channels.Clear();

            bool didFind = await _model.FindDevice();
            if (didFind)
            {
                foreach (INzxtAccessory accessory in _model.LedAccessories)
                {
                    Hue2AccessoryViewModel newAccessory = new Hue2AccessoryViewModel(accessory);

                    IChannelViewModel channel = Channels.FirstOrDefault(c => c.Channel == newAccessory.Channel);
                    if (channel == null)
                    {
                        channel = new ChannelViewModel(_model, newAccessory.Channel);
                        Channels.Add(channel);
                    }
                    channel.AddAccessory(newAccessory);
                }
            }

            IsLoading = false;
        }

        private async Task Apply()
        {
            foreach (IChannelViewModel channelViewModel in Channels)
            {
                await channelViewModel.ApplyChangesToDevice();
            }
        }
    }
}
