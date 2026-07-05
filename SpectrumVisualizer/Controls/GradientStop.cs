using System.Windows.Media;

namespace SpectrumVisualizer.Controls
{
    public readonly record struct GradientStop(double Offset, Color Color);
}
