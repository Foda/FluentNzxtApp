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
    public sealed partial class FixedColorEffectModeView : UserControl
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty
               .Register(nameof(ViewModel), typeof(FixedColorEffectModeViewModel), typeof(FixedColorEffectModeView), new PropertyMetadata(null));

        public FixedColorEffectModeViewModel ViewModel
        {
            get => (FixedColorEffectModeViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public FixedColorEffectModeView()
        {
            this.InitializeComponent();
        }

        private void ColorFlyout_Closed(object sender, object e)
        {
            ViewModel.Color = ColorPickerFlyoutPicker.Color;
        }
    }
}
