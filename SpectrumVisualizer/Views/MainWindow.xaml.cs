using SpectrumVisualizer.Controls;
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
        LoadFakeData();
    }

    private void LoadFakeData()
    {
        const int pointCount = 1024;
        const double minFreq = 90.0;
        const double maxFreq = 130.0; // change to 110(default),
        const double basePower = -80.0;

        var frequencies = new double[pointCount];
        var powers = new double[pointCount];

        var random = new Random(42);

        double step = (maxFreq - minFreq) / (pointCount - 1);

        for (int i = 0; i < pointCount; i++)
        {
            frequencies[i] = minFreq + i * step;

            double wave = Math.Sin(i * 0.05) * 10 + Math.Sin(i * 0.2) * 5;
            double noise = (random.NextDouble() - 0.5) * 8;

            powers[i] = basePower + wave + noise;
        }

        SpectrumChartControl.Frequency = frequencies;
        SpectrumChartControl.Powers = powers;
        SpectrumChartControl.LineColor = Brushes.Yellow;
    }
}
