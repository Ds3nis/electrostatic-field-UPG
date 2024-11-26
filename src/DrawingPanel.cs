using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Drawing.Imaging;
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
        List<Charge> charges;
        float deltaTime;
        float margin = 20f;
        public int _gridSpacingX; // Výchozí rozteč mřížky v ose X
        public int _gridSpacingY; // Výchozí rozteč mřížky v ose Y

        private Charge selectedCharge = null;
        private Point lastMousePosition;
        private ToolTip chargeToolTip;
        private int blockSize = 10;

        private ColourMap[] Lut { get; set; }


        private Bitmap colorMapBitmap;

        /// <summary>
        /// Konstruktor třídy <see cref="DrawingPanel"/>, který inicializuje panel, časovač a výchozí scénář.
        /// </summary>
        public DrawingPanel(Scenario scenario, int gridSpacingX, int gridSpacingY)
        {
            this._gridSpacingX = gridSpacingX;
            this._gridSpacingY = gridSpacingY;
            this._scenario = scenario;

            if (this._gridSpacingX == 0 && this._gridSpacingY == 0)
            {
                this._gridSpacingX = 50;
                this._gridSpacingY = 50;
            }


            if (this._scenario == null)
            {
                _scenario = new Scenario(0);
            }

            this.Lut = MakeLut();

            chargeToolTip = new ToolTip();
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Dock = DockStyle.Fill;
            this.Size = new Size(800, 600);
            this.DoubleBuffered = true;

            this.charges = this._scenario.charges;
            
            probe = new Probe(1);
        
            lastFrameTime = DateTime.Now;

            timer = new System.Windows.Forms.Timer();
            this.MouseDown += DrawingPanel_MouseDown;
            this.MouseMove += DrawingPanel_MouseMove;
            this.MouseUp += DrawingPanel_MouseUp;
            timer.Interval = 10; 
            timer.Tick += Timer_Tick;
            timer.Start();

        }

        private void DrawingPanel_MouseUp(object? sender, MouseEventArgs e)
        {
            selectedCharge = null;
        }

        private void DrawingPanel_MouseMove(object? sender, MouseEventArgs e)
        {
            if (selectedCharge != null)
            {
                
                float dx = (e.X - lastMousePosition.X) / (scale - selectedCharge.Radius);
                float dy = (e.Y - lastMousePosition.Y) / (scale - selectedCharge.Radius);

                //chargeToolTip.Show($"Position of mouse: ({dx}, {dy})\nCharge: ({selectedCharge.X}, {selectedCharge.Y})",
                //                       this, e.X + 10, e.Y + 10);

                selectedCharge.X += dx;
                selectedCharge.Y += dy;

                lastMousePosition = e.Location;

             
            }
        }

        private void DrawingPanel_MouseDown(object? sender, MouseEventArgs e)
        {
            foreach (Charge charge in charges)
            {
                float dx = e.X - charge.ScreenX;
                float dy = e.Y - charge.ScreenY;
                float distance = (float)Math.Sqrt(dx * dx + dy * dy);

                if (distance <= charge.Radius)
                {
                
                    chargeToolTip.Show($"Position of mouse: ({e.X}, {e.Y})\nCharge: ({charge.ScreenX}, {charge.ScreenY})",
                                    this, e.X + 10, e.Y + 10);
                    selectedCharge = charge;
                    lastMousePosition = e.Location;
                    break;
                }
            }
           
        }





        /// <summary>
        /// Překresluje obsah panelu, včetně nábojů a vektorů elektrického pole.
        /// </summary>
        /// <param name="e">Parametr obsahující data o události vykreslování.</param>
        protected override void OnPaint(PaintEventArgs e)
        {

            Graphics g = e.Graphics;


            float squareSize = Math.Min(this.Width, this.Height) - 2 * margin;


            float topLeftX = (this.Width - squareSize) / 2f;
            float topLeftY = (this.Height - squareSize) / 2f;
            g.DrawRectangle(Pens.Black, topLeftX, topLeftY, squareSize, squareSize);

            scale = (squareSize / 2f);

            this.CalculateFieldIntensitiesOnGrid();

            if (colorMapBitmap != null)
            {
                g.DrawImage(colorMapBitmap, 0, 0, this.Width, this.Height);
            }
        
            this.DrawFieldGrid(g, charges);
            this.DrawCharges(g, charges, this.Width, this.Height, topLeftX, topLeftY, squareSize);
         
            probe.Draw(e.Graphics, this.Width, this.Height, scale);
            // Výpočet intenzity elektrického pole v místě sondy.
            PointF intensityPoint = ElectricField.CalculateField(charges, probe.X, probe.Y);

            // Vytvoření a vykreslení vektoru elektrického pole
            ELectricFieldVector vector = new ELectricFieldVector(probe.ScreenX, probe.ScreenY, intensityPoint, arrowLength, scale);

            vector.Draw(g, Color.Green);

       


        }



        private ColourMap[] MakeLut()
        {

            List<ColourMap> colorMap = new List<ColourMap>
            {
                new ColourMap(0.1f, Color.Blue),
                new ColourMap(0.15f, Color.Cyan),
                new ColourMap(0.2f, Color.LimeGreen),
                new ColourMap(0.3f, Color.Yellow),
                new ColourMap(0.5f, Color.Red)
            };

            return colorMap.ToArray();
        }

        public Color GetColor(float normalizedIntensity)
        {
            for (int i = 0; i < Lut.Length - 1; i++)
            {
                var lower = Lut[i];
                var upper = Lut[i + 1];

                if (normalizedIntensity >= lower.Threshold && normalizedIntensity <= upper.Threshold)
                {
                    // Визначення пропорції між точками
                    float t = (normalizedIntensity - lower.Threshold) / (upper.Threshold - lower.Threshold);

                    // Лінійна інтерполяція компонентів R, G, B
                    int r = (int)(lower.ColorValue.R + t * (upper.ColorValue.R - lower.ColorValue.R));
                    int g = (int)(lower.ColorValue.G + t * (upper.ColorValue.G - lower.ColorValue.G));
                    int b = (int)(lower.ColorValue.B + t * (upper.ColorValue.B - lower.ColorValue.B));

                    return Color.FromArgb(r, g, b);
                }
            }

            // Якщо інтенсивність виходить за межі LUT
            return normalizedIntensity < Lut[0].Threshold ? Lut[0].ColorValue : Lut[Lut.Length - 1].ColorValue;
        }


        private void UpdateColorMap(float[,] fieldIntensities, float minFieldMagnitude, float maxFieldMagnitude)
        {

            //int width = fieldMagnitudes.GetLength(0);
            //int height = fieldMagnitudes.GetLength(1);

            //int bitmapWidth = width * pixelBlockSize;
            //int bitmapHeight = height * pixelBlockSize;

            this.colorMapBitmap = new Bitmap(this.Width, this.Height, PixelFormat.Format24bppRgb);
            BitmapData bmpData = colorMapBitmap.LockBits(new Rectangle(0, 0, colorMapBitmap.Width, colorMapBitmap.Height),
                                                         ImageLockMode.WriteOnly, colorMapBitmap.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            int heightInPixels = bmpData.Height;        
            byte[] pixelBuffer = new byte[bmpData.Height * bmpData.Stride];

            float logMax = maxFieldMagnitude > 0 ? (float)(Math.Log10(maxFieldMagnitude + 1)) : 0;
            float logMin = minFieldMagnitude > 0 ? (float)(Math.Log10(minFieldMagnitude + 1)) : 0;
            float normalizedMagnitude = 0.0f;
            for (int x = 0, offset = 0; x < bmpData.Height; x++)
            {
                for (int y = 0, index = offset; y < bmpData.Width; y++, index +=3)
                {

                    float magnitude = fieldIntensities[y, x];

                    float logMagnitude = magnitude > 0 ? (float)(Math.Log10(magnitude + 1)) : 0;

                    if (logMax > logMin)
                        normalizedMagnitude = (logMagnitude - logMin) / (logMax - logMin);

                    normalizedMagnitude = Math.Clamp(normalizedMagnitude, 0.0f, 1.0f);

                    Color color = GetColor(normalizedMagnitude);

                    pixelBuffer[index] = color.B;
                    pixelBuffer[index + 1] = color.G;
                    pixelBuffer[index + 2] = color.R;
                }
                offset += bmpData.Stride;
            }

            Marshal.Copy(pixelBuffer, 0, ptr, pixelBuffer.Length);
            colorMapBitmap.UnlockBits(bmpData);
        }







        private void DrawFieldGrid(Graphics g, List<Charge> charges)
        {
            float gridWidth = this.Width - 2 * margin;
            float gridHeight = this.Height - 2 * margin;

            float topLeftX = margin;
            float topLeftY = margin;
         
            Pen gridPen = new Pen(Color.LightGray, 1);
            float[,] intensities = new float[this.Width, this.Height];
           

            float maxRadius = Math.Min(this.Width, this.Height) * 0.05f;
            float minRadius = 15f;
            float radius = Math.Max(maxRadius, minRadius);

            for (float x = topLeftX; x <= topLeftX + gridWidth; x += _gridSpacingX)
            {
                g.DrawLine(gridPen, x, topLeftY, x, topLeftY + gridHeight);

                for (float y = topLeftY; y <= topLeftY + gridHeight; y += _gridSpacingY)
                {
                    g.DrawLine(gridPen, topLeftX, y, topLeftX + gridWidth, y);
                    float worldX = (x - this.Width / 2) / (scale - maxRadius);
                    float worldY = (y - this.Height / 2) / (scale - maxRadius);
                    PointF fieldIntensity = ElectricField.CalculateField(charges, worldX, worldY);
                    float magnitude = (float)Math.Sqrt(fieldIntensity.X * fieldIntensity.X + fieldIntensity.Y * fieldIntensity.Y);
                 
                    ELectricFieldVector vector = new ELectricFieldVector(x, y, fieldIntensity, arrowLength / 2, scale);
                    vector.DrawStatic(g, Color.Red);
                }
            }

            gridPen.Dispose(); 
        }


        private void CalculateFieldIntensitiesOnGrid()
        {
            //int gridWidth = this.Width / blockSize;
            //int gridHeight = this.Height / blockSize;

            //float width = Math.Max(gridWidth, 1);
            //float height = Math.Max(gridHeight, 1);

            float[,] intensities = new float[this.Width, this.Height];

            float localMin = float.MaxValue;
            float localMax = float.MinValue;

            float maxRadius = Math.Min(this.Width, this.Height) * 0.05f;
            float minRadius = 15f;
            float radius = Math.Max(maxRadius, minRadius);

            for (int i = 0; i < this.Height; i++)
            {
                for (int j = 0; j < this.Width; j++)
                {
                    float worldX = (j - this.Width / 2) / (scale - maxRadius);
                    float worldY = (i - this.Height / 2) / (scale - maxRadius);
                    PointF fieldIntensity = ElectricField.CalculateField(this.charges, worldX, worldY);
                    float magnitude = (float)Math.Sqrt(fieldIntensity.X * fieldIntensity.X + fieldIntensity.Y * fieldIntensity.Y);


                    intensities[j, i] = magnitude;

                    if (magnitude < localMin) localMin = magnitude;
                    if (magnitude > localMax) localMax = magnitude;
                }
            }

            this.UpdateColorMap(intensities, localMin, localMax);
        }



        /// <summary>
        /// Událost časovače, která se spustí každých 16ms a aktualizuje polohu sondy a překresluje panel.
        /// </summary>
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Získání aktuálního času a výpočet deltaTime (čas mezi snímky).
            DateTime currentTime = DateTime.Now;
            this.deltaTime = (float)(currentTime - lastFrameTime).TotalSeconds; 
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
            if (_scenario.numberOfScenario == 4 && _scenario.charges.Count == 2)
            {
                _scenario.charges[0].Q = 1 + 0.5f * (float)Math.Sin(Math.PI / 2 * this.deltaTime);
                _scenario.charges[1].Q = 1 - 0.5f * (float)Math.Sin(Math.PI / 2 * this.deltaTime);
            }

            foreach (Charge charge in  charges)
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
