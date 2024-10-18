using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;


namespace UPG_SP_2024
{
    
    public class Scenario
    {
        public readonly int numberOfScenario; 
        public readonly List<Charge> charges;


        public Scenario(int scenario) {
            this.charges = new List<Charge>();
            this.numberOfScenario = scenario;
            GetChargesForScenario(numberOfScenario);
        }

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
