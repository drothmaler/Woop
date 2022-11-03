using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Woop.Models;

namespace Woop.ViewModels
{
    public class ScriptViewModel : ObservableObject
    {
        private bool _isSelected;
        public Script Script { get; }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public ScriptViewModel(Script script)
        {
            Script = script;
        }
    }
}
