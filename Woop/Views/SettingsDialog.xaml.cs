using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Woop.Services;
using Woop.ViewModels;

namespace Woop.Views
{
    public sealed partial class SettingsDialog : ContentDialog
    {
        private readonly SettingsService _settingsService;

        public SettingsDialog(SettingsService settingsService)
        {
            _settingsService = settingsService;
            _settingsService.ApplicationThemeChanged += OnApplicationThemeChanged;
            ViewModel = new SettingsViewModel(settingsService);
            RequestedTheme = ViewModel.SelectedApplicationTheme;

            InitializeComponent();
        }

        public SettingsViewModel ViewModel { get; }

        private void OnCloseTapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Hide();
        }

        private async void OnApplicationThemeChanged(object sender, ElementTheme e)
        {
            await /*
                TODO UA306_A2: UWP CoreDispatcher : Windows.UI.Core.CoreDispatcher is no longer supported. Use DispatcherQueue instead. Read: https://docs.microsoft.com/en-us/windows/apps/windows-app-sdk/migrate-to-windows-app-sdk/guides/threading
            */Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => RequestedTheme = e);
        }

        private void ContentDialog_Unloaded(object sender, RoutedEventArgs e)
        {
            _settingsService.ApplicationThemeChanged -= OnApplicationThemeChanged;
        }
    }
}
