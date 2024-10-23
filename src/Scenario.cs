using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;


namespace UPG_SP_2024
{
    /// <summary>
    /// Třída reprezentující scénář pro zobrazení elektrických nábojů.
    /// Obsahuje seznam nábojů a číslo scénáře.
    /// </summary>
    public class Scenario
    {
        /// <summary>
        /// Číslo scénáře, které určuje specifický scénář nábojů.
        /// </summary>
        public readonly int numberOfScenario; 
        public readonly List<Charge> charges;

        /// <summary>
        /// Inicializuje novou instanci třídy Scenario s daným číslem scénáře.
        /// Vytváří seznam nábojů a volá metodu pro načtení nábojů podle scénáře.
        /// </summary>
        /// <param name="scenario">Číslo scénáře (int), které určuje, jaké náboje budou přidány do seznamu.</param>
        public Scenario(int scenario) {
            this.charges = new List<Charge>();
            this.numberOfScenario = scenario;
            GetChargesForScenario(numberOfScenario);
        }

        /// <summary>
        /// Načítá náboje podle zadaného čísla scénáře a přidává je do seznamu.
        /// </summary>
        /// <param name="scenario">Číslo scénáře (int), podle kterého se určují přidané náboje.</param>
        private void GetChargesForScenario(int scenario)
        {
            switch (scenario)
            {
                case 0:
                    this.charges.Add(new Charge(0, 0, 1));
                    break;
                case 1:
                    this.charges.Add(new Charge(-1, 0, 1));
                    this.charges.Add(new Charge(1, 0, 1));
                    break;
                case 2:
                    this.charges.Add(new Charge(-1, 0, -1));
                    this.charges.Add(new Charge(1, 0, 2));
                    break;
                case 3:
                    this.charges.Add(new Charge(-1, -1, 1));
                    this.charges.Add(new Charge(1, -1, 2));
                    this.charges.Add(new Charge(1, 1, -3));
                    this.charges.Add(new Charge(-1, 1, -4));
                    break;

                default:
                    this.charges.Add(new Charge(0, 0, 1));

                    break;
            }
        }
    }
}
