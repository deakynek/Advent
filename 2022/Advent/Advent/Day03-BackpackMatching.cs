using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace Advent
{
    internal class Packing
    {

        public Packing(List<string> inputs)
        {
            var tot = 0;
            var lowera = (int) 'a';
            var upperA = (int) 'A';


            foreach(var line in inputs)
            {
                var part1 = line.Substring(0,line.Length/2);
                var part2 = line.Substring(line.Length/2);

                foreach(var supply in part1.Distinct())
                {
                    if(!part2.Contains(supply))
                        continue;

                    if(supply.ToString().ToUpper() == supply.ToString())
                    {
                        tot += (int)supply - upperA + 27;
                    }
                    else
                    {
                        tot += (int)supply - lowera + 1;
                    }
                    break;
                }

            }

            Console.WriteLine(String.Format("Part 1 {0}",tot));

            tot = 0;
            for(int i = 0; i<inputs.Count; i+=3)
            {
                var first = inputs[i].Distinct();
                var second = inputs[i + 1].Distinct();
                var third = inputs[i + 2].Distinct();

                var same = first.Distinct().Where(x => second.Contains(x) &&
                                                       third.Contains(x))
                                           .First();


                if (same.ToString().ToUpper() == same.ToString())
                {
                    tot += (int)same - upperA + 27;
                }
                else
                {
                    tot += (int)same - lowera + 1;
                }
            }

            Console.WriteLine(String.Format("Part 2 {0}", tot));
        }
    }
}