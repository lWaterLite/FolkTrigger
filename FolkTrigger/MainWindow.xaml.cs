using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FolkTrigger;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private static readonly Dictionary<Type, Page> BufferPages = new();

    public MainWindow()
    {
        InitializeComponent();

        CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand,
            (_, _) => SystemCommands.CloseWindow(this)));
        CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand,
            (_, _) => SystemCommands.MinimizeWindow(this)));

        StartRadioButton.IsChecked = true;
    }

    private void NaviRadioButton_Checked(object sender, RoutedEventArgs eventArgs)
    {
        if (sender is not RadioButton radioButton)
            return;
        if (radioButton.Tag is not Type type)
            return;

        if (!BufferPages.TryGetValue(type, out Page? page))
            page = BufferPages[type] =
                Activator.CreateInstance(type) as Page ?? throw new Exception("this would never happen");

        MainFrame.Navigate(page);
    }
}