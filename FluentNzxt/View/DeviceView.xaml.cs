using FluentNzxt.ViewModel;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Numerics;
using Windows.UI;

namespace FluentNzxt.View
{
    public sealed partial class DeviceView : UserControl
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty
               .Register(nameof(ViewModel), typeof(IDeviceViewModel), typeof(DeviceView), new PropertyMetadata(null));

        public IDeviceViewModel ViewModel
        {
            get => (IDeviceViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public DeviceView()
        {
            this.InitializeComponent();
        }
    }
}
