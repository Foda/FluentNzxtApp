﻿using System;
using Microsoft.UI.Xaml.Data;

namespace FluentNzxt.Converters
{
    public class ImplicitDataTemplateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || App.Current == null)
                return null;

            object dataTemplate;

            if (App.Current.Resources.TryGetValue(value.GetType().Name, out dataTemplate))
                return dataTemplate;

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
