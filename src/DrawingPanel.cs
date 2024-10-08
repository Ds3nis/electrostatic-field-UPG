using System;
using System.Drawing;
using System.Windows.Forms;

namespace UPG_SP_2024
{

    /// <summary>
    /// The main panel with the custom visualization
    /// </summary>
    public class DrawingPanel : Panel
    {
        /// <summary>Initializes a new instance of the <see cref="DrawingPanel" /> class.</summary>
        public DrawingPanel()
        {
            
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Dock = DockStyle.Fill;

        }


        /// <summary>TODO: Custom visualization code comes into this method</summary>
        /// <remarks>Raises the <see cref="E:System.Windows.Forms.Control.Paint">Paint</see> event.</remarks>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs">PaintEventArgs</see> that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            List<Charge> charges = new List<Charge> 
            {
                new Charge(this.Width/2,this.Height/2,1)
            };

            foreach(Charge charge in charges)
            {
                charge.Draw(g);
            }

            var field = ElectricField.CalculateField(charges, (this.Width / 2) + 20, this.Height / 2);

            ELectricFieldVector vector = new ELectricFieldVector(200, 300, field.X, field.Y);
            vector.Draw(g);
        }

        /// <summary>
        /// Fires the event indicating that the panel has been resized. Inheriting controls should use this in favor of actually listening to the event, but should still call <span class="keyword">base.onResize</span> to ensure that the event is fired for external listeners.
        /// </summary>
        /// <param name="eventargs">An <see cref="T:System.EventArgs">EventArgs</see> that contains the event data.</param>
        protected override void OnResize(EventArgs eventargs)
        {
            this.Invalidate();  //ensure repaint

            base.OnResize(eventargs);
        }
    }
}
