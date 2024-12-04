using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        private float radius;
        private float scale;
        private float scenario4Q = 0;

        public float Scenario4Q
        {
            get {  return scenario4Q; }
            set { scenario4Q = value; }
        }

        public float X
        {
            get { return _x; }
            set { _x = value; }
        }

        public float Radius { get { return this.radius; } }

        
        public float ScreenX { get { return _screenX; }  }

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


        public float PanelScale { get; set; } = 1f;




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
            UpdateRadius();
        }

       

        /// <summary>
        /// Převádí souřadnice světa na souřadnice obrazovky.
        /// </summary>
        /// <param name="topLeftX">X-ová souřadnice levého horního rohu světa.</param>
        /// <param name="topLeftY">Y-ová souřadnice levého horního rohu světa.</param>
        /// <param name="squareSize">Velikost čtverce reprezentujícího svět.</param>
        /// <param name="maxRadius">Maximální poloměr zobrazení náboje.</param>
        /// <returns>Vrací bod typu <see cref="PointF"/> s převedenými souřadnicemi.</returns>
        public PointF WorldToScreen(float topLeftX, float topLeftY, float squareSize, float maxRadius)
        {

            float scale = (squareSize / 2f) - maxRadius;



            float screenX = (this.X * scale) + (topLeftX + squareSize / 2f);
            float screenY = (this.Y * scale) + (topLeftY + squareSize / 2f);

        

            return new PointF(screenX, screenY);
        }

        public void UpdateRadius()
        {
            const float baseRadius = 6f; //Мінімальний розмір
            const float maxRadius = 100f;  // Максимальний розмір
            const float scalingFactor = 20f; // Фактор чутливості

            // Попереднє обчислення радіуса залежно від Q
            float calculatedRadius = Math.Abs(Q) * scalingFactor * PanelScale;

            // Обмеження радіуса у межах [baseRadius, maxRadius]
            this.radius = Math.Clamp(calculatedRadius, baseRadius, maxRadius);
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
        public void Draw(Graphics g, float panelWidth, float panelHeight, float topLeftX, float topLeftY, float squareSize, float scale)
        {
            this.scale = scale;
            float panelMinSize = Math.Min(panelWidth, panelHeight);
            float scaleFactor = panelMinSize / 500f; // Базовий розмір для масштабу 
            this.PanelScale = scaleFactor; // Оновлюємо масштаб для кожного заряду
 
            UpdateRadius();
            Color baseColor = (this.Q > 0) ? Color.Red : Color.Blue;
            Color shadowColor = Color.White;  
            Color highlightColor = ControlPaint.Light(baseColor);


            //float maxRadius = Math.Min(panelWidth, panelHeight) * 0.05f; 
            //float minRadius = 15f;


            //float radius = Math.Max(maxRadius, minRadius);
            //float maxRadius = squareSize * 0.09f; // Максимальний розмір заряду (5% від розміру панелі)
            //float minRadius = 5f;                // Мінімальний розмір заряду
            //float radius = Math.Clamp(Math.Abs(this.Q) * scale, minRadius, maxRadius);
            ////this.radius = radius;
            //this.radius = Math.Max(Math.Abs(this.Q), scale * 0.1f);
            float diameter = 2 * this.radius;

            PointF screenPosition = WorldToScreen(topLeftX, topLeftY, squareSize, this.radius);
            this._screenX = screenPosition.X;
            this._screenY = screenPosition.Y;
            var charge = new GraphicsPath();

            charge.AddEllipse(screenPosition.X - radius, screenPosition.Y - radius, diameter, diameter);
            Pen eliipseBorder = new Pen(Color.Black, 3f);
            g.DrawEllipse(eliipseBorder ,screenPosition.X - this.radius, screenPosition.Y - this.radius, diameter, diameter);

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

            Region chargeRegion = new Region(charge);
          
            g.FillPath(gradient, charge);
            charge.CloseFigure();
      
            string text = $"{Math.Round(Q, 2).ToString()}";
            SizeF textSize = g.MeasureString(text, new Font("Arial", 14));

            float textX = screenPosition.X - (textSize.Width / 2);
            float textY = screenPosition.Y - (textSize.Height / 2);

            g.DrawString(text, new Font("Arial", 14), Brushes.White, textX, textY);
           
           

        }
  
     
  

    }
}
