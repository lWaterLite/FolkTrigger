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
    
    public ObservableCollection<Speaker> Speakers
    {
        get => _speakers;
        set
        {
            if (_speakers != value)
            {
                _speakers = value;
                OnPropertyChanged(nameof(Speakers));
            }
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