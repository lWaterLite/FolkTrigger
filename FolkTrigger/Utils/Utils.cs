using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FolkTrigger.Utils;

public class Utils
{
    public static async void ExceptionHandler(Exception exception, TextBlock waringTextBlock)
    {
        waringTextBlock.Text = exception.ToString();
        if (waringTextBlock.Visibility == Visibility.Hidden || waringTextBlock.Visibility == Visibility.Collapsed)
            waringTextBlock.Visibility = Visibility.Visible;
        else
            return;

        await Task.Delay(5000);
        waringTextBlock.Visibility = Visibility.Hidden;
        waringTextBlock.Text = String.Empty;
    }
}
