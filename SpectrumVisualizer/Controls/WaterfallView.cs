using System;
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
        private const int BitmapWidth = 1024;
        private const int BitmapHeight = 200;

        private const double LeftMargin = 55;
        private const double RightMargin = 25;

        public static readonly DependencyProperty HistoryDataProperty =
        DependencyProperty.Register(
        nameof(HistoryData),
        typeof(WaterfallHistory),
        typeof(WaterfallView),
        new FrameworkPropertyMetadata(null, OnDataChanged));

        public WaterfallHistory HistoryData
        {
            get { return (WaterfallHistory )GetValue(HistoryDataProperty); }
            set { SetValue(HistoryDataProperty, value); }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            _bitmap ??= new WriteableBitmap(BitmapWidth, BitmapHeight, 96, 96, PixelFormats.Bgr32, null);

            if (HistoryData == null || HistoryData.CountWritedFrames == 0)
            {
                drawingContext.DrawImage(_bitmap, new Rect(0, 0, ActualWidth, ActualHeight));
                return;
            }

            var rowPixels = new byte[BitmapWidth * 4];

            int rowCount = Math.Min(HistoryData.CountWritedFrames, BitmapHeight);

            for (int row = 0; row < rowCount; row++)
            {
                double[] frame = HistoryData.GetFrame(row)?.Powers ?? [];

                for (int x = 0; x < BitmapWidth; x++)
                {
                    int sourceIndex = (int)((double)x / BitmapWidth * frame.Length);
                    sourceIndex = Math.Min(sourceIndex, frame.Length - 1);

                    double power = frame[sourceIndex];
                    Color color = TransformPowerToColor(power, MinPower, MaxPower);

                    int offset = x * 4;
                    rowPixels[offset + 0] = color.B;
                    rowPixels[offset + 1] = color.G;
                    rowPixels[offset + 2] = color.R;
                    rowPixels[offset + 3] = 0;
                }

                _bitmap.WritePixels(
                    new Int32Rect(0, row, BitmapWidth, 1),
                    rowPixels,
                    BitmapWidth * 4,
                    0);
            }

            double plotWidth = ActualWidth - LeftMargin - RightMargin;
            drawingContext.DrawImage(_bitmap, new Rect(LeftMargin, 0, plotWidth, ActualHeight));

            var gridPen = new Pen(Brushes.Black, 1);
            double[] freqLevels = { 90, 95, 100, 105, 110 };

            foreach (var freq in freqLevels)
            {
                double x = LeftMargin + FrequencyToX(freq, plotWidth);
                drawingContext.DrawLine(gridPen, new Point(x, 0), new Point(x, ActualHeight));
            }
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
