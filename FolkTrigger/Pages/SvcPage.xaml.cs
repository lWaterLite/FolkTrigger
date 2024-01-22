using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using FolkTrigger.ViewModels;

namespace FolkTrigger.Pages;

public partial class SvcPage
{
    
    private readonly string _basePath = AppDomain.CurrentDomain.BaseDirectory + @"MetaFolk_SVC\";
    private SvcPageViewModel _viewModel;
    
    public SvcPage()
    {
        InitializeComponent();
        _viewModel = (DataContext as SvcPageViewModel)! ;
        
        _viewModel.Speakers.Add(new Speaker(){Name = "lsx", Count = 123});
    }

    #region RoutedEventMethods

    private void SvcPageLinkButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button) return;
        if (button.Tag is not SvgLink svgLink) return;

        switch (svgLink)
        {
            case SvgLink.Folder:
                Process.Start("explorer", _basePath);
                break;
            case SvgLink.Readme:
                Process.Start("explorer", _basePath + "readme.md");
                break;
            case SvgLink.Configs:
                Process.Start("explorer", _basePath + "configs");
                break;
            case SvgLink.DatasetRaw:
                Process.Start("explorer", _basePath + "dataset_raw");
                break;
            case SvgLink.Logs:
                Process.Start("explorer", _basePath + "logs");
                break;
            case SvgLink.Pretrain:
                Process.Start("explorer", _basePath + "pretrain");
                break;
            case SvgLink.Trained:
                Process.Start("explorer", _basePath + "trained");
                break;
            case SvgLink.Results:
                Process.Start("explorer", _basePath + "results");
                break;
            default:
                throw new ArgumentOutOfRangeException(svgLink.ToString());
        }
    }

    #endregion
}

public enum SvgLink
{
    Folder,
    Readme,
    Configs,
    DatasetRaw,
    Logs,
    Pretrain,
    Trained,
    Results
}