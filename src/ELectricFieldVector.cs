using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPG_SP_2024
{
    public class ELectricFieldVector
    {

        private float _x;
        private float _y;
        private PointF _intensity;
        private const float arrowLength = 40; // Довжина вектора в пікселях


        public ELectricFieldVector(float x, float y, PointF intensity)
        {
            _x = x;
            _y = y;
            _intensity = intensity;
        }

        public void Draw(Graphics g)
        {
            // Нормалізація вектора інтенсивності, щоб він завжди мав однаковий розмір
            float magnitude = (float)Math.Sqrt(_intensity.X * _intensity.X + _intensity.Y * _intensity.Y);
            if (magnitude > 0)
            {
                float normalizedX = _intensity.X / magnitude;
                float normalizedY = _intensity.Y / magnitude;

                // Кінцева точка вектора з фіксованою довжиною
                float endX = _x + normalizedX * arrowLength;
                float endY = _y + normalizedY * arrowLength;

                // Малюємо лінію
                Pen pen = new Pen(Color.Green, 2);
                g.DrawLine(pen, _x, _y, endX, endY);

                // Малюємо стрілку на кінці вектора
                DrawArrowHead(g, pen, _x, _y, endX, endY);

                // Підписуємо величину поля
                string text = $"|E| = {magnitude:F2}";
                Font font = new Font("Arial", 12);
                Brush brush = Brushes.Black;
                g.DrawString(text, font, brush, endX + 5, endY + 5);
            }
        }

        private void DrawArrowHead(Graphics g, Pen pen, float startX, float startY, float endX, float endY)
        {
            float arrowSize = 10f;  // Розмір стрілки
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
