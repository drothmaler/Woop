using System;
using Windows.Foundation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.UI.ViewManagement;
using Windows.ApplicationModel.Core;
using Windows.System;
using System.Numerics;
using Woop.ViewModels;
using Woop.Services;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace Woop.Views
{
    public sealed partial class MainPage : Page
    {
        private CoreApplicationViewTitleBar _coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
        private readonly SettingsService _settingsService;
        private readonly long _isOpenPropertyChangedCallbackToken;

        public double CoreTitleBarHeight => _coreTitleBar.Height;

        public Thickness CoreTitleBarPadding
        {
            get
            {
                if (FlowDirection == FlowDirection.LeftToRight)
                {
                    return new Thickness { Left = _coreTitleBar.SystemOverlayLeftInset, Right = _coreTitleBar.SystemOverlayRightInset };
                }
                else
                {
                    return new Thickness { Left = _coreTitleBar.SystemOverlayRightInset, Right = _coreTitleBar.SystemOverlayLeftInset };
                }
            }
        }

        public MainViewModel ViewModel { get; private set; }

        public MainPage()
        {
            _settingsService = new SettingsService();
            _settingsService.ApplicationThemeChanged += OnApplicationThemeChanged;
            ViewModel = new MainViewModel(Dispatcher, _settingsService);

            RequestedTheme = _settingsService.ApplicationTheme;

            InitializeComponent();

            App.Window.SetTitleBar(TitleBar);

            Selector.Translation += new Vector3(0, 0, 32);

                /*
                   TODO UA315_A Use Microsoft.UI.Windowing.AppWindow for window Management instead of ApplicationView/CoreWindow or Microsoft.UI.Windowing.AppWindow APIs
                   Read: https://docs.microsoft.com/en-us/windows/apps/windows-app-sdk/migrate-to-windows-app-sdk/guides/windowing
                */
                ApplicationView.PreferredLaunchViewSize = new Size(480, 480);

                /*
                   TODO UA315_A Use Microsoft.UI.Windowing.AppWindow for window Management instead of ApplicationView/CoreWindow or Microsoft.UI.Windowing.AppWindow APIs
                   Read: https://docs.microsoft.com/en-us/windows/apps/windows-app-sdk/migrate-to-windows-app-sdk/guides/windowing
                */
                ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            SetTitleBarColors();
            Root.RequestedTheme = _settingsService.ApplicationTheme;

            _isOpenPropertyChangedCallbackToken = SelectorPopup.RegisterPropertyChangedCallback(Popup.IsOpenProperty, (s, e) =>
            {
                if (SelectorPopup.IsOpen)
                {
                    FocusAction.TargetObject = Query;
                    FocusAction.Execute(s, e);
                }
            });
        }

        private void SetTitleBarColors()
        {
            var titleBar = 
                /*
                   TODO UA315_A Use Microsoft.UI.Windowing.AppWindow for window Management instead of ApplicationView/CoreWindow or Microsoft.UI.Windowing.AppWindow APIs
                   Read: https://docs.microsoft.com/en-us/windows/apps/windows-app-sdk/migrate-to-windows-app-sdk/guides/windowing
                */
                ApplicationView.GetForCurrentView().TitleBar;

            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            if (ActualTheme == ElementTheme.Light)
            {
                titleBar.ButtonHoverBackgroundColor = "#FFE6E6E6".ToColor();
                titleBar.ButtonPressedBackgroundColor = "#FFCCCCCC".ToColor();

                titleBar.ButtonForegroundColor = Colors.Black;
                titleBar.ButtonHoverForegroundColor = Colors.Black;
                titleBar.ButtonPressedForegroundColor = Colors.Black;
            }
            else
            {
                titleBar.ButtonHoverBackgroundColor = "#FF191919".ToColor();
                titleBar.ButtonPressedBackgroundColor = "#FF333333".ToColor();

                titleBar.ButtonForegroundColor = Colors.White;
                titleBar.ButtonHoverForegroundColor = Colors.White;
                titleBar.ButtonPressedForegroundColor = Colors.White;
            }
        }

        private async void OnApplicationThemeChanged(object sender, ElementTheme e)
        {
            await /*
                TODO UA306_A2: UWP CoreDispatcher : Windows.UI.Core.CoreDispatcher is no longer supported. Use DispatcherQueue instead. Read: https://docs.microsoft.com/en-us/windows/apps/windows-app-sdk/migrate-to-windows-app-sdk/guides/threading
            */Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Root.RequestedTheme = e;
                SetTitleBarColors();
            });
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LineNumbers.Initialize(Buffer);
            await ViewModel.InitializeAsync(Buffer);
        }

        private void Query_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                ViewModel.RunSelectedScript();
            }
            else if (e.Key == VirtualKey.Up)
            {
                ViewModel.SelectPrevious();
            }
            else if (e.Key == VirtualKey.Down)
            {
                ViewModel.SelectNext();
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Scripts.ScrollIntoView(ViewModel.SelectedScript);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            App.Window.SetTitleBar(null);
            _settingsService.ApplicationThemeChanged -= OnApplicationThemeChanged;
            SelectorPopup.UnregisterPropertyChangedCallback(Popup.IsOpenProperty, _isOpenPropertyChangedCallbackToken);
            ViewModel = null;
            _coreTitleBar = null;
        }

        private async void OnSettingsTapped(object sender, RoutedEventArgs e)
        {
            var settings = new SettingsDialog(_settingsService);
            await settings.ShowAsync();
        }

        private async void OnAboutClicked(object sender, RoutedEventArgs e)
        {
            var about = new AboutDialog();
            await about.ShowAsync();
        }

        private void OnScriptDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            ViewModel.RunSelectedScript();
        }
    }
}
