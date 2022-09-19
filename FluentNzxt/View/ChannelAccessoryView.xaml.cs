using FluentNzxt.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;

namespace FluentNzxt.View
{
    public sealed partial class ChannelAccessoryView : UserControl
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty
               .Register(nameof(ViewModel), typeof(ChannelAccessoryViewModel), typeof(ChannelAccessoryView), new PropertyMetadata(null));

        public ChannelAccessoryViewModel ViewModel
        {
            get => (ChannelAccessoryViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public ChannelAccessoryView()
        {
            this.InitializeComponent();
        }
    }
}
