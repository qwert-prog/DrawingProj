using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace DrawingProj.Converts
{
    /// <summary>
    /// Converts Color to SolidColorBrush
    /// </summary>
    internal class ColorToBrushConvert : IValueConverter
    {
        #region Public Methods

        /// <summary>
        /// Method for convert color to SolidColorBrush
        /// </summary>
        /// <param name="value">Значение для преобразования</param>
        /// <param name="targetType">Тип к которому нужно преобразовать значение</param>
        /// <param name="parameter">Вспомогательный параметр привязки</param>
        /// <param name="language">Языковой код</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Color)
            {
                return new SolidColorBrush((Color)value);
            }
            else
            {
                return null;
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