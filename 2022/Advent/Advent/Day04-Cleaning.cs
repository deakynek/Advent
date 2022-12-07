using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace Advent
{
    internal class Cleaning
    {
        public Cleaning(List<string> inputs)
        {
            var totOverlap1 = 0;
            var totOverlap2 = 0;
            char[] delimiters = new char[] { ',', '-' };

            foreach (var line in inputs)
            {
                var parsed = line.Split(delimiters);

                var aMin = Int16.Parse(parsed.ElementAt(0));
                var aMax = Int16.Parse(parsed.ElementAt(1));
                var bMin = Int16.Parse(parsed.ElementAt(2));
                var bMax = Int16.Parse(parsed.ElementAt(3));

                if(aMax-aMin>=bMax-bMin)
                    totOverlap1 += bMin>=aMin && bMax<= aMax ? 1 :0;
                
                else
                    totOverlap1 += aMin >= bMin && aMax <=bMax ? 1 :0;

                if(aMin<= bMin)
                    totOverlap2 += bMin>=aMin && bMin<=aMax ? 1 : 0;
                else
                    totOverlap2 += aMin>=bMin && aMin<=bMax ? 1 : 0;

            }
            Console.WriteLine(String.Format("Part 1: {0}",totOverlap1));
            Console.WriteLine(String.Format("Part 2: {0}",totOverlap2));


        }
    }
}