using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace UPG_SP_2024
{

    /// <summary>
    /// The main panel with the custom visualization
    /// </summary>
    public class DrawingPanel : Panel
    {
        /// <summary>Initializes a new instance of the <see cref="DrawingPanel" /> class.</summary>
        /// 
        public Scenario _scenario;
        public float scale;
        private System.Windows.Forms.Timer timer;
        private Probe probe;
        private DateTime lastFrameTime;
      
        public DrawingPanel()
        {
            
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Dock = DockStyle.Fill;
            this.DoubleBuffered = true;

           
      
            probe = new Probe(1); 
            lastFrameTime = DateTime.Now;

            timer = new System.Windows.Forms.Timer();
            timer.Interval = 16; 
            timer.Tick += Timer_Tick;
            timer.Start();

           

        }


        /// <summary>TODO: Custom visualization code comes into this method</summary>
        /// <remarks>Raises the <see cref="E:System.Windows.Forms.Control.Paint">Paint</see> event.</remarks>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs">PaintEventArgs</see> that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {

            float arrowLength = 20f;
            Graphics g = e.Graphics;
            List<Charge> charges = _scenario.charges;

            float margin = 20f; 
            float squareSize = Math.Min(this.Width, this.Height) - 2 * margin;

           
            float topLeftX = (this.Width - squareSize) / 2f;
            float topLeftY = (this.Height - squareSize) / 2f;
            g.DrawRectangle(Pens.Black, topLeftX, topLeftY, squareSize, squareSize);

            scale = (squareSize / 2f);  


            this.DrawCharges(g, charges, this.Width, this.Height, topLeftX, topLeftY, squareSize);

            probe.Draw(e.Graphics, this.Width, this.Height, scale);
            PointF intensityPoint = ElectricField.CalculateField(charges, probe.X, probe.Y);

            
            ELectricFieldVector vector = new ELectricFieldVector(probe.ScreenX, probe.ScreenY, intensityPoint, Math.Max(Math.Min(this.Width, this.Height) * 0.05f, arrowLength));

            vector.Draw(g);




        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            DateTime currentTime = DateTime.Now;
            float deltaTime = (float)(currentTime - lastFrameTime).TotalSeconds;
            lastFrameTime = currentTime;

      
            probe.UpdatePosition(deltaTime);


            this.Invalidate();
        }



        private void DrawCharges(Graphics g, List<Charge>  charges, float panelWidth, float panelHeight, float topLeftX, float topLeftY, float squareSize)
        {
            foreach (Charge charge in charges)
            {
                charge.Draw(g, panelWidth, panelHeight, topLeftX, topLeftY, squareSize);
            }
        }

    




        /// <summary>
        /// Fires the event indicating that the panel has been resized. Inheriting controls should use this in favor of actually listening to the event, but should still call <span class="keyword">base.onResize</span> to ensure that the event is fired for external listeners.
        /// </summary>
        /// <param name="eventargs">An <see cref="T:System.EventArgs">EventArgs</see> that contains the event data.</param>
        protected override void OnResize(EventArgs eventargs)
        {
            this.Invalidate();  

            base.OnResize(eventargs);
        }
    }
}
