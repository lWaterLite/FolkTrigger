using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Design;
using System.Windows.Markup;
using Microsoft.Win32;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;

namespace FolkTrigger.Pages;

public partial class CnnPage : Page
{

    private static readonly string BasePath = AppDomain.CurrentDomain.BaseDirectory + @"MetaFolk_CNN\";

    private string _datasetName = string.Empty;

    private readonly CnnPageViewModel _viewModel;

    private readonly bool _projectDirExist;
    
    public CnnPage()
    {
        InitializeComponent();
        
        _viewModel = (MainGrid.DataContext as CnnPageViewModel)!;
        _projectDirExist = CheckProjectDirExist();
        if (!_projectDirExist) return;

        ReloadDataset();
        Task.Run(() => _viewModel.InferenceDataCount = ReloadInferenceData());
    }

    #region RoutedEventMethods

    private void CnnPageLinkButton_Click(object sender, RoutedEventArgs eventArgs)
    {
        if (!_projectDirExist) return;
        
        if(sender is not Button button)
            return;
        if (button.Tag is not CnnLink cnnLink)
            return;

        switch (cnnLink)
        {
            case CnnLink.Readme:
                Process.Start("explorer", BasePath + "readme.md");
                break;
            case CnnLink.Root:
                Process.Start("explorer", BasePath);
                break;
            case CnnLink.Raw:
                Process.Start("explorer", BasePath + "raw");
                break;
            case CnnLink.Wav:
                Process.Start("explorer", BasePath + "wav");
                break;
            case CnnLink.Dataset:
                Process.Start("explorer", BasePath + "dataset");
                break;
            case CnnLink.Config:
                Process.Start("explorer", BasePath + "config");
                break;
            case CnnLink.Logs:
                Process.Start("explorer", BasePath + "logs");
                break;
            case CnnLink.Inference:
                Process.Start("explorer", BasePath + "inference");
                break;
            default:
                return;
        }
    }

    private void CnnPageRefreshButton_Click(object sender, RoutedEventArgs eventArgs)
    {
        if (!_projectDirExist) return;
        
        ReloadDataset();
    }

    private async void CnnPageLoadButton_Click(object sender, RoutedEventArgs eventArgs)
    {
        if (!_projectDirExist) return;
        
        FolderBrowserDialog folderBrowserDialog = new();
        folderBrowserDialog.InitialDirectory = BasePath;
        folderBrowserDialog.ShowDialog();
        string selectedPath = folderBrowserDialog.SelectedPath;
        string datasetName = Path.GetFileName(selectedPath);
        
        await Task.Run(() => GetFilesAndDirs(selectedPath, BasePath + $@"raw\{datasetName}\"));
        ReloadDataset();
    }

    private async void PreprocessButton_Click(object sender, RoutedEventArgs eventArgs)
    {
        if (!_projectDirExist) return;
        
        string virtualenvCommand = BasePath + @"venv\Scripts\activate";
        string configCommand = $"python {BasePath + "1_gen_config.py"} --dataset={_datasetName}";
        string preprocessCommand =
            $"python {BasePath + "2_dataset_preprocess.py"} " +
            $"--audio-format={((string)AudioTypeComboBox.SelectedItem).ToLower()} " +
            $"{((bool)ConverterSwitcher.IsChecked! ? "" : "--no-convert")} " +
            $"{((bool)SlicerSwitcher.IsChecked! ? "" : "--no-slice")}";
        ProcessStartInfo start = new()
        {
            FileName = "cmd.exe",
            Arguments = $"/c \"{virtualenvCommand} " +
                        $"& {Echo(configCommand)} & {configCommand} " +
                        $"& {Echo(preprocessCommand)} & {preprocessCommand} & pause\"",
            UseShellExecute = true, // 设置为false以重定向输入输出
            CreateNoWindow = false // 不创建新窗口
        };

        using Process process = new();
        process.StartInfo = start;
        process.Start();

        await process.WaitForExitAsync(); // 等待进程结束
        return;
        
        string Echo(string executedCommand) => $"echo {"executing command: " + executedCommand}";
    }

    private async void TrainingButton_Click(object sender, RoutedEventArgs eventArgs)
    {
        if (!_projectDirExist) return;
        
        string virtualenvCommand = BasePath + @"venv\Scripts\activate";
        string trainingCommand = $"python {BasePath + "3_train.py"} " +
                                 $"{((bool)ValidateSwitcher.IsChecked! ? "" : "--no-validate")} " +
                                 $"{((bool)RecoverSwitcher.IsChecked! ? "" : "--no-recover")}";
        ProcessStartInfo start = new()
        {
            FileName = "cmd.exe",
            Arguments = $"/c \"echo {"executing command: " + trainingCommand} " +
                        $"& {virtualenvCommand} & {trainingCommand} & pause\""
        };

        using Process process = new();
        process.StartInfo = start;
        process.Start();

        await process.WaitForExitAsync();
    }

    private void SelectCompressModelButton_Click(object sender, RoutedEventArgs e)
    {
        if (!_projectDirExist) return;
        
        OpenFileDialog dialog = new()
        {
            InitialDirectory = BasePath,
            Filter = "Checkpoint File (.ckpt)|*.ckpt"
        };
        if (dialog.ShowDialog() != true) return;
        _viewModel.SelectedCompressModelPath = dialog.FileName;
    }

    private async void CompressModelButton_Click(object sender, RoutedEventArgs e)
    {
        if (!_projectDirExist) return;
        
        string virtualenvCommand = BasePath + @"venv\Scripts\activate";
        string compressCommand = $"python {BasePath + "4_compress_model.py"} " +
                                 $"-m={_viewModel.SelectedCompressModelPath}";
        ProcessStartInfo start = new()
        {
            FileName = "cmd.exe",
            Arguments = $"/c \"echo {"executing command: " + compressCommand} " +
                        $"& {virtualenvCommand} & {compressCommand} & pause\""
        };

        using Process process = new();
        process.StartInfo = start;
        process.Start();

        await process.WaitForExitAsync();
    }
    
    private void SelectInferenceModelButton_Click(object sender, RoutedEventArgs e)
    {
        if (!_projectDirExist) return;
        
        OpenFileDialog dialog = new()
        {
            InitialDirectory = BasePath,
            Filter = "Checkpoint File (.ckpt)|*.ckpt;*.pt"
        };
        if (dialog.ShowDialog() != true) return;
        _viewModel.SelectedInferenceModelPath = dialog.FileName;
    }

    private async void ReloadInferenceDataButton_Click(object sender, RoutedEventArgs e)
    {
        if (!_projectDirExist) return;
        
        await Task.Run(() => _viewModel.InferenceDataCount = ReloadInferenceData());
    }

    private async void InferenceButton_Click(object sender, RoutedEventArgs e)
    {
        if (!_projectDirExist) return;
        
        string virtualenvCommand = BasePath + @"venv\scripts\activate";
        string inferenceCommand = $"python {BasePath + "5_inference.py"} " +
                                  $"-m={_viewModel.SelectedInferenceModelPath}";
        ProcessStartInfo start = new()
        {
            FileName = "cmd.exe",
            Arguments = $"/c \"echo {"executing command: " + inferenceCommand} " +
                        $"& {virtualenvCommand} & {inferenceCommand} & pause\""
        };

        using Process process = new();
        process.StartInfo = start;
        process.Start();

        await process.WaitForExitAsync();
    }

    #endregion

    #region Utils
    
    private void GetFilesAndDirs(string srcDir,string destDir)
    {
        if (Directory.Exists(destDir)) return; //若目标文件夹不存在
        Directory.CreateDirectory(destDir);//创建目标文件夹                                                  
        string[] files = Directory.GetFiles(srcDir);//获取源文件夹中的所有文件完整路径
        foreach (string path in files)          //遍历文件     
        {
            FileInfo fileInfo = new(path);
            string newPath = destDir + fileInfo.Name;
            File.Copy(path, newPath, true);
        }
        string[] dirs = Directory.GetDirectories(srcDir);
        foreach (string path in dirs)        //遍历文件夹
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            string newDir = destDir + directory.Name;
            GetFilesAndDirs(path+"\\", newDir+"\\");
        }
    }

    private void ReloadDataset()
    {
        DirectoryInfo directoryInfo = new(BasePath + "raw");
        DirectoryInfo[] subDirectories = directoryInfo.GetDirectories();

        switch (subDirectories.Length)
        {
            case 0:
                _datasetName = string.Empty;
                _viewModel.DatasetImportText = "错误: 未检测到数据集，请手工或点击按钮导入数据集";
                return;
            case 1:
            {
                DirectoryInfo[] tagDirectories = subDirectories[0].GetDirectories();
                if (tagDirectories.Length == 0)
                {
                    _datasetName = string.Empty;
                    _viewModel.DatasetImportText = "错误: 检测到了不符合格式的数据集根目录，请重新导入数据集";
                    return;
                }
                _datasetName = subDirectories[0].Name;
                _viewModel.DatasetImportText = "成功导入数据集。\n当前数据集为: " + _datasetName;
                return;
            }
        }

        if (subDirectories.Select(subDirectory => subDirectory.GetDirectories()).Any(tagDirectories => tagDirectories.Length != 0))
        {
            _datasetName = subDirectories[0].Name;
            _viewModel.DatasetImportText = "警告: 检测到多个数据集，仅读取第一个符合要求数据集。\n当前数据集为: " + _datasetName;
            return;
        }

        _datasetName = string.Empty;
        _viewModel.DatasetImportText = "错误: 检测到了多个数据集根目录，但是没有一个符合格式要求，请重新导入一个数据集";
    }

    // #region DatasetTreeView
    //
    // private List<Border> CreateTagCard(string datasetName)
    // {
    //     List<Border> tagCards = new List<Border>();
    //     Style? borderStyle = FindResource("TextCardBorder") as Style;
    //     Style? textStyle = FindResource("TextCardText") as Style;
    //     
    //     DirectoryInfo rootDirectoryInfo = new(BasePath + "raw\\" + datasetName);
    //     DirectoryInfo[] tagDirectoryInfos = rootDirectoryInfo.GetDirectories();
    //     foreach (DirectoryInfo tagDirectoryInfo in tagDirectoryInfos)
    //     {
    //         string tagName = tagDirectoryInfo.Name;
    //         int fileNumber = tagDirectoryInfo.GetFiles().Length;
    //         string text = $"标签: {tagName}, 数量: {fileNumber}";
    //         TextBlock textBlock = new() 
    //         {
    //             Style = textStyle,
    //             Text = text
    //         };
    //         Border border = new()
    //         {
    //
    //             Style = borderStyle,
    //             Child = textBlock
    //         };
    //         tagCards.Add(border);
    //     }
    //
    //     return tagCards;
    // }
    //
    // #endregion

    private int ReloadInferenceData()
    {
        DirectoryInfo directoryInfo = new(BasePath + "inference\\raw");
        int count = directoryInfo.EnumerateFiles("*.wav", SearchOption.TopDirectoryOnly).Count();
        return count;
    }

    private bool CheckProjectDirExist()
    {
        if (Directory.Exists(BasePath)) return true;
        Utils.Utils.ExceptionHandler(new Exception("Error: The sub project MetaFolk_CNN is missing!"), WarningTextBlock);
        return false;
    }

    #endregion

}

public enum CnnLink
{
    Readme,
    Root,
    Config,
    Dataset,
    Inference,
    Logs,
    Raw,
    Wav
}

public enum AudioType
{
    Mp3,
    Aac,
    Flac,
    Wav
}

public class CnnPageViewModel: INotifyPropertyChanged
{ 
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #region ViewModelProperty

    private string _selectedAudioType = "Mp3";
    private string _selectedCompressModelPath = "未选择模型";
    private string _selectedInferenceModelPath = "未选择模型";
    private int _inferenceDataCount;
    private string _datasetImportText = "未导入数据集";

    public string SelectedAudioType{
        get => _selectedAudioType;
        set
        {
            if (_selectedAudioType == value) return;
            _selectedAudioType = value;
            OnPropertyChanged(nameof(SelectedAudioType));
        }
    }

    public string SelectedCompressModelPath
    {
        get => _selectedCompressModelPath;
        set
        {
            if (_selectedCompressModelPath == value) return;
            _selectedCompressModelPath = value;
            OnPropertyChanged(nameof(SelectedCompressModelPath));
        }
    }

    public string SelectedInferenceModelPath
    {
        get => _selectedInferenceModelPath;
        set
        {
            if (_selectedInferenceModelPath == value) return;
            _selectedInferenceModelPath = value;
            OnPropertyChanged(nameof(SelectedInferenceModelPath));
        }
    }

    public int InferenceDataCount
    {
        get => _inferenceDataCount;
        set
        {
            if (_inferenceDataCount == value) return;
            _inferenceDataCount = value;
            OnPropertyChanged(nameof(InferenceDataCount));
        }
    }

    public string DatasetImportText
    {
        get => _datasetImportText;
        set
        {
            if (_datasetImportText == value) return;
            _datasetImportText = value;
            OnPropertyChanged(nameof(DatasetImportText));
        }
    }

    #endregion
    
}

// #region DatasetTreeView
//
// public class TagFolder
// {
//     private ObservableCollection<DatasetItem> _datasetItems;
//
//     public ObservableCollection<DatasetItem> DatasetItems
//     {
//         get => _datasetItems;
//         set => _datasetItems = value ?? throw new ArgumentNullException(nameof(value));
//     }
//
//     public string? FolderName { get; }
//
//
//     public TagFolder(string? folderName)
//     {
//         FolderName = folderName;
//         _datasetItems = new ObservableCollection<DatasetItem>();
//     }
// }
//
// public class DatasetItem
// {
//     private readonly string? _itemName;
//
//     public string? ItemName
//     {
//         get => _itemName;
//         private init => _itemName = value ?? throw new ArgumentNullException(nameof(value));
//     }
//
//
//     public DatasetItem(string? itemName)
//     {
//         ItemName = itemName;
//     }
// }
//
// #endregion
