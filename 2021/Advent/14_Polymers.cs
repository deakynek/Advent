using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    class Polymers
    {
        List<int> entries;

        string formula = "";
        Dictionary<string,string> formulas = new Dictionary<string, string>();
        Dictionary<string,long> compounds = new Dictionary<string, long>(); 


        public Polymers(List<string> lines)
        {
            formula= lines[0].Trim();

            for(int i = 2; i< lines.Count(); i++)
            {
                formulas.Add(lines[i].Substring(0,2), lines[i].Trim().Last().ToString());
            }

            for(int i = 0; i<formula.Length-1; i++)
            {
                if(!compounds.ContainsKey(formula.Substring(i,2)))
                    compounds.Add(formula.Substring(i,2),0);
                
                compounds[formula.Substring(i,2)] += 1;
                    
            }

            string curr = formula;
            for(int i = 0; i< 10; i++)
                curr = ApplyRulesPart1(curr);

            Console.WriteLine(CalcScore(curr));


            for(int i = 0; i< 40; i++)
                ApplyRulesPart2();

            Console.WriteLine(CalcScorePart2());            
        }

        private string ApplyRulesPart1(string curr)
        {
            int i = curr.Length-2;
            while(i>=0)
            {
                var code = curr.Substring(i,2);
                if(formulas.ContainsKey(code))
                {
                    curr = curr.Insert(i+1,formulas[code]);
                }
                
                i -=1;
            }

            return curr;

        }

        private void ApplyRulesPart2()
        {
            var currKeys = compounds.ToDictionary(x=>x.Key,y=>y.Value);
            foreach(var key in currKeys.Keys.ToList())
            {
                var prevCount = compounds.Values.Sum();
                var adder = formulas[key];
                var produces= new List<string>(){key.Substring(0,1)+adder, adder+key.Substring(1,1)};

                var totComp = currKeys[key];
                compounds[key] -=totComp;

                foreach(var p in produces)
                {
                    if(!compounds.ContainsKey(p))
                        compounds.Add(p,0);
                    compounds[p] += totComp;
                }

                    
            }

        }

        private long CalcScorePart2()
        {
            var strCount = new Dictionary<string, long>();

            foreach(var key in compounds.Keys)
            {
                if(!strCount.ContainsKey(key.Substring(0,1)))
                    strCount.Add(key.Substring(0,1),0);

                strCount[key.Substring(0,1)] += compounds[key];
            }

            var lastLetter = formula.Last().ToString();
            if(!strCount.ContainsKey(lastLetter))
                strCount.Add(lastLetter,0);

            strCount[lastLetter] +=1;

            return strCount.Values.Max() - strCount.Values.Min();
        }


        private long CalcScore(string curr)
        {
            var max = long.MinValue;
            var min = long.MaxValue;

            foreach(var c in curr.Distinct().ToList())
            {
                if(curr.Count(x=> x== c) > max)
                    max = curr.Count(x=> x== c);
                if(curr.Count(x=> x== c) < min)
                    min = curr.Count(x=> x== c);
            }

            return max-min;
        }
    }    
}