using FluentNzxt.ViewModel;
using Microsoft.UI.Xaml.Controls;
using System.Numerics;

namespace FluentNzxt.View
{
    public sealed partial class DeviceView : UserControl
    {
        public SmartDeviceViewModel ViewModel { get; }

        public DeviceView()
        {
            this.InitializeComponent();

            ViewModel = new SmartDeviceViewModel(new NzxtLib.SmartDevice());

            Loaded += DeviceView_Loaded;
        }

        private async void DeviceView_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            await ViewModel.FindDeviceCommand.ExecuteAsync(null);
        }

        private void DeviceColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            ViewModel.Color = DeviceColorPicker.Color;
        }
    }
}
