using SpectrumVisualizer.Models;
using SpectrumVisualizer.Services.Interfaces;

namespace SpectrumVisualizer.Services;

public class SpectrumEngine(ISpectrumDataGenerator generator)
{
    private readonly ISpectrumDataGenerator _generator = generator;
    private Timer? _timer;
    private bool _isRunning;

    public event Action<SpectrumFrame>? FrameGenerated;

    public void Start(int intervalMilliseconds)
    {
        if (_isRunning) return;
        _timer = new Timer(GenerateFrame, null, 0, intervalMilliseconds);// TODO: Maybe better avoid race condition in the future.
        _isRunning = true;
    }

    public void Stop()
    {
        if (!_isRunning) return;
        _timer?.Dispose();
        _isRunning = false;
    }

    private void GenerateFrame(object? state)
    {
        var frame = _generator.GenerateFrame();
        FrameGenerated?.Invoke(frame);
    }
}
