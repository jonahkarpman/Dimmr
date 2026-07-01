using System;
using System.Globalization;
using System.Windows.Data;

namespace Dimmr.Converters;

/// <summary>Returns the logical negation of a bool. Used to enable manual-bounds fields.</summary>
public sealed class InverseBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is bool b ? !b : value;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => value is bool b ? !b : value;
}
