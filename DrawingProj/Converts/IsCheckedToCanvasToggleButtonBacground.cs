using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace DrawingProj.Converts
{
    internal class IsCheckedToCanvasToggleButtonBacground : IValueConverter
    {
        #region Public Methods

        /// <summary>
        /// Method for bool value to SolidColorBrushS
        /// </summary>
        /// <param name="value">Значение для преобразования</param>
        /// <param name="targetType">Тип к которому нужно преобразовать значение</param>
        /// <param name="parameter">Вспомогательный параметр привязки</param>
        /// <param name="language">Языковой код</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((bool)value)
            {
                ///Return grey color
                object color = Application.Current.Resources["ActiveCanvasesIconBackground"];
                return (SolidColorBrush)color;
            }
            else
            {
                ///Return transparent color
                return new SolidColorBrush(Colors.Transparent);
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
            return (bool)value;
        }

        #endregion Public Methods
    }
}