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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FluentNzxt.View
{
    public sealed partial class MultiColorEffectModeView : UserControl
    {
        private int _editColorIndex = -1;

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty
               .Register(nameof(ViewModel), typeof(MultiColorEffectModeViewModel), typeof(MultiColorEffectModeView), new PropertyMetadata(null));

        public MultiColorEffectModeViewModel ViewModel
        {
            get => (MultiColorEffectModeViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public MultiColorEffectModeView()
        {
            this.InitializeComponent();
        }

        private void ColorPickerButton_Click(object sender, RoutedEventArgs e)
        {
            Color selectedColor = (Color)(sender as Button).Tag;
            _editColorIndex = ViewModel.ColorSequence.IndexOf(selectedColor);
        }

        private void ColorFlyout_Closed(object sender, object e)
        {
            if (_editColorIndex != -1)
            {
                ViewModel.ColorSequence[_editColorIndex] = ColorPickerFlyoutPicker.Color;
            }
            _editColorIndex = -1;
        }
    }
}
