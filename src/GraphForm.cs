using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.SkiaSharpView.WinForms;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UPG_SP_2024;
using Timer = System.Windows.Forms.Timer;

namespace ElectricFieldVis
{
    public partial class GraphForm : Form
    {
        private Timer updateTimer;
        public Dictionary<string, ObservableCollection<ObservablePoint>> seriesCollection;
        private const int MaxPointsPerSeries = 300;
        private const int MaxSeriesCount = 5;
        public ProbeManager ProbeManager;
        public GraphForm(ProbeManager probeManager)
        {
            InitializeComponent();
            this.MinimumSize = new Size(600, 450);
            this.ProbeManager = probeManager;
            this.Text = "Závislosti velikosti vektorového pole v místě sondy na čase";
            seriesCollection = new Dictionary<string, ObservableCollection<ObservablePoint>>();
            updateTimer = new Timer();
            updateTimer.Interval = 100; // Оновлення кожні 100 мс
            updateTimer.Tick += UpdateTimer_Tick;
            updateTimer.Start();
        }

        private void UpdateTimer_Tick(object? sender, EventArgs e)
        {
         
            try
            {
                foreach (var series in seriesCollection.Values)
                {
         
                    while (series.Count > MaxPointsPerSeries)
                    {
                        series.RemoveAt(0);
                    }
                }
                chart.Update();
            }
            catch (ObjectDisposedException)
            {
                // Форму уже закрыли, ничего не делаем
            }

        }

        private void GraphForm_Load(object sender, EventArgs e)
        {
            chart.XAxes = new[] { new Axis { Labeler = value => $"{value:0.0}s", Name = "Čas (s)" } };
            chart.YAxes = new[]{
                new Axis
                {
                    Name = "Intenzita pole (N/C)",
                    Labeler = value => value.ToString("0.00E+0")
                }
                };

            chart.Title = new LabelVisual()
            {
                Text = "Závislosti velikosti vektorového pole v místě sondy na čase",
                TextSize = 20,
                Padding = new LiveChartsCore.Drawing.Padding(10),
            };
            chart.LegendPosition = LiveChartsCore.Measure.LegendPosition.Bottom;
        }


        public void AddSeries(string seriesName, Color color)
        {

            if (seriesCollection.Count >= MaxSeriesCount)
            {
                var firstKey = seriesCollection.Keys.First();
                seriesCollection.Remove(firstKey);
                chart.Series = chart.Series.Skip(1).ToArray();
            }


            if (!seriesCollection.ContainsKey(seriesName))
            {
                var values = new ObservableCollection<ObservablePoint>();
                this.seriesCollection[seriesName] = values;
                var strokeColor = new SKColor(color.R, color.G, color.B);
                var newSeries = new LineSeries<ObservablePoint>
                {
                    Values = values,
                    Name = seriesName,
                    Fill = null,
                    Stroke = new SolidColorPaint(strokeColor, 2),
                    GeometrySize = 0,
                    LineSmoothness = 1
                };


                var seriesList = this.chart.Series.ToList();
                seriesList.Add(newSeries);
                chart.Series = seriesList;
                chart.Update();
            }
        }

        public void AppendDataPoint(string seriesName, float value, float elapsedTime)
        {
            if (seriesCollection.TryGetValue(seriesName, out var series))
            {

                series.Add(new ObservablePoint(elapsedTime, value));
            }


        }

        private void updateBtn_Click(object sender, EventArgs e)
        {
            foreach (var series in seriesCollection.Values)
            {
                series.Clear(); // Очищаємо всі серії
            }

            chart.Update(); // Оновлюємо графік
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (this.ProbeManager != null)
            {
                this.ProbeManager.CleanupProbes();
            }
            base.OnFormClosed(e);
         


        }

    }
}


