using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using FolkTrigger.ViewModels;

using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;
using DialogResult = System.Windows.Forms.DialogResult;

namespace FolkTrigger.Pages;

public partial class SvcPage
{
    
    private readonly string _basePath = AppDomain.CurrentDomain.BaseDirectory + "MetaFolk_SVC";
    private readonly SvcPageViewModel _viewModel;
    
    public SvcPage()
    {
        InitializeComponent();
        _viewModel = (DataContext as SvcPageViewModel)! ;
        
        ReloadTrainingDataset();
        
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
                Process.Start("explorer", _basePath + "\\readme.md");
                break;
            case SvgLink.Configs:
                Process.Start("explorer", _basePath + "\\configs");
                break;
            case SvgLink.DatasetRaw:
                Process.Start("explorer", _basePath + "\\dataset_raw");
                break;
            case SvgLink.Logs:
                Process.Start("explorer", _basePath + "\\logs");
                break;
            case SvgLink.Pretrain:
                Process.Start("explorer", _basePath + "\\pretrain");
                break;
            case SvgLink.Trained:
                Process.Start("explorer", _basePath + "\\trained");
                break;
            case SvgLink.Results:
                Process.Start("explorer", _basePath + "\\results");
                break;
            default:
                throw new ArgumentOutOfRangeException(svgLink.ToString());
        }
    }

    private async void TrainingDatasetImportButton_Click(object sender, RoutedEventArgs e)
    {
        FolderBrowserDialog dialog = new()
        {
            InitialDirectory = _basePath
        };
        if (dialog.ShowDialog() != DialogResult.OK) return;
        string selectedDatasetPath = dialog.SelectedPath;
        DirectoryInfo selectedDatasetPathDirectoryInfo = new(selectedDatasetPath);
        string datasetName = selectedDatasetPathDirectoryInfo.Name;
        string dstPath = _basePath + @"\dataset_raw\" + datasetName;
        await Task.Run(() =>
        {
            Directory.CreateDirectory(dstPath);
            string[] files = Directory.GetFiles(selectedDatasetPath, "*.wav");
            foreach (string file in files)
            {
                FileInfo fileInfo = new(file);
                string newFilePath = dstPath + "\\" + fileInfo.Name;
                File.Copy(file, newFilePath, true);
            }
        });

        ReloadTrainingDataset();
    }

    private void TrainingDatasetReloadButton_Click(object sender, RoutedEventArgs e)
    {
        ReloadTrainingDataset();
    }

    #endregion

    #region Utils

    private void ReloadTrainingDataset()
    {
            List<Speaker> speakerList = new();
            DirectoryInfo datasetDirectoryInfo = new(_basePath + "\\dataset_raw");

            if (datasetDirectoryInfo.GetDirectories().Length == 0)
            {
                TrainingDatasetDataGridSection.Visibility = Visibility.Collapsed;
                TrainingDatasetMessageSection.Visibility = Visibility.Visible;
                return;
            }
            
            TrainingDatasetDataGridSection.Visibility = Visibility.Visible;
            TrainingDatasetMessageSection.Visibility = Visibility.Collapsed;
            foreach (DirectoryInfo directoryInfo in datasetDirectoryInfo.GetDirectories())
            {
                speakerList.Add(new Speaker() { Name = directoryInfo.Name, Count = directoryInfo.GetFiles().Length });
            }

            _viewModel.Speakers = new ObservableCollection<Speaker>(speakerList);
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