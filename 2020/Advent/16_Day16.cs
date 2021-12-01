using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace Advent
{
    class Day16
    {
        List<int> myTicket = new List<int>();
        List<List<int>> NearbyTickets = new List<List<int>>();
        Dictionary<string, List<int>> TicketRules = new Dictionary<string, List<int>>();

        public Day16(List<string> lines)
        {
            bool rulesDone = false;
            bool myTicketDone = false;
            foreach(var line in lines)
            {
                if(String.IsNullOrEmpty(line))
                    continue;

                if(line == "your ticket:")
                {
                    rulesDone = true;
                    continue;
                }

                if(!rulesDone)
                {
                    var name = line.Split(':')[0];
                    var ranges = line.Split(':')[1];

                    var numbs = ranges.Split(' ','-');
                    var min1 = Int32.Parse(numbs[1]);
                    var max1 = Int32.Parse(numbs[2]);
                    var min2 = Int32.Parse(numbs[4]);
                    var max2 = Int32.Parse(numbs[5]);

                    TicketRules.Add(name,new List<int>(){min1,max1,min2, max2});
                    continue;
                }

                if(line == "nearby tickets:")
                {
                    myTicketDone = true;
                    continue;
                }

                if(!myTicketDone)
                {
                    myTicket = line.Split(',').Select(x => Int32.Parse(x)).ToList();
                    continue;
                }

                var thisTicket = line.Split(',').Select(x => Int32.Parse(x)).ToList();
                NearbyTickets.Add(thisTicket);
            }

            Console.WriteLine("Starting: " + NearbyTickets.Count());
            CountInvalidTickets();
            FindTicketValues();
        }

        public void CountInvalidTickets()
        {
            var invalidValueSum = 0;

            foreach(var nearBy in NearbyTickets.ToList())
            {
                bool anyInvalid = false;
                foreach(var numb in nearBy)
                {
                    if(!TicketRules.Values.Any(y => (numb>=y[0] && numb<= y[1]) || (numb>=y[2] && numb<=y[3])))
                    {
                        anyInvalid = true;
                        invalidValueSum += numb;
                    }
                }
                if(anyInvalid)
                    NearbyTickets.Remove(nearBy);
            }

            Console.WriteLine("Totally invalid Tickets: " + invalidValueSum);
            Console.WriteLine("Remaining: " + NearbyTickets.Count());
        }

        private void FindTicketValues()
        {
            var Possibles = new List<List<string>>();
            myTicket.ForEach(x => Possibles.Add(TicketRules.Keys.ToList()));

            var dict = new Dictionary<string, int>();
            while(dict.Count()< Possibles.Count())
            {
                for(int i = 0; i<Possibles.Count(); i++)
                {
                    var category = NearbyTickets.Select(x => x[i]).ToList();
                    foreach(var rule in TicketRules)
                    {
                        if(!Possibles[i].Contains(rule.Key))
                            continue;

                        if(!category.All(y => (y>=rule.Value[0] && y<= rule.Value[1]) || (y>=rule.Value[2] && y<=rule.Value[3])))
                        {
                            Possibles[i].Remove(rule.Key);
                        }
                    }
                }

                Possibles = CheckPossibles(Possibles);

                for(int i = 0; i<Possibles.Count(); i++)
                {
                    if(Possibles[i].Count() > 1)
                        continue;
                    if(!dict.ContainsKey(Possibles[i][0]))
                    {
                        dict.Add(Possibles[i][0],i);
                    }
                }
            }

            long departureMult = 1;
            foreach(var category in dict.Where(x => x.Key.Contains("departure")).ToList())
            {
                departureMult = departureMult*myTicket[category.Value];
            }

            Console.WriteLine("Part 2 - departure Categories mult : " + departureMult);
        }

        private List<List<string>> CheckPossibles(List<List<string>> Possibles)
        {
            if(!Possibles.Any(x => x.Count() == 1))
                return Possibles;

            bool noneChanged = true;
            do
            {
                var solos = Possibles.Where(x => x.Count() == 1).Select(x => x[0]).ToList();
                var copy = Possibles.ToList();
                
                noneChanged = true;
                for(int i = 0; i<copy.Count(); i++)
                {
                    if(copy[i].Count() <=1)
                        continue;

                    foreach(var solo in solos)
                    {
                        if(Possibles[i].Contains(solo))
                        {
                            noneChanged = false;
                            Possibles[i].Remove(solo);
                        }
                    }
                }
            }
            while(!noneChanged);

            return Possibles;
        }
    }
}