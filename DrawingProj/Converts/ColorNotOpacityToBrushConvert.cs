using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace DrawingProj.Converts
{
    /// <summary>
    /// Converts Color(regardless of opacity) to SolidColorBrush
    /// </summary>
    internal class ColorNotOpacityToBrushConvert : IValueConverter
    {
        #region Public Methods

        /// <summary>
        /// Method for convert color (regardless of opacity) to SolidColorBrush
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
                Color color = (Color)value;
                return new SolidColorBrush(Color.FromArgb(255, color.R, color.G, color.B));
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