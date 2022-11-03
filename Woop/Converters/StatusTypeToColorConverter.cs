using System;
using Windows.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Woop.ViewModels;

namespace Woop.Converters
{
    public class StatusTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is StatusViewModel.StatusType type)
            {
                switch (type)
                {
                    case StatusViewModel.StatusType.Normal:
                        return new SolidColorBrush(Colors.Transparent);
                    case StatusViewModel.StatusType.Info:
                        return new SolidColorBrush(Microsoft.Toolkit.Uwp.Helpers.ColorHelper.ToColor("#0063B1"));
                    case StatusViewModel.StatusType.Success:
                        return new SolidColorBrush(Microsoft.Toolkit.Uwp.Helpers.ColorHelper.ToColor("#10893E"));
                    case StatusViewModel.StatusType.Error:
                        return new SolidColorBrush(Microsoft.Toolkit.Uwp.Helpers.ColorHelper.ToColor("#E74856"));
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
