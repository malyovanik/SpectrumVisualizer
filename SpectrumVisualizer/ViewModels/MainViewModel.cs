using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SpectrumVisualizer.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanStart))]
    [NotifyPropertyChangedFor(nameof(CanStop))]
    [NotifyCanExecuteChangedFor(nameof(StartCommand))]
    [NotifyCanExecuteChangedFor(nameof(StopCommand))]
    private bool isRunning;

    public bool CanStart => !IsRunning;

    public bool CanStop => IsRunning;

    [RelayCommand(CanExecute = nameof(CanStart))]
    private void Start()
    {
        IsRunning = true;
    }

    [RelayCommand(CanExecute = nameof(CanStop))]
    private void Stop()
    {
        IsRunning = false;
    }
}
