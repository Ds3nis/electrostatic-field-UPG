using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPG_SP_2024
{


    internal class ColourMap
    {
        public float Threshold { get; set; }  // Prahová hodnota
        public Color ColorValue { get; set; }  // Barva pro tento práh

        public ColourMap(float Threshold, Color ColorValue) 
        {
            this.Threshold = Threshold;

            this.ColorValue = ColorValue;
        }

    }
}
