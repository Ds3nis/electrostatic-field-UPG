using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.AxHost;

namespace UPG_SP_2024
{
    /// <summary>
    /// Třída představující vektor elektrického pole.
    /// </summary>
    public class ELectricFieldVector
    {

        private float _x; // X-ová souřadnice počátečního bodu.
        private float _y; // Y-ová souřadnice počátečního bodu.
        private PointF _intensity; // Intenzita elektrického pole jako bod.
        private float _arrowLength; // Délka šipky znázorňující vektor.
        private float scale; // Měřítko pro vykreslování.


        /// <summary>
        /// Inicializuje nový instanci třídy ElectricFieldVector.
        /// </summary>
        /// <param name="x">X-ová souřadnice počátečního bodu vektoru.</param>
        /// <param name="y">Y-ová souřadnice počátečního bodu vektoru.</param>
        /// <param name="intensity">Intenzita elektrického pole jako bod typu <see cref="PointF"/>.</param>
        /// <param name="arrowLength">Maximální délka šipky vektoru.</param>
        /// <param name="scale">Měřítko pro vykreslování.</param>
        public ELectricFieldVector(float x, float y, PointF intensity, float arrowLength, float scale)
        {
            _x = x;
            _y = y;
            _intensity = intensity;
            float maxArrowLength = Math.Max(arrowLength, scale * 0.09f);
            _arrowLength = maxArrowLength;
            this.scale = scale;
        }


        /// <summary>
        /// Vykreslí vektor elektrického pole na grafický objekt.
        /// </summary>
        /// <param name="g">Grafický objekt, na který se vektor vykreslí.</param>
        public void Draw(Graphics g, Color color)
        {
            
            float magnitude = (float)Math.Sqrt(_intensity.X * _intensity.X + _intensity.Y * _intensity.Y);
            if (magnitude > 0)
            {
                float normalizedX = _intensity.X / magnitude;
                float normalizedY = _intensity.Y / magnitude;


             

                float endX = _x + normalizedX * _arrowLength;
                float endY = _y + normalizedY * _arrowLength;

                Pen pen = new Pen(color, 2);
                g.DrawLine(pen, _x, _y, endX, endY);

            
                DrawArrowHead(g, pen, _x, _y, endX, endY);


                this.DrawTitleMagnitude(g, magnitude, endX, endY);


            }
        }

        public void DrawStatic(Graphics g, Color color)
        {
            float magnitude = (float)Math.Sqrt(_intensity.X * _intensity.X + _intensity.Y * _intensity.Y);
            if (magnitude > 0)
            {
                float normalizedX = _intensity.X / magnitude;
                float normalizedY = _intensity.Y / magnitude;

                float endX = _x + normalizedX * (_arrowLength / 1.5f);
                float endY = _y + normalizedY * (_arrowLength / 1.5f);

                Pen pen = new Pen(color, 2f); // Тонший пензель
                g.DrawLine(pen, _x, _y, endX, endY);
                DrawArrowHead(g, pen, _x, _y, endX, endY, 8f);

                pen.Dispose(); 
            }
        }


        /// <summary>
        /// Vykreslí text představující velikost elektrického pole.
        /// </summary>
        /// <param name="g">Grafický objekt, na který se text vykreslí.</param>
        /// <param name="magnitude">Velikost elektrického pole.</param>
        /// <param name="endX">X-ová souřadnice konce šipky.</param>
        /// <param name="endY">Y-ová souřadnice konce šipky.</param>
        private void DrawTitleMagnitude(Graphics g, float magnitude, float endX, float endY)
        {
            float deltaX = endX - _x;
            float deltaY = endY - _y;

            float magnitudeScaled = magnitude * 1e-9f;
            string text = $"|E| = {magnitudeScaled:F2} * 10⁹";

            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
    

            float minFontSize = 7f; 
            float maxFontSize = 10f;
            float fontSize = Math.Max(minFontSize, Math.Min(maxFontSize, scale * 0.05f)); 

            using (Font font = new Font("Arial", fontSize, FontStyle.Bold))
            {
                SizeF textSize = g.MeasureString(text, font);
                Brush brush = Brushes.Black;

                float textX = _x; 
                float textY = _y; 

                float offsetX = 5f; 
                float offsetY = -textSize.Height - 5f; 

                textX += offsetX; 
                textY += offsetY; 

                
                if (textX < 0)
                {
                    textX = _x - textSize.Width - offsetX; 
                }
                else if (textX + textSize.Width > g.VisibleClipBounds.Width)
                {
                    textX = _x - textSize.Width - offsetX; 
                }

                if (textY < 0)
                {
                    textY = _y + 5f; 
                }
                else if (textY + textSize.Height > g.VisibleClipBounds.Height)
                {
                    textY = _y - textSize.Height - 5f;
                }

           
                g.DrawString(text, font, brush, textX, textY);
            }

            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
        }


        /// <summary>
        /// Vykreslí špičku šipky na konci vektoru.
        /// </summary>
        /// <param name="g">Grafický objekt, na který se špička šipky vykreslí.</param>
        /// <param name="pen">Pero pro vykreslení špičky šipky.</param>
        /// <param name="startX">X-ová souřadnice počátečního bodu.</param>
        /// <param name="startY">Y-ová souřadnice počátečního bodu.</param>
        /// <param name="endX">X-ová souřadnice konce šipky.</param>
        /// <param name="endY">Y-ová souřadnice konce šipky.</param>
        private void DrawArrowHead(Graphics g, Pen pen, float startX, float startY, float endX, float endY, float arrowSize = 10f)
        { 
            double angle = Math.Atan2(endY - startY, endX - startX);

            PointF arrowPoint1 = new PointF(
                endX - arrowSize * (float)Math.Cos(angle - Math.PI / 6),
                endY - arrowSize * (float)Math.Sin(angle - Math.PI / 6)
            );

            PointF arrowPoint2 = new PointF(
                endX - arrowSize * (float)Math.Cos(angle + Math.PI / 6),
                endY - arrowSize * (float)Math.Sin(angle + Math.PI / 6)
            );

            g.DrawLine(pen, endX, endY, arrowPoint1.X, arrowPoint1.Y);
            g.DrawLine(pen, endX, endY, arrowPoint2.X, arrowPoint2.Y);
        }
    }
    }
