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

            ViewModel = new MainWindowViewModel();
        }
    }
}
