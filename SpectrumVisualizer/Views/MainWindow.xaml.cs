using System.Windows;
using SpectrumVisualizer.ViewModels;

namespace SpectrumVisualizer.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}
