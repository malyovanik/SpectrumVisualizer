namespace SpectrumVisualizer.Extensions
{
    public static class DoubleExtensions
    {
        public static double MapRange(this double value, double fromMin, double fromMax, double toMin, double toMax)
        {
            double t = (value - fromMin) / (fromMax - fromMin);
            return toMin + t * (toMax - toMin);
        }
    }
}
