using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpectrumVisualizer.Models;
using SpectrumVisualizer.Services;

namespace SpectrumVisualizer.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly SpectrumEngine _spectrumEngine;

    [ObservableProperty]
    private double[]? currentPowers;

    [ObservableProperty]
    private double[]? currentFrequencies;

    [ObservableProperty]
    private WaterfallHistory waterfallHistory = new(capacity: 200);

    [ObservableProperty]
    private bool canStart = true;

    [ObservableProperty]
    private bool canStop;

    public MainViewModel()
    {
        var generator = new RandomSpectrumDataGenerator();
        _spectrumEngine = new SpectrumEngine(generator);
        _spectrumEngine.FrameGenerated += OnFrameGenerated;
    }

    [ObservableProperty]
    private int waterfallVersion;

    private void OnFrameGenerated(SpectrumFrame frame)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            CurrentPowers = frame.Powers;
            CurrentFrequencies = frame.Frequencies;

            WaterfallHistory.AddFrame(frame);
            WaterfallVersion++;
        });
    }

    [RelayCommand]
    private void Start()
    {
        _spectrumEngine.Start(intervalMilliseconds: 50);
        CanStart = false;
        CanStop = true;
    }

    [RelayCommand]
    private void Stop()
    {
        _spectrumEngine.Stop();
        CanStart = true;
        CanStop = false;
    }
}