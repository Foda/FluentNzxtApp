using FluentNzxt.ViewModel;
using Microsoft.UI.Xaml;
using System.Collections.ObjectModel;

namespace FluentNzxt
{
    public sealed partial class MainWindow : Window
    {
        public MainWindowViewModel ViewModel { get; }

        public MainWindow()
        {
            this.InitializeComponent();
            this.RootGrid.Loaded += RootGrid_Loaded;
            ViewModel = new MainWindowViewModel();
        }

        private async void RootGrid_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.FindDevices();
        }
    }
}
