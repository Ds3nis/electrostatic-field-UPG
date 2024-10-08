using System.Drawing;
using System.Windows.Forms;


namespace UPG_SP_2024
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
        
            InitializeComponent();
            this.MinimumSize = new Size(800, 600);
            this.Size = new Size(800, 600);
        }

        private void drawingPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void drawingPanel_Resize(object sender, EventArgs e)
        {

        }
    }
}
