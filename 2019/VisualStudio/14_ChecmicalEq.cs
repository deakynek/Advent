using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    public class ChemicalEq
    {
        Dictionary<Tuple<string,int>, List<Tuple<string,int>>> eqs = new Dictionary<Tuple<string,int>, List<Tuple<string,int>>>();

        Dictionary<string,int> baseOreReqs = new Dictionary<string, int>();
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
                    var quant = Int32.Parse(elParts[0]);
                    eqIns.Add(new Tuple<string, int>(name, quant));
                }

                var outs = parts[1].Split(" ").ToList();
                outs.RemoveAll(x => x=="");
                var eqOut = new Tuple<string,int>(outs[1],Int32.Parse(outs[0]));

                eqs.Add(eqOut, eqIns);
            }

        }

        public void GetOreForOneFuel()
        {
            RecurseGetOre("FUEL", 1);
            Console.WriteLine(String.Format("Total Ore: {0}", GetTotalOre()));
        }

        public void RecurseGetOre(string elementName, int ammount)
        {
            var key = eqs.Keys.FirstOrDefault(x => x.Item1 == elementName);
            if(key == null)
                return;

            double reqRuns = Math.Ceiling((double)(ammount/(double)key.Item2));
            foreach(var inputEl in eqs[key])
            {
                if(inputEl.Item1 == "ORE")
                {   
                    if(baseOreReqs.ContainsKey(elementName))
                    {
                        baseOreReqs[elementName] += ammount;
                    }
                    else
                        baseOreReqs.Add(elementName,ammount);
                }
                else
                    RecurseGetOre(inputEl.Item1,(int) (reqRuns*inputEl.Item2));
            }
        }

        public double GetTotalOre()
        {
            double totalOre = 0;
            foreach(var el in baseOreReqs.Keys)
            {
                var key = eqs.Keys.FirstOrDefault(x => x.Item1 == el);
                if(key == null)
                    continue;

                var reqAmmount = (double)baseOreReqs[el];
                double runTimes = Math.Ceiling((double)(reqAmmount/key.Item2));

                foreach(var input in eqs[key])
                {
                    if(input.Item1 == "ORE")
                    {
                        totalOre +=input.Item2 * runTimes;
                    }
                }
            }

            return totalOre;
        }
    }


}