using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace Advent
{
    class Customs
    {
        List<string> rawInput = new List<string>();
        Dictionary<string, List<Tuple<int,string>>> bagLaws = new Dictionary<string, List<Tuple<int,string>>> ();

        public Customs(List<string> lines)
        {
            foreach(var line in lines)
            {
                var split = " contain";
                var bag =  line.Substring(0,line.IndexOf(split));
                var rules = line.Substring(line.IndexOf(split)+split.Count());
                if(rules == " no other bags.")
                {
                    bagLaws.Add(bag, new List<Tuple<int,string>>());
                    continue;
                }

                var allComponents = new List<Tuple<int,string>>();
                var bagComponents = rules.Split((",.").ToArray());
                foreach(var comp in bagComponents)
                {
                    if(comp =="")
                        continue;

                    var bagName = comp.Substring(2).Trim();
                    if(!bagName.EndsWith("s"))
                        bagName += "s";
                    allComponents.Add(new Tuple<int,string>(Int32.Parse(comp[1].ToString()),bagName));
                }

                bagLaws.Add(bag, allComponents);
            }

            GetBagCount();
            GoldBagsReq();
        }

        public void GetBagCount()
        {
            var count = 0;
            foreach(var bagType in bagLaws.Keys)
            {
                if(RecurseLookForGold(bagType))
                    count++;
            }

            PrintPart1(count.ToString());
        }

        public void GoldBagsReq()
        {
            PrintPart2((GetNumberOfBagsRequired("shiny gold bags")-1).ToString());
        }

        public long GetNumberOfBagsRequired(string key)
        {
            if(bagLaws[key].Count() == 0)
                return 1;

            long runningTotal = 0;
            foreach(var lowerBag in bagLaws[key])
            {
                runningTotal += lowerBag.Item1 * GetNumberOfBagsRequired(lowerBag.Item2);
            }
            return runningTotal+1;
        }

        private bool RecurseLookForGold(string key)
        {
            foreach(var lowerBag in bagLaws[key].Select(x => x.Item2))
            {
                if(lowerBag.Contains("shiny gold") || RecurseLookForGold(lowerBag))
                    return true;
            }

            return false;
        }

        public void PrintPart1(string ans)
        {
            Console.WriteLine("Part 1: " +ans);
        }

        public void PrintPart2(string ans)
        {
            Console.WriteLine("Part 2: " +ans);
        }

    }
}