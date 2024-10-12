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

            public float X { get; set; }  // Координата X точки, де малюється вектор
            public float Y { get; set; }  // Координата Y точки
            public float IntensityX { get; set; }  // Компонента вектора інтенсивності по осі X
            public float IntensityY { get; set; }  // Компонента вектора інтенсивності по осі Y


            

            /// <summary>
            /// Конструктор, що ініціалізує координати і компоненти вектора
            /// </summary>
            public ELectricFieldVector(float x, float y, float intensityX, float intensityY)
            {
                this.X = x;
                this.Y = y;
                this.IntensityX = intensityX;
                this.IntensityY = intensityY;
            }

            /// <summary>
            /// Метод для малювання вектора електричного поля
            /// </summary>
            public void Draw(Graphics g)
            {
                // Визначаємо кінцеву точку вектора
                float endX = X + IntensityX;  // Масштабуємо вектор для наочності
                float endY = Y + IntensityY;

                // Малюємо стрілку, яка представляє вектор інтенсивності
                Pen pen = new Pen(Color.Red, 2);
                AdjustableArrowCap arrowCap = new AdjustableArrowCap(5, 5);  // Стрілка на кінці
                pen.CustomEndCap = arrowCap;

                g.DrawLine(pen, X, Y, endX, endY);  // Лінія від початкової до кінцевої точки
            }
        }
    }
