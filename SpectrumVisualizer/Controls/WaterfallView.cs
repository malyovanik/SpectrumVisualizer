using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SpectrumVisualizer.Extensions;
using SpectrumVisualizer.Models;

namespace SpectrumVisualizer.Controls
{
    public class WaterfallView : SpectrumControlBase
    {
        private WriteableBitmap? _bitmap;
        private SpectrumFrame? _lastRenderedFrame;

        private const double LeftMargin = 55;
        private const double RightMargin = 25;
        private static readonly Pen GridPen = new(Brushes.Black, 1);
        private static readonly double[] FreqLevels = { 90, 95, 100, 105, 110 };


        public static readonly DependencyProperty HistoryDataProperty =
        DependencyProperty.Register(
        nameof(HistoryData),
        typeof(WaterfallHistory),
        typeof(WaterfallView),
        new FrameworkPropertyMetadata(null, OnDataChanged));

        public WaterfallHistory HistoryData
        {
            get { return (WaterfallHistory)GetValue(HistoryDataProperty); }
            set { SetValue(HistoryDataProperty, value); }
        }

        public static readonly DependencyProperty VersionProperty =
        DependencyProperty.Register(
        nameof(Version),
        typeof(int),
        typeof(WaterfallView),
        new FrameworkPropertyMetadata(0, OnDataChanged));

        public int Version
        {
            get { return (int)GetValue(VersionProperty); }
            set { SetValue(VersionProperty, value); }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();

            double plotWidth = ActualWidth - LeftMargin - RightMargin;
            const int MaxBitmapWidth = 1024;
            const int MaxBitmapHeight = 200;

            if (_bitmap == null)
            {
                _bitmap = new WriteableBitmap(MaxBitmapWidth, MaxBitmapHeight, 96, 96, PixelFormats.Bgr32, null);
            }

            if (HistoryData == null || HistoryData.CountWritedFrames == 0)
            {
                drawingContext.DrawImage(_bitmap, new Rect(LeftMargin, 0, plotWidth, ActualHeight));
                return;
            }

            var newestFrame = HistoryData.GetFrame(0);

            if (!ReferenceEquals(newestFrame, _lastRenderedFrame))// better to compare reference, instead of value
            {
                ShiftBitmapDown();
                DrawNewTopRow(newestFrame!);
                _lastRenderedFrame = newestFrame;
            }

            drawingContext.DrawImage(_bitmap, new Rect(LeftMargin, 0, plotWidth, ActualHeight));

            foreach (var freq in FreqLevels)
            {
                double x = LeftMargin + FrequencyToX(freq, plotWidth);
                drawingContext.DrawLine(GridPen, new Point(x, 0), new Point(x, ActualHeight));
            }

            sw.Stop();
            System.Diagnostics.Debug.WriteLine(sw.ElapsedMilliseconds);
        }

        private void ShiftBitmapDown()
        {
            const int width = 1024;
            const int height = 200;
            int stride = width * 4;

            var buffer = new byte[stride * height];
            _bitmap!.CopyPixels(buffer, stride, 0);

            // shift memory on 1 row below
            Array.Copy(buffer, 0, buffer, stride, stride * (height - 1));

            _bitmap.WritePixels(new Int32Rect(0, 0, width, height), buffer, stride, 0);
        }

        private void DrawNewTopRow(SpectrumFrame frame)
        {
            const int width = 1024;
            var rowPixels = new byte[width * 4];
            int pointCount = frame.Powers.Length;

            for (int x = 0; x < width; x++)
            {
                int pointStart = (int)((double)x / width * pointCount);
                int pointEnd = (int)((double)(x + 1) / width * pointCount);
                pointEnd = Math.Max(pointEnd, pointStart + 1);
                pointEnd = Math.Min(pointEnd, pointCount);

                double maxPower = double.NegativeInfinity;
                for (int p = pointStart; p < pointEnd; p++)
                {
                    if (frame.Powers[p] > maxPower)
                    {
                        maxPower = frame.Powers[p];
                    }
                }

                Color color = TransformPowerToColor(maxPower, MinPower, MaxPower);
                int offset = x * 4;
                rowPixels[offset + 0] = color.B;
                rowPixels[offset + 1] = color.G;
                rowPixels[offset + 2] = color.R;
                rowPixels[offset + 3] = 0;
            }

            _bitmap!.WritePixels(new Int32Rect(0, 0, width, 1), rowPixels, width * 4, 0);
        }

        private static readonly GradientStop[] PowerColorStops =
        {
            new(0.0,  Color.FromRgb(0x00, 0x00, 0xff)),
            new(0.25, Color.FromRgb(0x00, 0xff, 0xff)),
            new(0.5,  Color.FromRgb(0x00, 0xff, 0x00)),
            new(0.75, Color.FromRgb(0xff, 0xff, 0x00)),
            new(1.0,  Color.FromRgb(0xff, 0x00, 0x00))
        };

        private static Color TransformPowerToColor(double powerValue, double minPower, double maxPower)
        {
            double t = powerValue.MapRange(minPower, maxPower, 0.0, 1.0);
            t = Math.Clamp(t, 0.0, 1.0);

            for (int i = 0; i < PowerColorStops.Length - 1; i++)
            {
                var stopA = PowerColorStops[i];
                var stopB = PowerColorStops[i + 1];

                if (t >= stopA.Offset && t <= stopB.Offset)
                {
                    double localT = (t - stopA.Offset) / (stopB.Offset - stopA.Offset);
                    return stopA.Color.Lerp(stopB.Color, localT);
                }
            }

            return PowerColorStops[^1].Color;
        }
    }
}
