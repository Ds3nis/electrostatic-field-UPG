using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UPG_SP_2024
{


    /// <summary>
    /// Třída reprezentující elektrický náboj.
    /// </summary>
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


        /// <summary>
        /// Konstruktor pro inicializaci náboje s jeho souřadnicemi a hodnotou náboje.
        /// </summary>
        /// <param name="x">X-ová realna souřadnice náboje.</param>
        /// <param name="y">Y-ová realna souřadnice náboje.</param>
        /// <param name="Q">Hodnota náboje Q.</param>
        public Charge(float x, float y, float Q)
        {
            this.X = x;
            this.Y = y;
            this.Q = Q;
        }

        /// <summary>
        /// Převádí souřadnice světa na souřadnice obrazovky.
        /// </summary>
        /// <param name="topLeftX">X-ová souřadnice levého horního rohu světa.</param>
        /// <param name="topLeftY">Y-ová souřadnice levého horního rohu světa.</param>
        /// <param name="squareSize">Velikost čtverce reprezentujícího svět.</param>
        /// <param name="maxRadius">Maximální poloměr zobrazení náboje.</param>
        /// <returns>Vrací bod typu <see cref="PointF"/> s převedenými souřadnicemi.</returns>
        private PointF WorldToScreen(float topLeftX, float topLeftY, float squareSize, float maxRadius)
        {

            float scale = (squareSize / 2f) - maxRadius;

            float screenX = (this.X * scale) + (topLeftX + squareSize / 2f);
            float screenY = (this.Y * scale) + (topLeftY + squareSize / 2f);

            this._screenX = screenX;
            this._screenY = screenY;

            return new PointF(screenX, screenY);
        }




        /// <summary>
        /// Kreslí náboj na dané grafické ploše.
        /// </summary>
        /// <param name="g">Grafická plocha, kde bude náboj vykreslen.</param>
        /// <param name="panelWidth">Šířka panelu.</param>
        /// <param name="panelHeight">Výška panelu.</param>
        /// <param name="topLeftX">X-ová souřadnice levého horního rohu sveta.</param>
        /// <param name="topLeftY">Y-ová souřadnice levého horního rohu sveta.</param>
        /// <param name="squareSize">Velikost čtverce reprezentujícího svět.</param>
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
