using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace InventarioWPF
{
    public static class Helpers
    {

        public static bool IsWindowOpen<T>(string name = "") where T : Window
        {
            return string.IsNullOrEmpty(name)
                ? Application.Current.Windows.OfType<T>().Any()
                : Application.Current.Windows.OfType<T>().Any(w => w.Name.Equals(name));
        }

        public static Window ReturnWindowOpen<T>() where T : Window
        {
            return Application.Current.Windows.Cast<Window>().FirstOrDefault(x => x is T);
        }

        public static int FindIndexByText(ComboBox comboBox, string text)
        {
            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                DataRowView item = (DataRowView)comboBox.Items[i];
                if (item[1].Equals(text))
                {
                    return i;
                }
            }

            return -1;
        }
        public static bool validarEmail(string email)
        {
            if (email.Length > 0)
            {
                try
                {
                    new MailAddress(email);
                    return true;
                }
                catch (FormatException)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static void SaveToPNG(FrameworkElement visual, string fileName)
        {
            var encoder = new PngBitmapEncoder();
            EncodeVisual(visual, fileName, encoder);
        }

        private static void EncodeVisual(FrameworkElement visual, string fileName, BitmapEncoder encoder)
        {
            var bitmap = new RenderTargetBitmap((int)visual.ActualWidth, (int)visual.ActualHeight + 20, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            var frame = BitmapFrame.Create(bitmap);
            encoder.Frames.Add(frame);
            using (var stream = File.Create("C:\\Users\\"+ Environment.UserName + "\\Desktop\\" + fileName)) encoder.Save(stream);
        }

        public static bool ChartToImage(this CartesianChart cartesianChart, LiveCharts.SeriesCollection data, Axis AxisX, Axis AxisY, double Width, double Height, string fileName, string targetPath, out Exception returnEx)
        {
            bool status = false;
            returnEx = null;           
            try
            {
                var myChart = new CartesianChart
                {
                    DisableAnimations = true,
                    Width = Width,
                    Height = Height,
                    Series = data
                };
                myChart.AxisX.Add(new Axis { IsMerged = AxisX.IsMerged, FontSize = AxisX.FontSize, FontWeight = AxisX.FontWeight, Foreground = AxisX.Foreground, Separator = new LiveCharts.Wpf.Separator { Step = AxisX.Separator.Step, StrokeThickness = AxisX.Separator.StrokeThickness, StrokeDashArray = AxisX.Separator.StrokeDashArray, Stroke = AxisX.Separator.Stroke }, Title = AxisX.Title, MinValue = AxisX.MinValue, MaxValue = AxisX.MaxValue });
                myChart.AxisY.Add(new Axis { IsMerged = AxisY.IsMerged, FontSize = AxisY.FontSize, FontWeight = AxisY.FontWeight, Foreground = AxisY.Foreground, Separator = new LiveCharts.Wpf.Separator { Step = AxisY.Separator.Step, StrokeThickness = AxisY.Separator.StrokeThickness, StrokeDashArray = AxisY.Separator.StrokeDashArray, Stroke = AxisX.Separator.Stroke }, Title = AxisY.Title, MinValue = AxisY.MinValue, MaxValue = AxisY.MaxValue });


                var viewbox = new Viewbox();
                viewbox.Child = myChart;
                viewbox.Measure(myChart.RenderSize);
                viewbox.Arrange(new Rect(new System.Windows.Point(0, 0), myChart.RenderSize));
                myChart.Update(true, true); //force chart redraw
                viewbox.UpdateLayout();

                var encoder = new PngBitmapEncoder();
                var bitmap = new RenderTargetBitmap((int)myChart.ActualWidth, (int)myChart.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                bitmap.Render(myChart);
                var frame = BitmapFrame.Create(bitmap);
                encoder.Frames.Add(frame);
                string path = Path.Combine(targetPath, fileName);
                using (var stream = File.Create(path))
                {
                    encoder.Save(stream);
                }
                myChart = null;
                viewbox = null;
                encoder = null;
                bitmap = null;
                frame = null;
                status = true;
            }
            catch (Exception ex)
            {
                returnEx = ex;
                status = false;
            }

            return status;
        }
    }
}
