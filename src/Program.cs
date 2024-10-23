using System.Windows.Forms;

namespace UPG_SP_2024
{
    internal static class Program
    {
        /// <summary>
        /// Hlavn? vstupn? bod pro aplikaci.
        /// Inicializuje vizu?ln? styly a spust? hlavn? okno aplikace.
        /// Pokud jsou p?ed?ny argumenty, pokus? se prvn? z nich p?ev?st na cel? ??slo.
        /// V p??pad? ?sp?chu je vytvo?en sc?n?? s t?mto ??slem.
        /// Pokud p?evod sel?e, zobraz? se chybov? zpr?va a aplikace se ukon??.
        /// Pokud nejsou p?ed?ny ??dn? argumenty, vytvo?? se v?choz? sc?n??.
        /// </summary>
        /// <param name="args">Pole ?et?zc? obsahuj?c? argumenty p??kazov?ho ??dku p?edan? aplikaci.</param>
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

          
               MainForm mainForm = new MainForm(scenario);
               Application.Run(mainForm);
            




        }
    }
}