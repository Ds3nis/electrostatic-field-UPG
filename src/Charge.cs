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

        public void Draw(Graphics g)
        {
            string chargeSymbol;
            Color chargeColor;
            if(this.Q > 0)
            {
   
                chargeColor = Color.Blue;
            }
            else
            {
    
                chargeColor = Color.Red;
            }

            Brush brush = new SolidBrush(chargeColor);

          
            float radius = Math.Abs(Q) * 10;  
            float diameter = 2 * radius;

        
            g.FillEllipse(brush, X - radius, Y - radius, diameter, diameter);

            string text = $"{Q.ToString()}";

            SizeF textSize = g.MeasureString(text, new Font("Arial", 14));

            float textX = X - (textSize.Width / 2);
            float textY = Y - (textSize.Height / 2);

            g.DrawString(text, new Font("Arial", 14), Brushes.White, textX, textY);
        }

    }
}
