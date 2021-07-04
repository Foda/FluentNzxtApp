using FluentNzxt.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Numerics;

namespace FluentNzxt.View
{
    public sealed partial class DeviceView : UserControl
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty
               .Register(nameof(ViewModel), typeof(SmartDeviceViewModel), typeof(DeviceView), new PropertyMetadata(null));

        public SmartDeviceViewModel ViewModel
        {
            get => (SmartDeviceViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public DeviceView()
        {
            this.InitializeComponent();
        }
    }
}
