using FluentNzxt.ViewModel;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Numerics;
using Windows.UI;

namespace FluentNzxt.View
{
    public sealed partial class ChannelView : UserControl
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty
               .Register(nameof(ViewModel), typeof(ChannelViewModel), typeof(ChannelView), new PropertyMetadata(null));

        public IChannelViewModel ViewModel
        {
            get => (IChannelViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public ChannelView()
        {
            this.InitializeComponent();
        }
    }
}
