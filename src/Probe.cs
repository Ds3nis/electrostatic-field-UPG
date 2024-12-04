using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UPG_SP_2024
{
    /// <summary>
    /// Třída reprezentující sondu, která se pohybuje po kruhové dráze.
    /// </summary>
    public class Probe
    {
        /// <summary>
        /// X souřadnice sondy v reálném prostoru.
        /// </summary>
        public float X { get; set; }
        /// <summary>
        /// Y souřadnice sondy v reálném prostoru.
        /// </summary>
        public float Y { get; set; }
        private float radius;          // Poloměr kruhu, po kterém se sonda pohybuje.
        private float angularVelocity; // Úhlová rychlost pohybu sondy.
        private float angle;           // Aktuální úhel sondy na kruhu.
        private float _screenX;       // X souřadnice na obrazovce.
        private float _screenY;       // Y souřadnice na obrazovce.
        private float worldX;
        private float worldY;
        public DateTime CreationTime { get; set; }
        public PointF intensityPoint;
        public float ScreenX { get { return _screenX; } }
        public Color ProbeColor { get; set; }
        public float ScreenY { get { return _screenY; } }

        /// <summary>
        /// Inicializuje novou instanci třídy Probe s daným poloměrem.
        /// </summary>
        /// <param name="radius">Poloměr (float) kruhu, po kterém se sonda pohybuje.</param>
        public Probe(float radius, float initialX, float initialY, Color? color = null)
        {
            this.radius = radius;
            this.worldX = initialX;
            this.worldY = initialY;
            this.CreationTime = DateTime.Now;
            this.angularVelocity = (float)(Math.PI / 6); 
            this.angle = 0;
           
            if (color.HasValue)
            {
                this.ProbeColor = color.Value;
            }
            else
            {
                Random randomColor = new Random();
                this.ProbeColor = Color.FromArgb(randomColor.Next(256), randomColor.Next(256), randomColor.Next(256));
            }
           
        }

        public float currentFieldMagnitude()
        {
            return (float)Math.Sqrt(intensityPoint.X * intensityPoint.X + intensityPoint.Y * intensityPoint.Y);
        }


        public void calculateIntensity(List<Charge> charges, float scale)
        {
            PointF intensityPoint = ElectricField.CalculateField(charges, this.X, this.Y, scale);
            this.intensityPoint = intensityPoint;
        }

        /// <summary>
        /// Aktualizuje pozici sondy na základě času, který uplynul od poslední aktualizace.
        /// </summary>
        /// <param name="deltaTime">Časový interval (float), který uplynul od poslední aktualizace.</param>
        public void UpdatePosition(float deltaTime)
        {
            // Оновлюємо кут руху
            angle += angularVelocity * deltaTime;

            // Обчислюємо позицію на колі (відносно центру X, Y)
            this.X = this.worldX + (radius * (float)Math.Cos(angle));
            this.Y = this.worldY + (radius * (float)Math.Sin(angle));

            // Фактична позиція обчислюється лише для малювання
        
        }
        /// <summary>
        /// Vykresluje sondu na daném grafickém kontextu.
        /// </summary>
        /// <param name="g">Grafický kontext (Graphics), na který se má sonda vykreslit.</param>
        /// <param name="panelWidth">Šířka panelu (float), na kterém se sonda vykresluje.</param>
        /// <param name="panelHeight">Výška panelu (float), na kterém se sonda vykresluje.</param>
        /// <param name="scale">Měřítko (float), které se používá pro úpravu velikosti pozice sondy.</param>
        public void Draw(Graphics g, float panelWidth, float panelHeight, float scale)
    {

            float radius1 = Math.Min(panelWidth, panelHeight) * 0.05f;

            float scale1 = scale - radius1;

            float screenX = (X * scale1) + panelWidth / 2f;
            float screenY = (Y * scale1) + panelHeight / 2f;


            this._screenX = screenX;
            this._screenY = screenY;
            
            float probeSize = 10;
            using (Brush brush = new SolidBrush(this.ProbeColor))
            {
                g.FillEllipse(brush, screenX - probeSize / 2, screenY - probeSize / 2, probeSize, probeSize);
            }
            //g.FillEllipse(Brushes.Green, screenX - probeSize / 2, screenY - probeSize / 2, probeSize, probeSize);
    }

       
    }
}
