using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.VisualBasic.CompilerServices;

namespace FolkTrigger.Pages;

public partial class StartPage : Page
{
    public StartPage()
    {
        InitializeComponent();
        
        StartTitleBackgroundInit();
    }

    private void StartTitleBackgroundInit()
    {
        List<string> imagePaths = new List<string>()
        {
            "pack://application:,,,/Assets/Image/start_1.png",
            "pack://application:,,,/Assets/Image/start_2.png"
        };

        Random random = new();
        string randomImagePath = imagePaths[random.Next(imagePaths.Count)];

        StartTitleBackgroundImageBrush.ImageSource = new BitmapImage(new Uri(randomImagePath));
    }

    private void StartPageLinkButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;
        if (button.Tag is not StartLink startLink)
            return;

        try
        {
            switch (startLink)
            {
                case StartLink.LauncherReadme:
                    Process.Start("explorer", AppDomain.CurrentDomain.BaseDirectory + "readme.md");
                    break;
                case StartLink.LauncherFolder:
                    Process.Start("explorer", AppDomain.CurrentDomain.BaseDirectory);
                    break;
                case StartLink.CnnReadme:
                    Process.Start("explorer", AppDomain.CurrentDomain.BaseDirectory + @"MetaFolk_CNN\readme.md");
                    break;
                case StartLink.CnnFolder:
                    Process.Start("explorer", AppDomain.CurrentDomain.BaseDirectory + "MetaFolk_CNN");
                    break;
                case StartLink.SvcReadme:
                    Process.Start("explorer", AppDomain.CurrentDomain.BaseDirectory + @"MetaFolk_SVC\readme.md");
                    break;
                case StartLink.SvcFolder:
                    Process.Start("explorer", AppDomain.CurrentDomain.BaseDirectory + "MetaFolk_SVC");
                    break;
            }
        }
        catch (Exception exception)
        {
            ShowBottomInfoTextBlock(exception.ToString(), "#b71c1c");
        }
    }

    // private void TestButton_Click(object sender, RoutedEventArgs eventArgs)
    // {
    //
    //     try
    //     {
    //         throw new Exception("This is a test Exception");
    //     }
    //     catch (Exception e)
    //     {
    //         Utils.Utils.ExceptionHandler(e, BottomInfoTextBlock);
    //     }
    // }
    
    private async void ShowBottomInfoTextBlock(string info, string backgroundColor)
    {
        if (BottomInfoTextBlock.Visibility is Visibility.Visible) return;
        BottomInfoTextBlock.Text = info;
        BottomInfoTextBlock.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(backgroundColor));
        BottomInfoTextBlock.Visibility = Visibility.Visible;
        await Task.Delay(5000);
        BottomInfoTextBlock.Visibility = Visibility.Collapsed;
    }
    
}

public enum StartLink
{
    LauncherReadme,
    CnnReadme,
    SvcReadme,
    LauncherFolder,
    CnnFolder,
    SvcFolder
}
