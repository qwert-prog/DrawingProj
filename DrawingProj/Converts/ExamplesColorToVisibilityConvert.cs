using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace DrawingProj.Converts
{
    /// <summary>
    /// Converts color to viability.
    /// If transparent, return visible.
    /// Else Collapsed
    /// </summary>
    internal sealed class ColorToVisibilityConvert : IValueConverter
    {
        #region Public Methods

        /// <summary>
        /// Method for converting color to Visibility.
        /// If transparent, return visible.
        /// Else Collapsed
        /// </summary>
        /// <param name="value">Значение для преобразования</param>
        /// <param name="targetType">Тип к которому нужно преобразовать значение</param>
        /// <param name="parameter">Вспомогательный параметр привязки</param>
        /// <param name="language">Языковой код</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Color color = (Color)value;
            if (color == Colors.Transparent)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Method for reverse conversion
        /// </summary>
        /// <param name="value">Значение для преобразования</param>
        /// <param name="targetType">Тип к которому нужно преобразовать значение</param>
        /// <param name="parameter">Вспомогательный параметр привязки</param>
        /// <param name="language">Языковой код</param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (Color)value;
        }

        #endregion Public Methods
    }
}