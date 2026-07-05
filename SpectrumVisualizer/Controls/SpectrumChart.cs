using System.Windows;
using System.Windows.Media;

namespace SpectrumVisualizer.Controls
{
    public class SpectrumChart : FrameworkElement
    {
        private const double MinFrequencyValue = 90.0;
        private const double MaxFrequencyValue = 110.0;
        private const double MinPowerValue = -120.0;
        private const double MaxPowerValue = -20.0;

        private const double LeftMargin = 55;
        private const double RightMargin = 40;
        private const double BottomMargin = 20;
        private const double TopMargin = 12;

        private double PlotWidth => ActualWidth - LeftMargin - RightMargin;
        private double PlotHeight => ActualHeight - BottomMargin - TopMargin;

        private double XScale => PlotWidth / (MaxFrequency - MinFrequency);
        private double YScale => PlotHeight / (MaxPower - MinPower);

        private double MapX(double freq) => LeftMargin + (freq - MinFrequency) * XScale;
        private double MapY(double power) => TopMargin + PlotHeight - (power - MinPower) * YScale;

        public static readonly DependencyProperty PowersProperty =
            DependencyProperty.Register(
                nameof(Powers),
                typeof(double[]),
                typeof(SpectrumChart),
                new FrameworkPropertyMetadata(null, OnDataChanged));

        public double[] Powers
        {
            get { return (double[])GetValue(PowersProperty); }
            set { SetValue(PowersProperty, value); }
        }

        public static readonly DependencyProperty FrequencyProperty =
        DependencyProperty.Register(
            nameof(Frequency),
            typeof(double[]),
            typeof(SpectrumChart),
            new FrameworkPropertyMetadata(null, OnDataChanged));

        public double[] Frequency
        {
            get { return (double[])GetValue(FrequencyProperty); }
            set { SetValue(FrequencyProperty, value); }
        }

        public static readonly DependencyProperty MinFrequencyProperty =
        DependencyProperty.Register(
        nameof(MinFrequency),
        typeof(double),
        typeof(SpectrumChart),
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
        typeof(SpectrumChart),
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
        typeof(SpectrumChart),
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
        typeof(SpectrumChart),
        new FrameworkPropertyMetadata(MaxPowerValue, OnDataChanged));

        public double MaxPower
        {
            get { return (double)GetValue(MaxPowerProperty); }
            set { SetValue(MaxPowerProperty, value); }
        }

        public static readonly DependencyProperty LineColorProperty =
        DependencyProperty.Register(
        nameof(LineColor),
        typeof(Brush),
        typeof(SpectrumChart),
        new FrameworkPropertyMetadata(Brushes.Yellow, OnDataChanged));

        public Brush LineColor
        {
            get { return (Brush)GetValue(LineColorProperty); }
            set { SetValue(LineColorProperty, value); }
        }

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chart = (SpectrumChart)d;
            chart.InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(Brushes.Black, null, new Rect(0, 0, ActualWidth, ActualHeight));

            if (Powers == null || Frequency == null || Powers.Length != Frequency.Length)
                return;

            DrawGridWithText(drawingContext);

            var points = new Point[Powers.Length];
            for (int i = 0; i < Powers.Length; i++)
            {
                points[i] = new Point(MapX(Frequency[i]), MapY(Powers[i]));
            }

            if (points.Length > 1)
            {
                var geometry = new StreamGeometry();
                using (var ctx = geometry.Open()) // TODO: Check performance, maybe better use another approach.
                {
                    ctx.BeginFigure(points[0], false, false);
                    for (int i = 1; i < points.Length; i++)
                        ctx.LineTo(points[i], true, false);
                }
                geometry.Freeze();
                drawingContext.DrawGeometry(null, new Pen(LineColor, 1), geometry);
            }
        }

        private void DrawGridWithText(DrawingContext drawingContext)
        {
            var gridPen = new Pen(Brushes.Gray, 0.5);

            double[] powerLevels = { -120, -95, -70, -45, -20 };
            foreach (var level in powerLevels)
            {
                double y = MapY(level);
                drawingContext.DrawLine(gridPen, new Point(LeftMargin, y), new Point(ActualWidth, y));
                DrawText(drawingContext, $"{level} dBm", new Point(2, y - 7));
            }

            double[] freqLevels = { 90, 95, 100, 105, 110 };
            foreach (var freq in freqLevels)
            {
                double x = MapX(freq);
                drawingContext.DrawLine(gridPen, new Point(x, TopMargin), new Point(x, TopMargin + PlotHeight));
                DrawText(drawingContext, $"{freq} MHz", new Point(x + 2, TopMargin + PlotHeight + 2));
            }
        }

        private void DrawText(DrawingContext dc, string text, Point position)
        {
            var formattedText = new FormattedText(
                text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"),
                11,
                Brushes.LightGray,
                VisualTreeHelper.GetDpi(this).PixelsPerDip);

            dc.DrawText(formattedText, position);
        }
    }
}

