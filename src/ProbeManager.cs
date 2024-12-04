using ElectricFieldVis;
using LiveChartsCore.Defaults;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPG_SP_2024
{
    public class ProbeManager
    {
        public List<ProbeDataCollector> collectors = new List<ProbeDataCollector>();
        public List<Probe> probes = new List<Probe>();
        public GraphForm graphForm { get; private set; }

        public ProbeManager(GraphForm graphForm = null)
        {
            this.graphForm = graphForm ?? new GraphForm(this);
        }

        public void AddProbe(Probe probe, string seriesName)
        {
            setGraphForm();
            probes.Add(probe);
            //// Додаємо серію до графіка
            graphForm.AddSeries(seriesName, probe.ProbeColor);

            // Створюємо новий збирач даних для зонда
            var collector = new ProbeDataCollector(probe, seriesName);
            collectors.Add(collector);
        }

        public void UpdateProbes()
        {
            setGraphForm();
            foreach (var collector in collectors)
            {
                collector.CollectData(graphForm);
            }

        }

        public void setGraphForm()
        {
            if (this.graphForm == null)
            {
                this.graphForm = new GraphForm(this);
            }
        }

        public void CleanupProbes()
        {

            //if (probes.Count > 1)
            //{
            //    probes.RemoveRange(1, probes.Count - 1);
            //}


            //if (collectors.Count > 1)
            //{
            //    collectors.RemoveRange(1, collectors.Count - 1);
            //}
            probes.Clear();
            collectors.Clear();

            if (graphForm != null)
            {
                //var firstSeries = graphForm.seriesCollection.FirstOrDefault();
                //ObservableCollection<ObservablePoint> points = firstSeries.Value;

                graphForm.seriesCollection.Clear();

                //if (firstSeries.Value != null)
                //{
                //    graphForm.seriesCollection[firstSeries.Key] = points;
                //}

                graphForm.Update();
                this.graphForm = new GraphForm(this);
            }


        }
    }
}
