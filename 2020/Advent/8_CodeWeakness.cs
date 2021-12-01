using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace Advent
{
    class CodeWeakness
    {
        List<long> instructions = new List<long>();

        public CodeWeakness(List<string> lines)
        {
            instructions = lines.Select(x => Int64.Parse(x)).ToList();

            var inv = TestAll();

            var sumFound = false;
            var n = 2;
            var sum = new List<long>();
            var index = instructions.IndexOf(inv);
    

            while(!sumFound)
            {
                if(n % 1000000 == 0)
                {
                    Console.WriteLine("Trying to Find " +  n.ToString() + " numbers from " + inv.ToString());
                }

                sum = FindSum(index, n, inv);
                if(sum != null)
                {
                    sumFound = true;
                    break;
                }
                n++;
            }

            var result = sum.Min() +sum.Max();
            foreach(var num in sum)
            {
                Console.WriteLine(num.ToString());
            }

            Console.WriteLine(inv.ToString());
            Console.WriteLine("Part 2 - " + result.ToString());
        }

        private bool TestValid(int preamble, long value,int index)
        {
            for(int i = index-1; i>index-preamble-1; i--)
            {
                for(int j = i-1; j> index-preamble-1; j--)
                {
                    if(instructions[i] + instructions[j] == value)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private long TestAll()
        {
            int preamble = 25;
            for(int i = preamble; i<instructions.Count(); i++)
            {
                if(!TestValid(preamble, instructions[i], i))
                {
                    Console.WriteLine("Part 1 " + instructions[i].ToString());
                    return instructions[i];
                }
            }
            return 0;
        }

        private List<long> FindSum(int index, int nNumbers, long value)
        {
            for(int i = 0; i<index-nNumbers; i++)
            {
                if(instructions.Skip(i).Take(nNumbers).Sum() == value)
                {
                    return instructions.Skip(i).Take(nNumbers).ToList();
                }
            }

            return null;

        }
    }
}