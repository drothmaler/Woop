using System;
using Windows.Storage;
using Windows.System;
using Microsoft.UI.Xaml.Controls;

namespace Woop.Views
{
    public sealed partial class AboutDialog : ContentDialog
    {
        public AboutDialog()
        {
            this.InitializeComponent();
            Version = GetCurrentVersion();
        }

        public string Version { get; }

        private void OnCloseTapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Hide();
        }

        private async void LicensesClicked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/third-party-licenses.txt"));
            await Launcher.LaunchFileAsync(file);
        }

        public string GetCurrentVersion()
        {
            var version = Windows.ApplicationModel.Package.Current.Id.Version;
            return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
    }
}
