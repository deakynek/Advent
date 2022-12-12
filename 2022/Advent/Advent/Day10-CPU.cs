using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent
{
    internal class CPU
    {
        Dictionary<int, int> val = new Dictionary<int, int>();
        public CPU(List<string> inputs) 
        {
            var cyc = 0;
            val[0]= 1;
            //Calc Value at every cycle
            foreach(var line in inputs)
            {
                cyc += 1;
                if (line != "noop")
                {
                    var parsed = line.Split(' ');
                    var ammount = int.Parse(parsed[1]);

                    for(int i = 0; i<3; i++) 
                    { 
                        if(!val.ContainsKey(cyc+i))
                        {
                            if (i == 2)
                                val[cyc + i] = val[cyc+i-1] +ammount;
                            else
                                val[cyc + i] = val[cyc+i-1];
                        }
                    }
                    cyc++;
                }
                else if (!val.ContainsKey(cyc))
                {
                    val[cyc] = val[cyc - 1];
                }
            }

            //Part 1 Sum certain indexes
            var listInd = new List<int>() {20,60,100,140,180,220};
            var sum = 0;
            foreach(var ind in listInd)
            {
                sum += val[ind]*ind;
            }
            Console.WriteLine(String.Format("Part 1: sum of values = {0}",sum));

            //Part 2 Write out each line of CRT Screen
            Console.WriteLine(String.Format("Part 2"));
            string x = "";
            for(int i = 0; i<240;i++)
            {
                if (Math.Abs(i%40 - val[i+1]) <= 1)
                    x += "0";
                else
                    x += " ";

                if((i+1)%40 == 0)
                {
                    Console.WriteLine(x);
                    x = "";
                }
            }
        }
    }
}
