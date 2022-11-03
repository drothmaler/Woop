using ColorCode;
using Microsoft.Toolkit.Uwp.UI;
using Windows.System;
using Windows.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Woop.Models;
using Woop.Services;
using Woop.ViewModels;

namespace Woop.Views
{
    public class SyntaxHighlightingRichEditBox : RichEditBox, IBuffer
    {
        private RtfFormatter _rtfFormatter;
        private readonly ILanguage _language;

        public ScrollViewer ScrollViewer { get; private set; }

        public SyntaxHighlightingRichEditBox()
        {
            _rtfFormatter = new RtfFormatter(ActualTheme == ElementTheme.Light ? ColorCodeThemes.Light : ColorCodeThemes.Dark);
            _language = new BoopPseudoLanguage();

            KeyDown += OnKeyDown;
            TextChanging += OnTextChanging;
            Loaded += OnLoaded;
            ActualThemeChanged += OnActualThemeChanged;

            DisabledFormattingAccelerators = DisabledFormattingAccelerators.All;
            IsSpellCheckEnabled = false;

            ActualThemeChanged += OnActualThemeChanged;
        }

        private void OnActualThemeChanged(FrameworkElement sender, object args)
        {
            _rtfFormatter = new RtfFormatter(ActualTheme == ElementTheme.Light ? ColorCodeThemes.Light : ColorCodeThemes.Dark);
            UpdateText();
        }

        private void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ScrollViewer = this.FindDescendant<ScrollViewer>();
        }

        private void OnTextChanging(RichEditBox sender, RichEditBoxTextChangingEventArgs args)
        {
            if (args.IsContentChanging)
            {
                UpdateText();
            }
        }

        private void OnKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Tab)
            {
                Document.Selection.TypeText("\t");
                e.Handled = true;
            }
        }

        public void UpdateText()
        {
            // Attempt to get Scrollviewer offsets, to preserve location.
            var vertOffset = ScrollViewer?.VerticalOffset;
            var horOffset = ScrollViewer?.HorizontalOffset;

            var selection = Document.Selection;
            var selectionStart = selection.StartPosition;
            var selectionEnd = selection.EndPosition;

            Document.GetText(TextGetOptions.UseCrlf, out string raw);
            Document.Undo();
            Document.BeginUndoGroup();

            var rtf = _rtfFormatter.GetRtfString(raw, _language);

            Document.SetText(TextSetOptions.FormatRtf, rtf);

            var newSelection = Document.Selection;
            newSelection.StartPosition = selectionStart;
            newSelection.EndPosition = selectionEnd;

            Document.ApplyDisplayUpdates();
            Document.EndUndoGroup();

            ScrollViewer?.ChangeView(horOffset, vertOffset, null, true);
        }

        string IBuffer.GetText()
        {
            Document.GetText(TextGetOptions.UseCrlf, out var text);
            return text;
        }

        void IBuffer.SetText(string text)
        {
            Document.SetText(TextSetOptions.None, text);
            Document.Selection.StartPosition = text.Length;
        }

        Selection IBuffer.GetSelection()
        {
            Document.Selection.GetText(TextGetOptions.UseCrlf, out var selectedText);
            return new Selection
            {
                Content = selectedText,
                Start = Document.Selection.StartPosition,
                Length = Document.Selection.Length
            };
        }

        void IBuffer.SetSelection(string text)
        {
            Document.Selection.SetText(TextSetOptions.None, text ?? string.Empty);
        }
    }
}
