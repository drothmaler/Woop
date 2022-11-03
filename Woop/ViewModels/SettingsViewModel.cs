using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Microsoft.UI.Xaml;
using Woop.Services;

namespace Woop.ViewModels
{
    public class SettingsViewModel : ObservableObject
    {
        private readonly SettingsService _settingsService;
        private bool _invalidDirectory;

        public SettingsViewModel(SettingsService settingsService)
        {
            _settingsService = settingsService;

            ElementThemes = Enum.GetValues(typeof(ElementTheme)).Cast<ElementTheme>();

            BrowseCommand = new AsyncRelayCommand(Browse);
        }

        public IAsyncRelayCommand BrowseCommand { get; }

        public IEnumerable<ElementTheme> ElementThemes { get; }

        public ElementTheme SelectedApplicationTheme
        {
            get => _settingsService.ApplicationTheme;
            set
            {
                if (_settingsService.ApplicationTheme != value)
                {
                    _settingsService.ApplicationTheme = value;
                    OnPropertyChanged(nameof(SelectedApplicationTheme));
                }
            }
        }

        public string CustomScriptsFolderLocation
        {
            get => _settingsService.CustomScriptsFolderLocation;
            set
            {
                _settingsService.CustomScriptsFolderLocation = value;
                OnPropertyChanged(nameof(CustomScriptsFolderLocation));
            }
        }

        public async Task Browse()
        {
            var picker = this.InitializeWithWindow(new FolderPicker());
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add("*");

            var folder = await picker.PickSingleFolderAsync();

            if (folder != null)
            {
                StorageApplicationPermissions.FutureAccessList.AddOrReplace("CustomScriptsToken", folder);
                CustomScriptsFolderLocation = folder.Path;
            }
        }
                        private FolderPicker InitializeWithWindow(FolderPicker obj)
                        {
                            WinRT.Interop.InitializeWithWindow.Initialize(obj, App.WindowHandle);
                            return obj;
                        }
    }
}
