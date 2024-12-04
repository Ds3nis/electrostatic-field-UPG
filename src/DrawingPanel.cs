using ElectricFieldVis;
using Microsoft.VisualBasic.Logging;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
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
        public float drawingPanelScale;
        public float globalScale;
        private System.Windows.Forms.Timer timer;
        private Probe probe1;
        private DateTime lastFrameTime;
        float arrowLength = 15f;
        List<Charge> charges;
        float deltaTime;
        float margin = 50f;
        public int _gridSpacingX; // Výchozí rozteč mřížky v ose X
        public int _gridSpacingY; // Výchozí rozteč mřížky v ose Y

        private Charge selectedCharge = null;
        private Point lastMousePosition;
        private ToolTip chargeToolTip;
        private int blockSize = 5;
        private float totalTime = 0.0f;


        private ColourMap[] Lut { get; set; }


        private Bitmap colorMapBitmap;

        float minFieldMagnitude;
        float maxFieldMagnitude;

        private float squareSize;
        private float topLeftX;
        private float topLeftY;



        public ProbeManager probeManager;
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
            
            probe1 = new Probe(1, 0f, 0f, Color.Green);
            probeManager = new ProbeManager();
            //probeManager.AddProbe(probe1, $"Probe {probeManager.collectors.Count + 1}");
       
        
            lastFrameTime = DateTime.Now;

            timer = new System.Windows.Forms.Timer();
            this.MouseDown += DrawingPanel_MouseDown;
            this.MouseMove += DrawingPanel_MouseMove;
            this.MouseUp += DrawingPanel_MouseUp;
            this.MouseWheel += DrawingPanel_MouseWheel;
            this.MouseClick += DrawingPanel_MouseClick;
            timer.Interval = 60; 
            timer.Tick += Timer_Tick;
            timer.Start();

        }

        private void DrawingPanel_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                float worldX = (e.X - this.Width / 2) / scale;
                float worldY = (e.Y - this.Height / 2) / scale;
                var newProbe = new Probe(1, worldX, worldY);

                string seriesName = $"Probe {probeManager.collectors.Count + 1}";
                probeManager.AddProbe(newProbe, seriesName);



                if (!probeManager.graphForm.Visible)
                {
                    probeManager.graphForm.Show();
                }

                
            }
        }

        private void DrawingPanel_MouseWheel(object? sender, MouseEventArgs e)
        {
            foreach (Charge charge in charges)
            {
                float dx = e.X - charge.ScreenX;
                float dy = e.Y - charge.ScreenY;
                float distance = (float)Math.Sqrt(dx * dx + dy * dy);

                if (distance <= charge.Radius) // Якщо курсор над зарядом
                {
                    // Змінюємо радіус залежно від напрямку прокрутки
                    float delta = e.Delta > 0 ? 1f : -1f; // Збільшення або зменшення радіуса
                    if (charge.Q > 0)
                    {
                        float value = charge.Q + delta * 0.05f;//Наприклад, змінюємо Q, щоб враховувати логіку масштабу
                        if(value <= 0)
                        {
                            return;
                        }
                        if(this._scenario.numberOfScenario == 4)
                        {
                            float scenario4Value = charge.Q + charge.Scenario4Q + delta * 0.05f;

                            if (scenario4Value - charge.Q < -0.5 || scenario4Value <=0)
                            {
                                return;
                            }

                            charge.Scenario4Q = charge.Scenario4Q +  delta * 0.05f;
                        }
                        charge.Q = value; 
                    }

                    if(charge.Q < 0)
                    {
                        float value =  charge.Q - delta * 0.05f;//Наприклад, змінюємо Q, щоб враховувати логіку масштабу
                        if (value >= -0.01)
                        {
                            return;
                        }
                        charge.Q = value;
                    }
                   

                    charge.UpdateRadius(); // Оновлюємо радіус із врахуванням нового значення
                
                    break;
                }
            }
        }

        private void DrawingPanel_MouseUp(object? sender, MouseEventArgs e)
        {
            selectedCharge = null;
        }

        private void DrawingPanel_MouseMove(object? sender, MouseEventArgs e)
        {
            if (selectedCharge != null)
            {
                float panelLeft = 0;
                float panelRight = this.Width;
                float panelTop = 0;
                float panelBottom = this.Height;

                // Якщо мишка за межами панелі, не оновлювати позицію заряду
                if (e.X < panelLeft || e.X > panelRight || e.Y < panelTop || e.Y > panelBottom)
                {
                    this.selectedCharge = null;
                    return;
                }
                  

                // Обчислення зміщення миші
                float dx = (e.X - lastMousePosition.X) / (scale - selectedCharge.Radius);
                float dy = (e.Y - lastMousePosition.Y) / (scale - selectedCharge.Radius);

                // Оновлення позиції заряду
                selectedCharge.X += dx;
                selectedCharge.Y += dy;

                // Отримання екранних координат заряду
                PointF screenPoint = selectedCharge.WorldToScreen(topLeftX, topLeftY, squareSize, selectedCharge.Radius);

                // Обмеження позиції заряду в межах панелі
                float radius = selectedCharge.Radius;
                if (screenPoint.X < radius + this.margin)
                    selectedCharge.X -= dx;
                if (screenPoint.X > this.Width - radius - this.margin)
                    selectedCharge.X -= dx;
                if (screenPoint.Y < radius + this.margin)
                    selectedCharge.Y -= dy;
                if (screenPoint.Y > this.Height - radius - margin)
                    selectedCharge.Y -= dy;

                // Збереження останньої позиції миші
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

                    chargeToolTip.Show($"Position of mouse: ({e.X}, {e.Y})\nCharge: ({charge.X}, {charge.Y})",
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


            squareSize = Math.Min(this.Width, this.Height) - (2 * margin);


            topLeftX = (this.Width - squareSize) / 2f;
            topLeftY = (this.Height - squareSize) / 2f;
            g.DrawRectangle(Pens.Black, topLeftX, topLeftY, squareSize, squareSize);

            scale = (squareSize / 2f);
            drawingPanelScale = squareSize;

            this.CalculateFieldIntensitiesOnGrid();

            if (colorMapBitmap != null)
            {
                g.DrawImage(colorMapBitmap, 0, 0, this.Width, this.Height);
            }
        
            this.DrawFieldGrid(g, charges);
            this.DrawCharges(g, charges, this.Width, this.Height, topLeftX, topLeftY, squareSize);
           
       
            foreach(Probe probe in this.probeManager.probes)
            {
                probe.Draw(e.Graphics, this.Width, this.Height, this.scale);
                probe.calculateIntensity(this.charges, this.scale);        
            }
            this.probeManager.UpdateProbes();
            probe1.Draw(e.Graphics, this.Width, this.Height, this.scale);
            probe1.calculateIntensity(this.charges, this.scale);

            // Výpočet intenzity elektrického pole v místě sondy.


            // Vytvoření a vykreslení vektoru elektrického pole
            ELectricFieldVector vector = new ELectricFieldVector(probe1.ScreenX, probe1.ScreenY, probe1.intensityPoint, arrowLength, scale);

            vector.Draw(g, Color.Green);

            DrawColorMapLegend(g, minFieldMagnitude, maxFieldMagnitude);


        }





        private void DrawColorMapLegend(Graphics g, float minFieldMagnitude, float maxFieldMagnitude)
        {
            // Налаштування для легенди
            float legendWidth = 20; // Ширина легенди
            float legendHeight = 300; // Висота легенди
            float legendX = this.Width - legendWidth - 5; // Позиція по X
            float legendY = this.Height / 2 - legendHeight / 2; // Центрування по Y

            // Розмір кожного кольорового блоку в легенді
            int numBlocks = 80; // Кількість блоків
            float blockHeight = legendHeight / numBlocks;


            // Межі LUT для нормалізації
            float lutMin = Lut[0].Threshold;
            float lutMax = Lut[Lut.Length - 1].Threshold;

            // Малюємо кольорові блоки
            for (int i = numBlocks; i > 0; i--)
            {
                float normalizedMagnitude = lutMin + i * (lutMax - lutMin) / (numBlocks - 1);
                Color color = GetColor(normalizedMagnitude);

                using (Brush brush = new SolidBrush(color))
                {
                    g.FillRectangle(brush, legendX, (legendY + legendHeight) - i *blockHeight, legendWidth, blockHeight);
                }
            }
            // Логарифмічні межі
            float logMin = (float)Math.Log10(minFieldMagnitude);
            float logMax = (float)Math.Log10(maxFieldMagnitude);


            // Додавання підписів
            Font font = new Font("Arial", 10);
            Brush textBrush = new SolidBrush(Color.Black);

            // Кількість підписів (включно з мінімумом і максимумом)
            int numLabels = 5;
            float labelStep = (logMax - logMin) / (numLabels - 1);

            for (int i = 0; i < numLabels; i++)
            {
                // Обчислення нормалізованого значення магнітуди
                float normalizedLabel = logMin + i * labelStep;
                float magnitude = (float)Math.Pow(10, normalizedLabel);

                // Форматування у вигляді 10^x
                int exponent = (int)Math.Floor(Math.Log10(magnitude));
                float mantissa = magnitude / (float)Math.Pow(10, exponent);
                string label = $"{mantissa:F1}×10^{exponent}";


                // Розрахунок позиції підпису
                float positionY = legendY + legendHeight - (normalizedLabel - logMin) / (logMax - logMin) * legendHeight;

                // Відображення підпису
                g.DrawString(label, font, textBrush, legendX - 50, positionY - font.Size / 2);
            }
        }


        private ColourMap[] MakeLut()
        {

            List<ColourMap> colorMap = new List<ColourMap>
            {
                new ColourMap(0.1f, Color.Blue),
                new ColourMap(0.15f, Color.Cyan),
                new ColourMap(0.2f, Color.LimeGreen),
                new ColourMap(0.3f, Color.Yellow),
                new ColourMap(0.6f, Color.Red)
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
            int bitmapWidth = this.Width;
            int bitmapHeight = this.Height;
            int stride = ((bitmapWidth * 24 + 31) / 32) * 4; // Обчислюємо stride для 24bpp

            byte[] pixelBuffer = new byte[stride * bitmapHeight];

            this.minFieldMagnitude = minFieldMagnitude;
            this.maxFieldMagnitude = maxFieldMagnitude;

            float logMax = maxFieldMagnitude > 0 ? (float)Math.Log10(maxFieldMagnitude + 1) : 0;
            float logMin = minFieldMagnitude > 0 ? (float)Math.Log10(minFieldMagnitude + 1) : 0;

            int blockHeight = fieldIntensities.GetLength(1);
            int blockWidth = fieldIntensities.GetLength(0);

            // Паралельна обробка
            Parallel.For(0, blockHeight, i =>
            {
                for (int j = 0; j < blockWidth; j++)
                {
                    float magnitude = fieldIntensities[j, i];
                    float logMagnitude = magnitude > 0 ? (float)Math.Log10(magnitude + 1) : 0;

                    float normalizedMagnitude = (logMagnitude - logMin) / (logMax - logMin);
                    normalizedMagnitude = Math.Clamp(normalizedMagnitude, 0.0f, 1.0f);

                    Color color = GetColor(normalizedMagnitude);

                    for (int y = i * blockSize; y < (i + 1) * blockSize && y < bitmapHeight; y++)
                    {
                        for (int x = j * blockSize; x < (j + 1) * blockSize && x < bitmapWidth; x++)
                        {
                            int index = y * stride + x * 3;
                            pixelBuffer[index] = color.B;
                            pixelBuffer[index + 1] = color.G;
                            pixelBuffer[index + 2] = color.R;
                        }
                    }
                }
            });

            // Паралельна обробка
            //for (int i = 0; i < blockHeight; i++)
            //{
            //    {
            //        for (int j = 0; j < blockWidth; j++)
            //        {
            //            float magnitude = fieldIntensities[j, i];
            //            float logMagnitude = magnitude > 0 ? (float)Math.Log10(magnitude + 1) : 0;

            //            float normalizedMagnitude = (logMagnitude - logMin) / (logMax - logMin);
            //            normalizedMagnitude = Math.Clamp(normalizedMagnitude, 0.0f, 1.0f);

            //            Color color = GetColor(normalizedMagnitude);

            //            for (int y = i * blockSize; y < (i + 1) * blockSize && y < bitmapHeight; y++)
            //            {
            //                for (int x = j * blockSize; x < (j + 1) * blockSize && x < bitmapWidth; x++)
            //                {
            //                    int index = y * stride + x * 3;
            //                    pixelBuffer[index] = color.B;
            //                    pixelBuffer[index + 1] = color.G;
            //                    pixelBuffer[index + 2] = color.R;
            //                }
            //            }
            //        }
            //    }
            //};

            // Робота з Bitmap після завершення обробки
            this.colorMapBitmap = new Bitmap(bitmapWidth, bitmapHeight, PixelFormat.Format24bppRgb);
            BitmapData bmpData = colorMapBitmap.LockBits(new Rectangle(0, 0, bitmapWidth, bitmapHeight),
                                                         ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            Marshal.Copy(pixelBuffer, 0, bmpData.Scan0, pixelBuffer.Length);
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
           

            //float maxRadius = Math.Min(this.Width, this.Height) * 0.05f;
            //float minRadius = 15f;
            //float radius = Math.Max(maxRadius, minRadius);

            for (float x = topLeftX; x <= topLeftX + gridWidth; x += _gridSpacingX)
            {
                g.DrawLine(gridPen, x, topLeftY, x, topLeftY + gridHeight);

                for (float y = topLeftY; y <= topLeftY + gridHeight; y += _gridSpacingY)
                {
                    g.DrawLine(gridPen, topLeftX, y, topLeftX + gridWidth, y);
                    //float worldX = (x - this.Width / 2) / (scale - maxRadius);
                    //float worldY = (y - this.Height / 2) / (scale - maxRadius);
                    float worldX = (x - this.Width / 2);
                    float worldY = (y - this.Height / 2);
                    PointF fieldIntensity = ElectricField.CalculateField(charges, worldX, worldY, this.scale);
                    float magnitude = (float)Math.Sqrt(fieldIntensity.X * fieldIntensity.X + fieldIntensity.Y * fieldIntensity.Y);
                 
                    ELectricFieldVector vector = new ELectricFieldVector(x, y, fieldIntensity, arrowLength / 2, scale);
                    vector.DrawStatic(g, Color.Red);
                }
            }

            gridPen.Dispose(); 
        }


        private void CalculateFieldIntensitiesOnGrid()
        {
            int gridWidth = this.Width / this.blockSize;
            int gridHeight = this.Height / this.blockSize;

            float[,] intensities = new float[gridWidth, gridHeight];

            float localMin = float.MaxValue;
            float localMax = float.MinValue;

            //float maxRadius = Math.Min(this.Width, this.Height) * 0.05f;
            //float minRadius = 15f;
            //float radius = Math.Max(maxRadius, minRadius);

            // Використовуємо блокування для оновлення глобальних значень
            object lockObj = new object();

            Parallel.For(0, gridHeight, i =>
            {
                for (int j = 0; j < gridWidth; j++)
                {
                    int centerBlockX = j * blockSize + blockSize / 2;
                    int centerBlockY = i * blockSize + blockSize / 2;

                    //float worldX = (centerBlockX - this.Width / 2) / (scale - maxRadius);
                    //float worldY = (centerBlockY - this.Height / 2) / (scale - maxRadius);

                    float worldX = (centerBlockX - this.Width / 2);
                    float worldY = (centerBlockY - this.Height / 2);

                    PointF fieldIntensity = ElectricField.CalculateField(this.charges, worldX, worldY, this.scale);
                    float magnitude = (float)Math.Sqrt(fieldIntensity.X * fieldIntensity.X + fieldIntensity.Y * fieldIntensity.Y);

                    intensities[j, i] = magnitude;

                    // Оновлення мінімуму та максимуму з блокуванням
                    lock (lockObj)
                    {
                        if (magnitude < localMin) localMin = magnitude;
                        if (magnitude > localMax) localMax = magnitude;
                    }
                }
            });

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
            foreach (Probe probe in this.probeManager.probes)
            {
                probe.UpdatePosition(deltaTime);
            }
            probe1.UpdatePosition(deltaTime);
          

            

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
                totalTime += this.deltaTime;

                float q1 = 1 + 0.5f * (float)Math.Sin((Math.PI / 2) * totalTime); 
                float q2 = 1 - 0.5f * (float)Math.Sin((Math.PI / 2) * totalTime);

                q1 = (float)Math.Round(q1, 2);
                q2 = (float)Math.Round(q2, 2);

                _scenario.charges[0].Q = q1 + _scenario.charges[0].Scenario4Q;
                _scenario.charges[1].Q = q2 + _scenario.charges[1].Scenario4Q;
            }

          

            foreach (Charge charge in  charges)
            {      
                if(charge.X > 1 || charge.X < -1 || charge.Y > 1 || charge.Y < -1)
                {
                    globalScale = drawingPanelScale;
                }
                else
                {
                    globalScale = scale;
                }

          
             
                charge.Draw(g, panelWidth, panelHeight, topLeftX, topLeftY, squareSize, globalScale);
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
