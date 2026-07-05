using SpectrumVisualizer.Models;
using SpectrumVisualizer.ViewModels;
using System.Windows;
using System.Windows.Media;

namespace SpectrumVisualizer.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}