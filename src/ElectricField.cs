using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UPG_SP_2024;

namespace UPG_SP_2024
{
    /// <summary>
    /// Třída pro výpočet intenzity elektrického pole v určité souřadnici
    /// </summary>
    public class ElectricField
    {
        // Hodnota permittivity vakua.
        public const float Epsilon_0 = (float)(8.854e-12);

        /// <summary>
        /// Vypočítá intenzitu v daném bodě na základě seznamu elektrických nábojů.
        /// </summary>
        /// <param name="charges">Seznam nábojů typu <see cref="Charge"/>, které ovlivňují elektrické pole.</param>
        /// <param name="x">X-ová souřadnice zondy, kde se elektrické pole vypočítá.</param>
        /// <param name="y">Y-ová souřadnice zondy, kde se elektrické pole vypočítá.</param>
        /// <returns>Vrátí bod typu <see cref="PointF"/> představující vektor elektrického pole.</returns>
        public static PointF CalculateField(List<Charge> charges, float x, float y, float scale)
        {
             
            PointF field = new PointF(0, 0);  

            foreach (Charge charge in charges)
            {
                float scaleX = x / (scale - charge.Radius);
                float scaleY = y / (scale - charge.Radius);


                float dx = charge.X - scaleX;
                float dy = charge.Y - scaleY;
                float r = (float)Math.Sqrt(dx * dx + dy * dy); 
                float vectorToCube = r * r * r;
              
                if (r == 0) continue;


                // Výpočet síly elektrického pole podle Coulombova zákona.
                float force =  -charge.Q / (4 * (float)Math.PI * Epsilon_0 * (vectorToCube));


                // Aktualizace složek elektrického pole.
                field.X += force * dx;
                field.Y += force * dy;
            }

            return field;
        }


    }
}






