using System.Drawing;
using System.Windows.Forms;


namespace UPG_SP_2024
{
    /// <summary>
    /// Hlavn? t??da formul??e aplikace.
    /// </summary>
    public partial class MainForm : Form
    {
        private Scenario scenario;


        /// <summary>
        /// Inicializuje novou instanci t??dy MainForm.
        /// </summary>
        /// <param name="scenario">Objekt typu <see cref="Scenario"/>, kter? obsahuje informace o sc?n??i, 
        /// jako jsou elektrick? n?boje a jejich um?st?n?.</param>
        public MainForm(Scenario scenario)
        {
   
            InitializeComponent(); // Inicializuje komponenty formul??e.
            this.MinimumSize = new Size(200, 200); // Nastav? minim?ln? velikost formul??e.
            this.Size = new Size(800, 600); // Nastav? v?choz? velikost formul??e.
            this.scenario = scenario; // Ulo?? sc?n?? do soukrom? prom?nn?.
            this.drawingPanel._scenario = scenario; // P?ed? sc?n?? do vykreslovac?ho panelu.
        }

        private void drawingPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

        }

        private void drawingPanel_Resize(object sender, EventArgs e)
        {

        }

        private void drawingPanel_ParentChanged(object sender, EventArgs e)
        {

        }
    }
}
