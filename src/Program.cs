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
            Scenario scenario;
            int gridSpacingX = 50;
            int gridSpacingY = 50;


  

            if (args.Length > 0)
            {
                
                if (int.TryParse(args[0], out int number))
                {
                    foreach (string arg in args)
                    {
                        if (arg.StartsWith("-g"))
                        {
                            string[] dimensions = arg.Substring(2).Split('x');
                            if (dimensions.Length == 2 && int.TryParse(dimensions[0], out gridSpacingX) && int.TryParse(dimensions[1], out gridSpacingY))
                            {
                                break; // Nalezen platný parametr, ukonèit cyklus
                            }
                            else
                            {
                                MessageBox.Show("Chybný formát parametru -g. Oèekávaný formát: -g<X>x<Y>", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
               
                                break;
                            }
                        }
                    }
                    scenario = new Scenario(number);
                    MessageBox.Show("Ziskane cele cislo: " + number, "Informace", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else 
                {
                    MessageBox.Show("Chyba: predany argument neni cele cislo.", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                    scenario = new Scenario(0);
                }
            }
            else
            {
                scenario = new Scenario(4);  
            }

          
               MainForm mainForm = new MainForm(scenario, gridSpacingX, gridSpacingY);
               Application.Run(mainForm);
            




        }
    }
}