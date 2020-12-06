using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    public class ChemicalEq
    {
        Dictionary<Tuple<string,int>, List<Tuple<string,int>>> eqs = new Dictionary<Tuple<string,int>, List<Tuple<string,int>>>();

        Dictionary<string,double> reqAmmounts = new Dictionary<string, double>();
        Dictionary<string,double> extras = new Dictionary<string, double>();
        public ChemicalEq(List<string> inputs)
        {
            foreach(var input in inputs)
            {
                var parts = input.Split("=>");
                var ins = parts[0].Split(",");

                List<Tuple<string,int>> eqIns = new List<Tuple<string, int>>();
                foreach(var el in ins)
                {
                    var elParts = el.Split(" ").ToList();
                    elParts.RemoveAll(x => x=="");

                    var name = elParts[1];
                    if(!reqAmmounts.ContainsKey(name))
                    {
                        reqAmmounts.Add(name,0);
                        extras.Add(name,0);
                    }

                    var quant = Int32.Parse(elParts[0]);
                    eqIns.Add(new Tuple<string, int>(name, quant));
                }

                var outs = parts[1].Split(" ").ToList();
                outs.RemoveAll(x => x=="");

                if(!reqAmmounts.ContainsKey(outs[1]))
                {
                    reqAmmounts.Add(outs[1],0);
                    extras.Add(outs[1],0);
                }
                var eqOut = new Tuple<string,int>(outs[1],Int32.Parse(outs[0]));

                eqs.Add(eqOut, eqIns);
            }

        }

        public double GetOreForFuel(int ammount)
        {
            foreach(var key in reqAmmounts.Keys)
            {
                reqAmmounts[key] = 0;
                extras[key] = 0;
            }

            reqAmmounts["FUEL"] = ammount;

            while(reqAmmounts.Any(x=> x.Key !="ORE" && x.Value != 0))
            {
                foreach(var reqKey in reqAmmounts.Keys)
                {
                    if(reqKey == "ORE" || reqAmmounts[reqKey]==0)
                        continue;

                    GetReqAmounts(reqKey, reqAmmounts[reqKey]);
                }

            }
            return GetTotalOre();
        }

        public void GetOreForOneFuel()
        {
            Console.WriteLine("Min Ore For 1 Fuel: " + GetOreForFuel(1));
        }

        public void GetFuelForTrillionOre()
        {
            int maxpowerOf10 = 0;
            int fuelGuess = 0;
            double oreForGuess = 0;
            double maxOre = 1000000000000;
            do
            {
                Console.WriteLine("Checking " + fuelGuess.ToString());
                oreForGuess = GetOreForFuel(fuelGuess);

                if(oreForGuess<maxOre)
                {
                    maxpowerOf10++;
                    fuelGuess = (int)Math.Pow(10,maxpowerOf10);
                }
            }
            while(oreForGuess < maxOre);

            fuelGuess = 0;
            oreForGuess = 0;
            for(int pow = maxpowerOf10; pow >= 0; pow--)
            {
                if(oreForGuess<maxOre)
                {
                    while(oreForGuess < maxOre)
                    {
                        Console.WriteLine("Checking " + fuelGuess.ToString());
                        fuelGuess+= (int)Math.Pow(10,pow);
                        oreForGuess = GetOreForFuel(fuelGuess);
                    }
                }
                else
                {
                    while(oreForGuess > maxOre)
                    {
                        Console.WriteLine("Checking " + fuelGuess.ToString());
                        fuelGuess-= (int)Math.Pow(10,pow);
                        oreForGuess = GetOreForFuel(fuelGuess);
                    }
                }
            }

            if(oreForGuess > maxOre)
                fuelGuess -= 1;

            Console.WriteLine("Max Fuel for 1 Trillion Ore: " + (fuelGuess).ToString());
        }

        public void GetReqAmounts(string elementName, double ammount)
        {
            var key = eqs.Keys.FirstOrDefault(x => x.Item1 == elementName);
            if(key == null)
                return;

            var elementsThatRelyOnThisElement = eqs.Where(eq => eq.Value.Select(el => el.Item1).Any(elName => elName==elementName)).Select(eq => eq.Key);
            if(elementsThatRelyOnThisElement.Any(el => !reqAmmounts.ContainsKey(el.Item1) || reqAmmounts[el.Item1] != 0))
                return;

            double reqRuns = 0;
            if(extras[elementName] >= ammount)
            {
                reqAmmounts[elementName] = 0;
                extras[elementName] -= ammount;
                return;
            }

            reqRuns =  Math.Ceiling((double)(ammount-extras[elementName])/(double)key.Item2);

            foreach(var inputEl in eqs[key])
            {   
                if(reqAmmounts.ContainsKey(inputEl.Item1))
                {
                    reqAmmounts[inputEl.Item1] += reqRuns*inputEl.Item2;
                }
                else
                    reqAmmounts.Add(inputEl.Item1, reqRuns*inputEl.Item2);
            }

            extras[elementName] += reqRuns*key.Item2 - ammount;
            reqAmmounts[elementName] = 0;

        }

        public double GetTotalOre()
        {
            return reqAmmounts["ORE"];
        }
    }


}