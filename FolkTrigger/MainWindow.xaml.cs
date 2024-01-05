using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using FolkTrigger.Pages;

namespace FolkTrigger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand,
                (_, _) => SystemCommands.CloseWindow(this)));
            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand,
                (_, _) => SystemCommands.MinimizeWindow(this)));

            StartRadioButton.IsChecked = true;
        }

        private static readonly Dictionary<Type, Page> BufferPages = new();

        private void NaviRadioButton_Checked(object sender, RoutedEventArgs eventArgs)
        {
            if (sender is not RadioButton radioButton)
                return;
            if (radioButton.Tag is not Type type)
                return;
            
            if (!BufferPages.TryGetValue(type, out Page? page))
                page = BufferPages[type] = Activator.CreateInstance(type) as Page ?? throw new Exception("this would never happen");

            MainFrame.Navigate(page);
        }
    }
}