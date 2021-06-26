using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using Windows.UI;

namespace FluentNzxt.Converters
{
    public class ColorToSolidColorBrushConverter : IValueConverter
    {
        private static Dictionary<Color, SolidColorBrush> _brushCache = new Dictionary<Color, SolidColorBrush>();

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Color drawingColor = (Color)value;
            if (_brushCache.ContainsKey(drawingColor))
            {
                return _brushCache[drawingColor];
            }
            else
            {
                SolidColorBrush newColor = new(
                    Color.FromArgb(255, drawingColor.R, drawingColor.G, drawingColor.B));

                _brushCache.Add(drawingColor, newColor);

                return newColor;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
