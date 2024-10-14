using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
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
        private float _screenX;
        private float _screenY;
        public float X
        {
            get { return _x; }
            set { _x = value; }
        }

        
        public float ScreenX { get { return _screenX; } }

        public float ScreenY { get { return _screenY; } }

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

            this._screenX = screenX;
            this._screenY = screenY;

            return new PointF(screenX, screenY);
        }

        public void Draw(Graphics g, int panelWidth, int panelHeight)
        {
            Color baseColor = (this.Q > 0) ? Color.Red : Color.Blue;
            Color shadowColor = Color.White;  
            Color highlightColor = ControlPaint.Light(baseColor);

     
            float maxRadius = Math.Min(panelWidth, panelHeight) * 0.05f; 
            float minRadius = 15f; 

          
            float radius = Math.Max(maxRadius, minRadius);  
            float diameter = 2 * radius;

            PointF screenPosition = WorldToScreen(X, Y, panelWidth, panelHeight, radius);
            var charge = new GraphicsPath();

            charge.AddEllipse(screenPosition.X - radius, screenPosition.Y - radius, diameter, diameter);

            var gradient = new PathGradientBrush(charge);
            // Встановлюємо центральну точку для відблиску (трохи зміщена вгору)
            gradient.CenterPoint = new PointF(screenPosition.X + radius /3 , screenPosition.Y - radius / 3);
            gradient.InterpolationColors = new ColorBlend()
            {
                Colors = new Color[]
                {
                    baseColor,
                    highlightColor,
                    Color.White,
                    shadowColor     

                },

                Positions = new float[] { 0.0f, 0.3f, 1f , 1.0f }
            };

      /*      var chargeReion = new Region(charge);*/          
            g.FillPath(gradient, charge);
            charge.CloseFigure();
      
            string text = $"{Q.ToString()}";
            SizeF textSize = g.MeasureString(text, new Font("Arial", 14));

            float textX = screenPosition.X - (textSize.Width / 2);
            float textY = screenPosition.Y - (textSize.Height / 2);

            g.DrawString(text, new Font("Arial", 14), Brushes.White, textX, textY);
        }
  
     
  

    }
}
