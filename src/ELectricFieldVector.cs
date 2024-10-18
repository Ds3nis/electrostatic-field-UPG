using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.AxHost;

namespace UPG_SP_2024
{
    public class ELectricFieldVector
    {

        private float _x;
        private float _y;
        private PointF _intensity;
        private float _arrowLength; 


        public ELectricFieldVector(float x, float y, PointF intensity, float arrowLength)
        {
            _x = x;
            _y = y;
            _intensity = intensity;
            _arrowLength = arrowLength;
        }

        public void Draw(Graphics g)
        {
            
            float magnitude = (float)Math.Sqrt(_intensity.X * _intensity.X + _intensity.Y * _intensity.Y);
            if (magnitude > 0)
            {
                float normalizedX = _intensity.X / magnitude;
                float normalizedY = _intensity.Y / magnitude;


             

                float endX = _x + normalizedX * _arrowLength;
                float endY = _y + normalizedY * _arrowLength;

                Pen pen = new Pen(Color.Green, 2);
                g.DrawLine(pen, _x, _y, endX, endY);

            
                DrawArrowHead(g, pen, _x, _y, endX, endY);


                this.DrawTitleMagnitude(g, magnitude, endX, endY);

             
            }
        }

        private void DrawTitleMagnitude(Graphics g, float magnitude, float endX, float endY)
        {

            float midX = (_x + endX) / 2;
            float midY = (_y + endY) / 2;
            float magnitudeScaled = magnitude * 1e-9f;
            string text = $"|E| = {magnitudeScaled:F2} * 10⁹";      
            Font font = new Font("Arial", 10);
            SizeF textSize = g.MeasureString(text, font);
            Brush brush = Brushes.Black;
            g.DrawString(text, font, brush, midX - (textSize.Width / 2), midY - (textSize.Height/ 2));
        }

        private void DrawArrowHead(Graphics g, Pen pen, float startX, float startY, float endX, float endY)
        {
            float arrowSize = 10f;  
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
