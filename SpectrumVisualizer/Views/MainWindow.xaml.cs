using SpectrumVisualizer.Models;
using SpectrumVisualizer.ViewModels;
using System.Windows;
using System.Windows.Media;

namespace SpectrumVisualizer.Views;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new MainViewModel();
        DataContext = _viewModel;
        Closed += (s, e) => _viewModel.Dispose();
    }
}