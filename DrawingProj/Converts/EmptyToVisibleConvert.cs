using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace DrawingProj.Converts
{
    /// <summary>
    /// Converts count list to visibility
    /// If count = 0, return Visible. Else Collapsed
    /// </summary>
    public class EmptyToVisibleConvert : IValueConverter
    {
        #region Public Methods

        /// <summary>
        /// Method for convert list count to visibility
        /// </summary>
        /// <param name="value">Значение для преобразования</param>
        /// <param name="targetType">Тип к которому нужно преобразовать значение</param>
        /// <param name="parameter">Вспомогательный параметр привязки</param>
        /// <param name="language">Языковой код</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((int)value != 0)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
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
            return (int)value;
        }

        #endregion Public Methods
    }
}