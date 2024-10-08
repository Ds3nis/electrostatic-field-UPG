using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPG_SP_2024
{
    public class ElectricField
    {

        public const float Epsilon_0 = (float)(8.854e-12);
        // Метод для обчислення вектора інтенсивності електричного поля в точці (x, y)
        public static PointF CalculateField(List<Charge> charges, float x, float y)
        {
             
            PointF field = new PointF(0, 0);  

            foreach (Charge charge in charges)
            {
                // Обчислення відстані між зарядом та точкою (x, y)
                float dx = charge.X - x;
                float dy = charge.Y - y;
                float r = (float)Math.Sqrt(dx * dx + dy * dy);  // Відстань
                float vectorToCube = r * r * r;
              
                if (r == 0) continue;

           
                float force = charge.Q / (4 * (float)Math.PI * Epsilon_0 * (vectorToCube));

           
                field.X += force * dx;
                field.Y += force * dy;
            }

            return field;
        }


    }
}

