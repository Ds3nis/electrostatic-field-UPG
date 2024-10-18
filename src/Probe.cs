using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPG_SP_2024
{
    public class Probe
    {
          public float X { get; set; }
    public float Y { get; set; }
    private float radius;
    private float angularVelocity;
    private float angle;
    private float _screenX;
    private float _screenY;
    public float ScreenX { get { return _screenX; } }

    public float ScreenY { get { return _screenY; } }

    public Probe(float radius)
    {
        this.radius = radius;
        this.angularVelocity = (float)(Math.PI / 6); 
        this.angle = 0;
    }

    public void UpdatePosition(float deltaTime)
    {
     
        angle += angularVelocity * deltaTime;

      
        X = (radius) * (float)Math.Cos(angle);
        Y = (radius) * (float)Math.Sin(angle);
    }


        public void Draw(Graphics g, float panelWidth, float panelHeight, float scale)
    {

            float radius1 = Math.Min(panelWidth, panelHeight) * 0.05f;

            float scale1 = scale - radius1; ;

            float screenX = ((X * scale1)) + panelWidth / 2f;
            float screenY = (Y * (scale1)) + panelHeight / 2f;

            this._screenX = screenX;
            this._screenY = screenY;
            
            float probeSize = 10;
            g.FillEllipse(Brushes.Green, screenX - probeSize / 2, screenY - probeSize / 2, probeSize, probeSize);

            float r = radius * scale1;
            g.DrawEllipse(Pens.Red, panelWidth / 2f - r, panelHeight / 2f - r, 2 * r, 2 * r);
    }

       
    }
}
