using System.Drawing;
using System.Windows.Forms;


namespace UPG_SP_2024
{
    public partial class MainForm : Form
    {
        private Scenario scenario;

        public MainForm(Scenario scenario)
        {

            InitializeComponent();
            this.Size = new Size(800, 600);
            this.scenario = scenario;
            this.drawingPanel._scenario = scenario;
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
