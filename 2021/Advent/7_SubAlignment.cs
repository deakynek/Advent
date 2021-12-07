using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    class SubAlignment
    {
        List<int> entries;

        public SubAlignment(List<string> lines)
        {
            entries= lines[0].Split(',').Select(x => int.Parse(x)).ToList();

            var minFuel = int.MaxValue;
            for(int i = entries.Min(); i<= entries.Max(); i++)
            {
                var thisFuel = entries.Select(x => Math.Abs(x-i)).Sum();

                if(thisFuel< minFuel)
                    minFuel = thisFuel;
            }

            Console.WriteLine(String.Format("Part 1 Minimum fuel to alignment {0}",minFuel));


            minFuel = int.MaxValue;
            for(int i = entries.Min(); i<= entries.Max(); i++)
            {
                var thisFuel = entries.Select(x => Math.Abs(x-i)*(Math.Abs(x-i)+1)/2).Sum();

                if(thisFuel< minFuel)
                    minFuel = thisFuel;
            }

            Console.WriteLine(String.Format("Part 2 Minimum fuel to alignment {0}",minFuel));            
        }

        
    }    
}