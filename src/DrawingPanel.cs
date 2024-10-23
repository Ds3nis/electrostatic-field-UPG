using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace UPG_SP_2024
{

    /// <summary>
    /// Třída <see cref="DrawingPanel"/> je panel určený pro vykreslování grafických prvků, jako jsou náboje a zonda a vektor intenzity elektrickeho pole.
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
        float arrowLength = 15f;
        float margin = 20f;

        /// <summary>
        /// Konstruktor třídy <see cref="DrawingPanel"/>, který inicializuje panel, časovač a výchozí scénář.
        /// </summary>
        public DrawingPanel()
        {

            if (this._scenario == null)
            {
                _scenario = new Scenario(0);
            }
           
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Dock = DockStyle.Fill;
            this.Size = new Size(800, 600);
            this.DoubleBuffered = true;

           
            
            probe = new Probe(1);
        
            lastFrameTime = DateTime.Now;

            timer = new System.Windows.Forms.Timer();
            timer.Interval = 16; 
            timer.Tick += Timer_Tick;
            timer.Start();

        }

        /// <summary>
        /// Překresluje obsah panelu, včetně nábojů a vektorů elektrického pole.
        /// </summary>
        /// <param name="e">Parametr obsahující data o události vykreslování.</param>
        protected override void OnPaint(PaintEventArgs e)
        {

            Graphics g = e.Graphics;
            List<Charge> charges = _scenario.charges;

            float squareSize = Math.Min(this.Width, this.Height) - 2 * margin;

           
            float topLeftX = (this.Width - squareSize) / 2f;
            float topLeftY = (this.Height - squareSize) / 2f;
/*            g.DrawRectangle(Pens.Black, topLeftX, topLeftY, squareSize, squareSize);*/

            scale = (squareSize / 2f);

            // Vykreslení nábojů na panel.
            this.DrawCharges(g, charges, this.Width, this.Height, topLeftX, topLeftY, squareSize);

            probe.Draw(e.Graphics, this.Width, this.Height, scale);
            // Výpočet intenzity elektrického pole v místě sondy.
            PointF intensityPoint = ElectricField.CalculateField(charges, probe.X, probe.Y);

            // Vytvoření a vykreslení vektoru elektrického pole
            ELectricFieldVector vector = new ELectricFieldVector(probe.ScreenX, probe.ScreenY, intensityPoint, arrowLength, scale);

            vector.Draw(g);




        }

        /// <summary>
        /// Událost časovače, která se spustí každých 16ms a aktualizuje polohu sondy a překresluje panel.
        /// </summary>
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Získání aktuálního času a výpočet deltaTime (čas mezi snímky).
            DateTime currentTime = DateTime.Now;
            float deltaTime = (float)(currentTime - lastFrameTime).TotalSeconds; 
            lastFrameTime = currentTime;

            // Aktualizace polohy sondy na základě deltaTime.
            probe.UpdatePosition(deltaTime);

            // Zneplatnění panelu a jeho překreslení.
            this.Invalidate();
        }


        /// <summary>
        /// Vykreslí všechny náboje v panelu.
        /// </summary>
        /// <param name="g">Grafická plocha, na které se budou náboje vykreslovat.</param>
        /// <param name="charges">Seznam nábojů, které se mají vykreslit.</param>
        /// <param name="panelWidth">Šířka panelu.</param>
        /// <param name="panelHeight">Výška panelu.</param>
        /// <param name="topLeftX">X-ová souřadnice levého horního rohu čtverce.</param>
        /// <param name="topLeftY">Y-ová souřadnice levého horního rohu čtverce.</param>
        /// <param name="squareSize">Velikost čtverce, kde se vykreslují náboje.</param>
        private void DrawCharges(Graphics g, List<Charge>  charges, float panelWidth, float panelHeight, float topLeftX, float topLeftY, float squareSize)
        {
            foreach (Charge charge in charges)
            {
                charge.Draw(g, panelWidth, panelHeight, topLeftX, topLeftY, squareSize);
            }
        }






        /// <summary>
        /// Událost pro změnu velikosti panelu, která zneplatní panel a vynutí jeho překreslení.
        /// </summary>
        /// <param name="eventargs">Parametr obsahující data o události změny velikosti.</param>
        protected override void OnResize(EventArgs eventargs)
        {
            this.Invalidate();  

            base.OnResize(eventargs);
        }
    }
}
