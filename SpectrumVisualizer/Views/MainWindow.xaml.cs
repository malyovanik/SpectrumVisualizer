using SpectrumVisualizer.Models;
using SpectrumVisualizer.ViewModels;
using System.Windows;
using System.Windows.Media;

namespace SpectrumVisualizer.Views;

public partial class MainWindow : Window
{
    private const int PointCount = 1024;
    private const double MinFreq = 90.0;
    private const double MaxFreq = 110.0;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
        LoadFakeSpectrumData();
        LoadFakeWaterfallData();
    }

    private void LoadFakeSpectrumData()
    {
        var random = new Random(42);
        var frequencies = GenerateFrequencies(PointCount, MinFreq, MaxFreq);
        var powers = GeneratePowers(PointCount, random);

        SpectrumChartControl.Frequency = frequencies;
        SpectrumChartControl.Powers = powers;
        SpectrumChartControl.LineColor = Brushes.Yellow;
    }

    private void LoadFakeWaterfallData()
    {
        const int frameCount = 200;

        var random = new Random(42);
        var frequencies = GenerateFrequencies(PointCount, MinFreq, MaxFreq);
        var history = new WaterfallHistory(frameCount);

        for (int frame = 0; frame < frameCount; frame++)
        {
            var powers = GeneratePowers(PointCount, random, phaseShift: frame * 0.1);
            history.AddFrame(new SpectrumFrame(powers, frequencies, DateTime.Now));
        }

        WaterfallControl.HistoryData = history;
    }

    private static double[] GenerateFrequencies(int pointCount, double minFreq, double maxFreq)
    {
        var frequencies = new double[pointCount];
        double step = (maxFreq - minFreq) / (pointCount - 1);

        for (int i = 0; i < pointCount; i++)
            frequencies[i] = minFreq + i * step;

        return frequencies;
    }

    private static double[] GeneratePowers(int pointCount, Random random, double phaseShift = 0)
    {
        var powers = new double[pointCount];
        const double basePower = -80.0;

        for (int i = 0; i < pointCount; i++)
        {
            double wave = Math.Sin(i * 0.05 + phaseShift) * 10 + Math.Sin(i * 0.2 + phaseShift * 2) * 5;
            double noise = (random.NextDouble() - 0.5) * 8;
            powers[i] = basePower + wave + noise;
        }

        return powers;
    }
}