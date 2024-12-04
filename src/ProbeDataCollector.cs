using ElectricFieldVis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPG_SP_2024
{
    public class ProbeDataCollector
    {
        private Probe probe;
        private string seriesName;

        public ProbeDataCollector(Probe probe, string seriesName)
        {
            this.probe = probe;
            this.seriesName = seriesName;
        }

        public void CollectData(GraphForm graphForm)
        {
            // Отримуємо величину електричного поля 
            float fieldMagnitude = probe.currentFieldMagnitude();
            float elapsedTime = (float)(DateTime.Now - probe.CreationTime).TotalSeconds; // Час з моменту створення

            //// Додаємо точку в серію, що відповідає цьому зонду
            graphForm.AppendDataPoint(seriesName, fieldMagnitude, elapsedTime);
           
        }
    }
}
