using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UPG_SP_2024
{



    public class Charge
    {
        private float _x;
        private float _y;
        private float _q;

        public float X
        {
            get { return _x; }
            set { _x = value; }
        }

        public float Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public float Q
        {
            get { return _q; }
            set { _q = value; }
        }


        public Charge(float x, float y, float Q)
        {
            this.X = x;
            this.Y = y;
            this.Q = Q;
        }


        private PointF WorldToScreen(float worldX, float worldY, int panelWidth, int panelHeight, float maxRadius)
        {
            // Визначаємо мінімальний масштаб для збереження пропорцій
            /*float scaleX = panelWidth / 2f;
            float scaleY = panelHeight / 2f;
            float scale = Math.Min(scaleX, scaleY);*/
            // Визначаємо масштаб з урахуванням відступів, щоб заряди не виходили за межі панелі
            float scaleX = (panelWidth - 2 * maxRadius) / 2f;  // Відступ на радіус по обидва боки
            float scaleY = (panelHeight - 2 * maxRadius) / 2f; // Відступ на радіус по обидва боки
            float scale = Math.Min(scaleX, scaleY);

            // Перетворюємо світові координати в пікселі
            float screenX = (worldX * scale) + (panelWidth / 2.0f);   // Центрування по осі X
            float screenY = (worldY * scale) + (panelHeight / 2.0f);  // Центрування по осі Y

            return new PointF(screenX, screenY);
        }

        public void Draw(Graphics g, int panelWidth, int panelHeight)
        {
            // Перетворення координат заряду у пікселі
   

            // Задай колір залежно від заряду
            Color chargeColor = (this.Q > 0) ? Color.Blue : Color.Red;
            Brush brush = new SolidBrush(chargeColor);

            // Радіус і діаметр для малювання кола
            float radius = Math.Abs(Q) * 25;
            float diameter = 2 * radius;

            PointF screenPosition = WorldToScreen(X, Y, panelWidth, panelHeight, radius);

            // Малюємо заряд (коло) на відповідних координатах
            g.FillEllipse(brush, screenPosition.X - radius, screenPosition.Y - radius, diameter, diameter);

            // Малюємо значення заряду як текст
        

            string text = $"{Q.ToString()}";
            SizeF textSize = g.MeasureString(text, new Font("Arial", 14));

            float textX = screenPosition.X - (textSize.Width / 2);
            float textY = screenPosition.Y - (textSize.Height / 2);

            g.DrawString(text, new Font("Arial", 14), Brushes.White, textX, textY);
        }
  
     
  

    }
}
