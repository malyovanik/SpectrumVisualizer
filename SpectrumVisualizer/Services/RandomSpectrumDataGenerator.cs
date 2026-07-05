using SpectrumVisualizer.Models;
using SpectrumVisualizer.Services.Interfaces;
namespace SpectrumVisualizer.Services;

class RandomSpectrumDataGenerator : ISpectrumDataGenerator
{
    private const int PointCount = 1024;
    private readonly double[] _frequencies;
    private readonly Random _random = new();
    private int _frameCounter;

    public RandomSpectrumDataGenerator()
    {
        _frequencies = GenerateFrequencies(PointCount);
    }

    public SpectrumFrame GenerateFrame()
    {
        double phaseShift = _frameCounter * 0.1;
        var powers = GeneratePowers(PointCount, _random, phaseShift);
        _frameCounter++;

        return new SpectrumFrame(powers, _frequencies, DateTime.Now);
    }

    private static double[] GenerateFrequencies(int pointCount)
    {
        const double MaxFreq = 110.0;
        const double MinFreq = 90.0;
        var frequencies = new double[pointCount];
        double step = (MaxFreq - MinFreq) / (pointCount - 1);

        for (int i = 0; i < pointCount; i++)
        {
            frequencies[i] = MinFreq + i * step;
        }

        return frequencies;
    }

    private static double[] GeneratePowers(int pointCount, Random random, double phaseShift)
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
