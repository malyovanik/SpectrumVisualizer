namespace SpectrumVisualizer.Models;

public class WaterfallHistory(int capacity)
{
    private readonly SpectrumFrame?[] _buffer = new SpectrumFrame?[capacity];
    private readonly int _capacity = capacity;
    private int _head;
    private int _countWritedFrames;

    public void AddFrame(SpectrumFrame frame)
    {
        _buffer[_head] = frame;
        if (_countWritedFrames < _capacity) {
            _countWritedFrames++;
        }
        _head = (_head + 1) % _capacity;
    }

    public SpectrumFrame? GetFrame(int index)
    {
        if (index < 0 || index >= _countWritedFrames)
            return null;

        return _buffer[(_head - 1 - index + _capacity) % _capacity];
    }

    public int CountWritedFrames => _countWritedFrames;
}
