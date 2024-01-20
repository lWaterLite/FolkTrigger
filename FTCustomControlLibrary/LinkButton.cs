using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FTCustomControlLibrary;

public class LinkButton : Button
{
    static LinkButton()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(LinkButton), new FrameworkPropertyMetadata(typeof(LinkButton)));
    }

    #region PropertyDependency

    public static readonly DependencyProperty LinkNameTextProperty = DependencyProperty.Register(nameof(LinkNameText),
        typeof(string), typeof(LinkButton), new PropertyMetadata("LinkName"));

    public static readonly DependencyProperty LinkPathTextProperty = DependencyProperty.Register(nameof(LinkPathText),
        typeof(string), typeof(LinkButton), new PropertyMetadata("LinkPath"));

    public static readonly DependencyProperty IconTextProperty =
        DependencyProperty.Register(nameof(IconText), typeof(string), typeof(LinkButton));

    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(nameof(CornerRadius),
        typeof(CornerRadius), typeof(LinkButton), new PropertyMetadata(new CornerRadius(0)));

    public static readonly DependencyProperty MouseOverBackgroundProperty = DependencyProperty.Register(
        nameof(MouseOverBackground),
        typeof(Brush), typeof(LinkButton), new PropertyMetadata(null));

    public static readonly DependencyProperty MouseOverForegroundProperty = DependencyProperty.Register(
        nameof(MouseOverForeground),
        typeof(Brush), typeof(LinkButton), new PropertyMetadata(null));

    public string LinkNameText
    {
        get => (string)GetValue(LinkNameTextProperty);
        set => SetValue(LinkNameTextProperty, value);
    }

    public string LinkPathText
    {
        get => (string)GetValue(LinkPathTextProperty);
        set => SetValue(LinkPathTextProperty, value);
    }

    public string IconText
    {
        get => (string)GetValue(IconTextProperty);
        set => SetValue(IconTextProperty, value);
    }

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public Brush MouseOverBackground
    {
        get => (Brush)GetValue(MouseOverBackgroundProperty);
        set => SetValue(MouseOverBackgroundProperty, value);
    }

    public Brush MouseOverForeground
    {
        get => (Brush)GetValue(MouseOverForegroundProperty);
        set => SetValue(MouseOverForegroundProperty, value);
    }

    #endregion
}