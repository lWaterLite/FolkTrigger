using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FTCustomControlLibrary
{
    public class Switcher : ToggleButton
    {
        static Switcher()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Switcher),
                new FrameworkPropertyMetadata(typeof(Switcher)));
        }

        #region PropertyDependency

        public static readonly DependencyProperty CheckedTextProperty =
            DependencyProperty.Register(nameof(CheckedText), typeof(string), typeof(Switcher), new PropertyMetadata("ON"));

        public static readonly DependencyProperty UncheckedTextProperty =
            DependencyProperty.Register(nameof(UncheckedText), typeof(string), typeof(Switcher), new PropertyMetadata("OFF"));
        
        public static readonly DependencyProperty ButtonColorProperty = 
            DependencyProperty.Register(nameof(ButtonColor), typeof(Brush), typeof(Switcher),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(43, 43, 43))));

        public string CheckedText
        {
            get => (string)GetValue(CheckedTextProperty);
            set => SetValue(CheckedTextProperty, value);
        }

        public string UncheckedText
        {
            get => (string)GetValue(UncheckedTextProperty);
            set => SetValue(UncheckedTextProperty, value);
        }

        public Brush ButtonColor
        {
            get => (Brush)GetValue(ButtonColorProperty);
            set => SetValue(ButtonColorProperty, value);
        }

        #endregion

        private TranslateTransform _buttonMovement;

    }
}