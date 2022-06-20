using System;
using System.Globalization;
using System.IO;
using AutoMapper;
using Xamarin.Forms;

namespace AsteriskMenu.Helpers
{
    public class BytesToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ImageSource.FromStream(() =>
            {
                return new MemoryStream((Byte[])value);
            });
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}