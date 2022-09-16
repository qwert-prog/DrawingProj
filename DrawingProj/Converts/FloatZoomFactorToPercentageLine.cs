using System;
using Windows.UI.Xaml.Data;

namespace DrawingProj.Converts
{
    /// <summary>
    /// Converts a float number to a string with a % appended
    /// </summary>
    internal class FloatZoomFactorToPercentageLine : IValueConverter
    {
        #region Public Methods

        /// <summary>
        /// Converts a float number to a string
        /// </summary>
        /// <param name="value">Значение для преобразования</param>
        /// <param name="targetType">Тип к которому нужно преобразовать значение</param>
        /// <param name="parameter">Вспомогательный параметр привязки</param>
        /// <param name="language">Языковой код</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            float scale = (float)value * 100;
            scale = (float)Math.Round(scale);
            string scaleString = scale.ToString() + " %";
            return scaleString;
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
            return (float)value;
        }

        #endregion Public Methods
    }
}