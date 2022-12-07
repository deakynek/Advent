using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent
{
    internal class Radios
    {
        public Radios(List<string> input) 
        {
            var listChar = input[0].ToCharArray().ToList();

            var part1Found = false;
            for (int i = 0; i < listChar.Count; i++)
            {
                if (!part1Found && listChar.Skip(i).Take(4).Distinct().Count() == 4)
                {
                    Console.WriteLine(String.Format("Part 1: processed {0} chars", i + 4));
                    part1Found = true;
                }
                if (listChar.Skip(i).Take(14).Distinct().Count() == 14)
                {
                    Console.WriteLine(String.Format("Part 2: processed {0} chars", i + 14));
                    break;
                }
            }
        }
     
    }
}
