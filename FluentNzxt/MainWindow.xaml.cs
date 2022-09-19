using FluentNzxt.Utils;
using FluentNzxt.ViewModel;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices; // For DllImport
using Windows.Graphics;
using WinRT; // required to support Window.As<ICompositionSupportsSystemBackdrop>()

namespace FluentNzxt
{
    public sealed partial class MainWindow : Window
    {
        public MainWindowViewModel ViewModel { get; }

        private WindowsSystemDispatcherQueueHelper _wsdqHelper; // See separate sample below for implementation
        private Microsoft.UI.Composition.SystemBackdrops.MicaController _micaController;
        private Microsoft.UI.Composition.SystemBackdrops.SystemBackdropConfiguration _configurationSource;

        public MainWindow()
        {
            this.Activated += Window_Activated;
            this.Closed += Window_Closed;

            this.InitializeComponent();
            TrySetMicaBackdrop();

            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId wndId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appW = AppWindow.GetFromWindowId(wndId);
            appW.Resize(new SizeInt32(1000, 800));

            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);

            this.RootGrid.Loaded += RootGrid_Loaded;
            ViewModel = new MainWindowViewModel();
        }

        private async void RootGrid_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.FindDevices();
        }

        private bool TrySetMicaBackdrop()
        {
            if (Microsoft.UI.Composition.SystemBackdrops.MicaController.IsSupported())
            {
                _wsdqHelper = new WindowsSystemDispatcherQueueHelper();
                _wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

                // Hooking up the policy object
                _configurationSource = new Microsoft.UI.Composition.SystemBackdrops.SystemBackdropConfiguration();
                ((FrameworkElement)this.Content).ActualThemeChanged += Window_ThemeChanged;

                // Initial configuration state.
                _configurationSource.IsInputActive = true;
                SetConfigurationSourceTheme();

                _micaController = new Microsoft.UI.Composition.SystemBackdrops.MicaController();

                // Enable the system backdrop.
                // Note: Be sure to have "using WinRT;" to support the Window.As<...>() call.
                _micaController.AddSystemBackdropTarget(this.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
                _micaController.SetSystemBackdropConfiguration(_configurationSource);
                return true; // succeeded
            }

            return false; // Mica is not supported on this system
        }

        private void Window_Activated(object sender, WindowActivatedEventArgs args)
        {
            if (_configurationSource != null)
            {
                _configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
            }
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            // Make sure any Mica/Acrylic controller is disposed so it doesn't try to
            // use this closed window.
            if (_micaController != null)
            {
                _micaController.Dispose();
                _micaController = null;
            }
            this.Activated -= Window_Activated;
            _configurationSource = null;
        }


        private void Window_ThemeChanged(FrameworkElement sender, object args)
        {
            if (_configurationSource != null)
            {
                SetConfigurationSourceTheme();
            }
        }

        private void SetConfigurationSourceTheme()
        {
            switch (((FrameworkElement)this.RootGrid).ActualTheme)
            {
                case ElementTheme.Dark:    _configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Dark; break;
                case ElementTheme.Light:   _configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Light; break;
                case ElementTheme.Default: _configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Default; break;
            }
        }
    }
}
