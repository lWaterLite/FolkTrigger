using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace FolkTrigger.ViewModels;

public class Speaker
{
    public string? Name { get; set; }
    public int Count { get; set; }
}

public class SvcPageViewModel: INotifyPropertyChanged
{
    #region ViewModelProperty

    private ObservableCollection<Speaker> _speakers = new();
    private string _compressModelPath = "未选择模型文件";
    private string _compressConfigPath = "未选择配置文件";
    
    public ObservableCollection<Speaker> Speakers
    {
        get => _speakers;
        set
        {
            if (_speakers == value) return;
            _speakers = value;
            OnPropertyChanged(nameof(Speakers));
        }
    }

    public string CompressModelPath
    {
        get => _compressModelPath;
        set
        {
            if (_compressModelPath == value) return;
            _compressModelPath = value;
            OnPropertyChanged(nameof(CompressModelPath));
        }
    }

    public string CompressConfigPath
    {
        get => _compressConfigPath;
        set
        {
            if (_compressConfigPath == value) return;
            _compressConfigPath = value;
            OnPropertyChanged(nameof(CompressConfigPath));
        }
    }

    #endregion
    
    #region PropertyChangedEventHandler

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
}