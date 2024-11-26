using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UPG_SP_2024;

namespace UPG_SP_2024
{
    /// <summary>
    /// Třída pro výpočet intenzity elektrického pole v určité souřadnici
    /// </summary>
    public class ElectricField
    {
        // Hodnota permittivity vakua.
        public const float Epsilon_0 = (float)(8.854e-12);

        /// <summary>
        /// Vypočítá intenzitu v daném bodě na základě seznamu elektrických nábojů.
        /// </summary>
        /// <param name="charges">Seznam nábojů typu <see cref="Charge"/>, které ovlivňují elektrické pole.</param>
        /// <param name="x">X-ová souřadnice zondy, kde se elektrické pole vypočítá.</param>
        /// <param name="y">Y-ová souřadnice zondy, kde se elektrické pole vypočítá.</param>
        /// <returns>Vrátí bod typu <see cref="PointF"/> představující vektor elektrického pole.</returns>
        public static PointF CalculateField(List<Charge> charges, float x, float y)
        {
             
            PointF field = new PointF(0, 0);  

            foreach (Charge charge in charges)
            {
             
                float dx = charge.X - x;
                float dy = charge.Y - y;
                float r = (float)Math.Sqrt(dx * dx + dy * dy); 
                float vectorToCube = r * r * r;
              
                if (r == 0) continue;


                // Výpočet síly elektrického pole podle Coulombova zákona.
                float force =  -charge.Q / (4 * (float)Math.PI * Epsilon_0 * (vectorToCube));


                // Aktualizace složek elektrického pole.
                field.X += force * dx;
                field.Y += force * dy;
            }

            return field;
        }


    }
}







//> 👾:
//using System;
//using System.Drawing;
//using System.Drawing.Imaging;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System.Runtime.InteropServices;
//using System.Drawing.Drawing2D;

//namespace ElectricFieldVisualization
//{
//    public class ElectricFieldIntensityMap : IDisposable
//    {
//        private readonly List<Charge> charges;
//        private readonly double proximityThreshold;
//        private float scale;
//        private double offsetX;
//        private double offsetY;
//        private double chargeRadiusWorld;

//        private double[,] fieldMagnitudes;
//        private double minFieldMagnitude = double.MaxValue;
//        private double maxFieldMagnitude = double.MinValue;

//        private readonly int gridStep;

//        private int panelWidth;
//        private int panelHeight;

//        private Bitmap? cachedBitmap = null;
//        private bool isCacheValid = false;

//        private readonly object fieldLock = new object();

//        private int pixelBlockSize;

//        private readonly Color[] colorLookup;

//        public ElectricFieldIntensityMap(
//            List<Charge> charges,
//            double proximityThreshold,
//            float scale,
//            double offsetX,
//            double offsetY,
//            double chargeRadiusWorld,
//            int panelWidth,
//            int panelHeight,
//            int gridStep = 1,
//            int pixelBlockSize = 1)
//        {
//            this.charges = new List<Charge>(charges) ?? throw new ArgumentNullException(nameof(charges));
//            this.proximityThreshold = proximityThreshold;
//            this.scale = scale;
//            this.offsetX = offsetX;
//            this.offsetY = offsetY;
//            this.chargeRadiusWorld = chargeRadiusWorld;
//            this.panelWidth = panelWidth;
//            this.panelHeight = panelHeight;
//            this.gridStep = gridStep > 0 ? gridStep : throw new ArgumentException("Grid step must be positive.", nameof(gridStep));

//            this.pixelBlockSize = pixelBlockSize > 0 ? pixelBlockSize : 1;

//            CalculateFieldMagnitudes();

//            colorLookup = new Color[256];
//            for (int i = 0; i < 256; i++)
//            {
//                double normalized = i / 255.0;
//                colorLookup[i] = GetColorFromMagnitude(normalized);
//            }
//        }

//        public void UpdateParameters(
//            float scale,
//            double offsetX,
//            double offsetY,
//            double chargeRadiusWorld,
//            int panelWidth,
//            int panelHeight,
//            List<Charge> chargesSnapshot)
//        {
//            lock (fieldLock)
//            {
//                this.scale = scale;
//                this.offsetX = offsetX;
//                this.offsetY = offsetY;
//                this.chargeRadiusWorld = chargeRadiusWorld;
//                this.panelWidth = panelWidth;
//                this.panelHeight = panelHeight;
//                this.charges.Clear();
//                this.charges.AddRange(chargesSnapshot);

//                InvalidateCache();
//                CalculateFieldMagnitudes();
//            }
//        }

//        public void SetPixelBlockSize(int newPixelBlockSize)
//        {
//            lock (fieldLock)
//            {
//                this.pixelBlockSize = newPixelBlockSize > 0 ? newPixelBlockSize : 1;
//                InvalidateCache();
//                CalculateFieldMagnitudes();
//            }
//        }

//        public Task CalculateFieldMagnitudesAsync()
//        {
//            return Task.Run(() => CalculateFieldMagnitudes());
//        }

//        public void CalculateFieldMagnitudes()
//        {
//            int width = panelWidth / (gridStep * pixelBlockSize);
//            int height = panelHeight / (gridStep * pixelBlockSize);

//            width = Math.Max(width, 1);
//            height = Math.Max(height, 1);

//            double[,] newFieldMagnitudes = new double[width, height];
//            double localMin = double.MaxValue;
//            double localMax = double.MinValue;

//> 👾:
//            List<Charge> chargesSnapshot;
//            lock (fieldLock)
//            {
//                chargesSnapshot = new List<Charge>(charges);
//            }

//            Parallel.For(0, width, x =>
//            {
//                for (int y = 0; y < height; y++)
//                {
//                    PointD screenPoint = new PointD(x * gridStep * pixelBlockSize, y * gridStep * pixelBlockSize);
//                    PointD worldPoint = ToWorldCoordinates(screenPoint);

//                    PointD field = Physics.CalculateTotalElectricField(worldPoint, chargesSnapshot, proximityThreshold);
//                    double magnitude = field.Magnitude();
//                    newFieldMagnitudes[x, y] = magnitude;

//                    if (magnitude < localMin) localMin = magnitude;
//                    if (magnitude > localMax) localMax = magnitude;
//                }
//            });

//            lock (fieldLock)
//            {
//                fieldMagnitudes = newFieldMagnitudes;
//                minFieldMagnitude = localMin;
//                maxFieldMagnitude = localMax;
//                isCacheValid = false;
//            }
//        }

//        public void Draw(Graphics g)
//        {
//            if (!isCacheValid)
//            {
//                GenerateBitmap();
//            }

//            if (cachedBitmap != null)
//            {
//                g.DrawImageUnscaled(cachedBitmap, 0, 0);
//            }
//        }

//        private void GenerateBitmap()
//        {
//            if (fieldMagnitudes == null)
//            {
//                throw new InvalidOperationException("fieldMagnitudes не были рассчитаны. Вызовите CalculateFieldMagnitudes() перед генерацией изображения.");
//            }

//            int width = fieldMagnitudes.GetLength(0);
//            int height = fieldMagnitudes.GetLength(1);

//            int bitmapWidth = width * pixelBlockSize;
//            int bitmapHeight = height * pixelBlockSize;

//            Bitmap bitmap = new Bitmap(bitmapWidth, bitmapHeight, PixelFormat.Format32bppArgb);

//            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmapWidth, bitmapHeight),
//                                                ImageLockMode.WriteOnly,
//                                                bitmap.PixelFormat);

//            try
//            {
//                int bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
//                int stride = bmpData.Stride;
//                IntPtr scan0 = bmpData.Scan0;
//                int byteCount = stride * bitmapHeight;
//                byte[] pixels = new byte[byteCount];

//                Parallel.For(0, height, y =>
//                {
//                    for (int x = 0; x < width; x++)
//                    {
//                        double magnitude = fieldMagnitudes[x, y];

//                        double logMagnitude = magnitude > 0 ? Math.Log10(magnitude + 1) : 0;
//                        double logMax = maxFieldMagnitude > 0 ? Math.Log10(maxFieldMagnitude + 1) : 0;
//                        double logMin = minFieldMagnitude > 0 ? Math.Log10(minFieldMagnitude + 1) : 0;
//                        double normalizedMagnitude = 0.0;

//                        if (logMax > logMin)
//                            normalizedMagnitude = (logMagnitude - logMin) / (logMax - logMin);

//                        normalizedMagnitude = Math.Clamp(normalizedMagnitude, 0.0, 1.0);

//                        int colorIndex = (int)(normalizedMagnitude * 255.0);
//                        colorIndex = Math.Clamp(colorIndex, 0, 255);
//                        Color color = colorLookup[colorIndex];

//                        for (int dy = 0; dy < pixelBlockSize; dy++)
//                        {
//                            int row = y * pixelBlockSize + dy;
//                            if (row >= bitmapHeight) continue;

//                            for (int dx = 0; dx < pixelBlockSize; dx++)
//                            {
//                                int col = x * pixelBlockSize + dx;
//                                if (col >= bitmapWidth) continue;

//> 👾:
//int pixelIndex = row * stride + col * bytesPerPixel;
//                                pixels[pixelIndex] = color.B;
//                                pixels[pixelIndex + 1] = color.G;
//                                pixels[pixelIndex + 2] = color.R;
//                                pixels[pixelIndex + 3] = color.A;
//                            }
//                        }
//                    }
//                });

//                Marshal.Copy(pixels, 0, scan0, pixels.Length);
//            }
//            finally
//            {
//                bitmap.UnlockBits(bmpData);
//            }

//            lock (fieldLock)
//            {
//                cachedBitmap?.Dispose();

//                if (bitmapWidth != panelWidth || bitmapHeight != panelHeight)
//                {
//                    Bitmap resizedBitmap = new Bitmap(bitmap, panelWidth, panelHeight);
//                    bitmap.Dispose();
//                    cachedBitmap = resizedBitmap;
//                }
//                else
//                {
//                    cachedBitmap = bitmap;
//                }
//                isCacheValid = true;
//            }
//        }

//        private Color GetColorFromMagnitude(double magnitude)
//        {
//            magnitude = Math.Clamp(magnitude, 0, 1);

//            int r = 0, g = 0, b = 0;

//            if (magnitude < 0.143)
//            {
//                double ratio = magnitude / 0.143;
//                r = 0;
//                g = (int)(127 * ratio);
//                b = 255;
//            }
//            else if (magnitude < 0.286)
//            {
//                double ratio = (magnitude - 0.143) / 0.143;
//                r = 0;
//                g = (int)(127 + (128 * ratio));
//                b = 255;
//            }
//            else if (magnitude < 0.429)
//            {
//                double ratio = (magnitude - 0.286) / 0.143;
//                r = 0;
//                g = 255;
//                b = (int)(255 * (1 - ratio));
//            }
//            else if (magnitude < 0.571)
//            {
//                double ratio = (magnitude - 0.429) / 0.143;
//                r = (int)(255 * ratio);
//                g = 255;
//                b = 0;
//            }
//            else if (magnitude < 0.714)
//            {
//                double ratio = (magnitude - 0.571) / 0.143;
//                g = (int)(255 * (1 - ratio) + 165 * ratio);
//                r = 255;
//                b = 0;
//            }
//            else if (magnitude < 0.857)
//            {
//                double ratio = (magnitude - 0.714) / 0.143;
//                r = 255;
//                g = (int)(165 * (1 - ratio));
//                b = 0;
//            }
//            else
//            {
//                double ratio = (magnitude - 0.857) / 0.143;
//                r = 255 - (int)(29 * ratio);
//                g = (int)(0 + 47 * ratio);
//                b = (int)(0 + 244 * ratio);
//            }

//            return Color.FromArgb(255, r, g, b);
//        }

//        private PointD ToWorldCoordinates(PointD point)
//        {
//            double worldX = (point.X - offsetX) / scale;
//            double worldY = -(point.Y - offsetY) / scale;
//            return new PointD(worldX, worldY);
//        }

//        public PointD ToScreenCoordinates(PointD worldPoint)
//        {
//            double screenX = worldPoint.X * scale + offsetX;
//            double screenY = -worldPoint.Y * scale + offsetY;
//            return new PointD(screenX, screenY);
//        }

//        public double MinFieldMagnitude => minFieldMagnitude;
//        public double MaxFieldMagnitude => maxFieldMagnitude;

//        public void InvalidateCache()
//        {
//            lock (fieldLock)
//            {
//                cachedBitmap?.Dispose();
//                cachedBitmap = null;
//                isCacheValid = false;
//            }
//        }

//        public void Dispose()
//        {
//            lock (fieldLock)
//            {
//                cachedBitmap?.Dispose();
//                cachedBitmap = null;
//                isCacheValid = false;
//            }
//        }
//    }
//}

