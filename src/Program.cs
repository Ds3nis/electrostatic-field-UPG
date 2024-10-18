using System.Windows.Forms;

namespace UPG_SP_2024
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Scenario scenario = null;

            if (args.Length > 0)
            {
                if (int.TryParse(args[0], out int number))
                {
                    scenario = new Scenario(number);
                    MessageBox.Show("Ziskane cele cislo: " + number, "Informace", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else 
                {
                    MessageBox.Show("Chyba: predany argument neni cele cislo.", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
            }
            else
            {
                scenario = new Scenario(0);  
            }

 
            if (scenario != null)
            {
                MainForm mainForm = new MainForm(scenario);
                Application.Run(mainForm);
            }




        }
    }
}