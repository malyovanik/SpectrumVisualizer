using SpectrumVisualizer.Models;

namespace SpectrumVisualizer.Services.Interfaces;

public interface ISpectrumDataGenerator
{
    SpectrumFrame GenerateFrame();
}
