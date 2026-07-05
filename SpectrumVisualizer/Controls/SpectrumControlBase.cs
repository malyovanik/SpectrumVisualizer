using System.Windows;
using SpectrumVisualizer.Extensions;

namespace SpectrumVisualizer.Controls
{
    public abstract class SpectrumControlBase : FrameworkElement
    {
        private const double MinFrequencyValue = 90.0;
        private const double MaxFrequencyValue = 110.0;
        private const double MinPowerValue = -120.0;
        private const double MaxPowerValue = -20.0;

        public static readonly DependencyProperty MinFrequencyProperty =
            DependencyProperty.Register(
                nameof(MinFrequency),
                typeof(double),
                typeof(SpectrumControlBase),
                new FrameworkPropertyMetadata(MinFrequencyValue, OnDataChanged));

        public double MinFrequency
        {
            get { return (double)GetValue(MinFrequencyProperty); }
            set { SetValue(MinFrequencyProperty, value); }
        }

        public static readonly DependencyProperty MaxFrequencyProperty =
            DependencyProperty.Register(
                nameof(MaxFrequency),
                typeof(double),
                typeof(SpectrumControlBase),
                new FrameworkPropertyMetadata(MaxFrequencyValue, OnDataChanged));

        public double MaxFrequency
        {
            get { return (double)GetValue(MaxFrequencyProperty); }
            set { SetValue(MaxFrequencyProperty, value); }
        }

        public static readonly DependencyProperty MinPowerProperty =
            DependencyProperty.Register(
                nameof(MinPower),
                typeof(double),
                typeof(SpectrumControlBase),
                new FrameworkPropertyMetadata(MinPowerValue, OnDataChanged));

        public double MinPower
        {
            get { return (double)GetValue(MinPowerProperty); }
            set { SetValue(MinPowerProperty, value); }
        }

        public static readonly DependencyProperty MaxPowerProperty =
            DependencyProperty.Register(
                nameof(MaxPower),
                typeof(double),
                typeof(SpectrumControlBase),
                new FrameworkPropertyMetadata(MaxPowerValue, OnDataChanged));

        public double MaxPower
        {
            get { return (double)GetValue(MaxPowerProperty); }
            set { SetValue(MaxPowerProperty, value); }
        }

        protected static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SpectrumControlBase)d).InvalidateVisual();
        }

        protected double FrequencyToX(double frequency, double width)
        {
            return frequency.MapRange(MinFrequency, MaxFrequency, 0, width);
        }

        protected double PowerToY(double power, double height)
        {
            return power.MapRange(MinPower, MaxPower, height, 0);
        }
    }
}
