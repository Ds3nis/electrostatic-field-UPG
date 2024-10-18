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


        private PointF WorldToScreen(float topLeftX, float topLeftY, float squareSize, float maxRadius)
        {


            /*     float scaleX = (panelWidth) / 2f;
                 float scaleY = (panelHeight) / 2f;
                 float scale = Math.Min(scaleX, scaleY);*/


            /*      float screenX = (worldX * scale) + (panelWidth / 2.0f); 
                  float screenY = (worldY * scale) + (panelHeight / 2.0f);*/
            /*if (screenX - maxRadius < 0)
            {
                screenX = maxRadius; // Зменшуємо координату X до максимального радіусу
            }
            else if (screenX + maxRadius > panelWidth)
            {
                screenX = panelWidth - maxRadius; // Зменшуємо координату X до краю панелі
            }

            if (screenY - maxRadius < 0)
            {
                screenY = maxRadius; // Зменшуємо координату Y до максимального радіусу
            }
            else if (screenY + maxRadius > panelHeight)
            {
                screenY = panelHeight - maxRadius; // Зменшуємо координату Y до краю панелі
            }
*/
            float scale = (squareSize / 2f) - maxRadius;

            float screenX = (this.X * scale) + (topLeftX + squareSize / 2f);
            float screenY = (this.Y * scale) + (topLeftY + squareSize / 2f);

            this._screenX = screenX;
            this._screenY = screenY;

            return new PointF(screenX, screenY);
        }

        public void Draw(Graphics g, float panelWidth, float panelHeight, float topLeftX, float topLeftY, float squareSize)
        {
            Color baseColor = (this.Q > 0) ? Color.Red : Color.Blue;
            Color shadowColor = Color.White;  
            Color highlightColor = ControlPaint.Light(baseColor);

     
            float maxRadius = Math.Min(panelWidth, panelHeight) * 0.05f; 
            float minRadius = 15f; 

          
            float radius = Math.Max(maxRadius, minRadius);  
            float diameter = 2 * radius;

            PointF screenPosition = WorldToScreen(topLeftX, topLeftY, squareSize, maxRadius);
            var charge = new GraphicsPath();

            charge.AddEllipse(screenPosition.X - radius, screenPosition.Y - radius, diameter, diameter);
            Pen eliipseBorder = new Pen(Color.Black, 3f);
            g.DrawEllipse(eliipseBorder ,screenPosition.X - radius, screenPosition.Y - radius, diameter, diameter);

            var gradient = new PathGradientBrush(charge);
 
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
