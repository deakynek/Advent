using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    class GrowthRate
    {
        List<int> entries;

        public GrowthRate(List<string> lines)
        {
            entries = lines[0].Trim().Split(',').Select(x => int.Parse(x)).ToList();

            long tot = 0;
            foreach(var fish in entries)
            {
                tot += GetFish(fish, 80,7);
            }
            Console.WriteLine(String.Format("{0} fish after {1} days",tot,80));

            
            
            tot = 0;
            foreach(var fish in entries)
            {
                tot += GetFish(fish, 256,7);
            }
            Console.WriteLine(String.Format("{0} fish after {1} days",tot,256));

        }

        private long GetFish(int start, int days, int gest)
        {
            days -= start;

            long tot = 1;
            if(days == 0)
                return tot;

            var triLevels = (days-1)/gest + 1;

            for(int i = 1; i<=triLevels; i++)
            {
                var values = GetTriLevel(i);
                var currDay = gest*(i-1);
                var adder = 0;

                foreach(var val in values)
                {
                    if(currDay+adder <= (days-1))
                    {
                        tot += val;
                    }
                    else
                    {
                        break;
                    }
                    adder+=2;
                }
            }

            return tot;
        }

        private List<int> GetTriLevel(int level)
        {
            List<int> triLevel = new List<int>();
            triLevel.Add(1);

            var currLevel = 1;
            while(currLevel < level)
            {
                var temp = new List<int>();
                for(int i = 1; i<triLevel.Count(); i++)
                {
                    temp.Add(triLevel[i]+triLevel[i-1]);
                }

                temp.Insert(0,1);
                temp.Add(1);
                triLevel = temp.ToList();

                currLevel++;
            }
            return triLevel;
        }
    }    
}