using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace DrawingProj.Converts
{
    /// <summary>
    /// Converts null to Collapsed,object to Visible
    /// </summary>
    internal class ObjectToVisibilityConvert : IValueConverter
    {
        #region Public Methods

        /// <summary>
        /// Method for convert object to visibility
        /// </summary>
        /// <param name="value">Значение для преобразования</param>
        /// <param name="targetType">Тип к которому нужно преобразовать значение</param>
        /// <param name="parameter">Вспомогательный параметр привязки</param>
        /// <param name="language">Языковой код</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
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
            return value;
        }

        #endregion Public Methods
    }
}