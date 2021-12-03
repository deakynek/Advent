using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    class Sonar
    {
        List<int> entries;

        public Sonar(List<string> lines)
        {
            entries = lines.Select(line => Int32.Parse(line)).ToList<int>();
            getIncreases();
            getIncreasesSlideWindow();
        }

        private void getIncreases()
        {
            int increases = 0;
            for(int i= 1; i<entries.Count(); i++)
            {
                if(entries[i] > entries[i-1])
                    increases++;
            }

            Console.WriteLine(String.Format("Increases {0}", increases));
        }

        private void getIncreasesSlideWindow()
        {
            int increases = 0;
            for(int i= 1; i<entries.Count()-2; i++)
            {
                if(getSlidingWindowSum(i)> getSlidingWindowSum(i-1))
                    increases++;
            }

            Console.WriteLine(String.Format("Increases with slide window {0}", increases));
        }

        private int getSlidingWindowSum(int i)
        {
            return entries.GetRange(i,3).Sum();
        }
    }    
}