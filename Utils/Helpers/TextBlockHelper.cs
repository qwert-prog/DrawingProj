using System;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace Utils.Helpers
{
    /// <summary>
    /// Позволяет доставать из словаря текст с тегом <b></b> и переводит его в жирный
    /// </summary>
    public static class TextBlockHelper
    {
        #region Public Fields

        /// <summary>
        /// Свойство зависимости <see cref="FormattedText"/>
        /// </summary>
        public static readonly DependencyProperty FormattedTextProperty =
            DependencyProperty.RegisterAttached("FormattedText", typeof(string), typeof(TextBlock),
            new PropertyMetadata(string.Empty, (sender, e) =>
            {
                string text = e.NewValue as string;
                TextBlock textBlock = sender as TextBlock;
                if (textBlock != null && text != null)
                {
                    textBlock.Inlines.Clear();
                    string[] str = text.Split(new string[] { "<b>", "</b>" }, StringSplitOptions.None);
                    for (int i = 0; i < str.Length; i++)
                    {
                        textBlock.Inlines.Add(new Run { Text = str[i], FontWeight = i % 2 == 1 ? FontWeights.Bold : FontWeights.Normal });
                    }
                }
            }));

        /// <summary>
        /// Свойство зависимости <see cref="RemoveLastWrapCase"/>
        /// </summary>
        public static readonly DependencyProperty RemoveLastWrapProperty =
          DependencyProperty.RegisterAttached("RemoveLastWrapCase", typeof(bool), typeof(TextBlock), new PropertyMetadata(false, (sender, args) =>
          {
              TextBlock textBlock = (TextBlock)sender;
              textBlock.RegisterPropertyChangedCallback(TextBlock.TextProperty, (s, e) =>
              {
                  if (textBlock.Text.Length > 0)
                  {
                      textBlock.Text = textBlock.Text.TrimEnd('\n');
                  }
              });
          }));

        /// <summary>
        /// Свойство зависимости <see cref="UseUpperCase"/>
        /// </summary>
        public static readonly DependencyProperty UseUpperCaseProperty =
                    DependencyProperty.RegisterAttached("UseUpperCase", typeof(bool), typeof(TextBlock), new PropertyMetadata(false, (sender, args) =>
            {
                TextBlock textBlock = (TextBlock)sender;
                textBlock.RegisterPropertyChangedCallback(TextBlock.TextProperty, (s, e) =>
                {
                    textBlock.Text = textBlock.Text.ToUpper();
                });
            }));

        #endregion Public Fields

        #region Public Methods

        /// <summary>
        /// Метод для преобразования текста в жирный
        /// </summary>
        /// <param name="textBlock"></param>
        /// <returns></returns>
        public static string GetFormattedText(DependencyObject textBlock)
        {
            return (string)textBlock.GetValue(FormattedTextProperty);
        }

        /// <summary>
        /// Метод для удаления последнего символа переноса строки
        /// </summary>
        /// <param name="textBlock"></param>
        /// <returns></returns>
        public static bool GetRemoveLastWrapText(DependencyObject textBlock)
        {
            return (bool)textBlock.GetValue(RemoveLastWrapProperty);
        }

        /// <summary>
        /// Метод для преобразования текста в верхний регистр
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetUseUpperCase(DependencyObject obj)
        {
            return (bool)obj.GetValue(UseUpperCaseProperty);
        }

        /// <summary>
        /// Метод для установки текста для преобразования
        /// </summary>
        /// <param name="textBlock"></param>
        /// <param name="value"></param>
        public static void SetFormattedText(DependencyObject textBlock, string value)
        {
            textBlock.SetValue(FormattedTextProperty, value);
        }

        /// <summary>
        /// Метод для установки флага для функции удаления переноса
        /// </summary>
        /// <param name="textBlock"></param>
        /// <param name="value"></param>
        public static void SetRemoveLastWrapText(DependencyObject textBlock, bool value)
        {
            textBlock.SetValue(RemoveLastWrapProperty, value);
        }

        /// <summary>
        /// Метод для установки флага для функции преобразования в верхний регистр
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetUseUpperCase(DependencyObject obj, bool value)
        {
            obj.SetValue(UseUpperCaseProperty, value);
        }

        #endregion Public Methods
    }
}