using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent
{
    internal class CraneOps
    {
        List<List<string>> craneState1 = new List<List<string>>();
        List<List<string>> craneState2 = new List<List<string>>();

        public CraneOps(List<String> inputs)
        {
            var numCranes = (int)(inputs[0].Length / 4)+1;
            for (int i = 0; i < numCranes; i++)
            {
                craneState1.Add(new List<string>());
                craneState2.Add(new List<string>());
            }

            bool init = true;
            foreach (var line in inputs)
            {
                if (String.IsNullOrEmpty(line))
                {
                    continue;
                }
                if(line[1] == '1')
                {
                    init = false;
                    foreach (var c in craneState1)
                    {
                        c.Reverse();
                    }

                    foreach (var c in craneState2)
                    {
                        c.Reverse();
                    }
                    continue;
                }

                if (init)
                {
                    for (int i = 0; i < numCranes; i++)
                    {
                        if (line.Substring(4 * i + 1, 1) !=" ")
                        {
                            craneState1.ElementAt(i).Add(line.Substring(4 * i + 1, 1));
                            craneState2.ElementAt(i).Add(line.Substring(4 * i + 1, 1));
                        }
                    }
                }
                else
                {
                    var parseOp = line.Split(' ');
                    var num = int.Parse(parseOp[1]);
                    var oldInd = int.Parse(parseOp[3])-1;
                    var newInd = int.Parse(parseOp[5])-1;

                    var taken = craneState1[oldInd].ToList();
                    if (num < taken.Count)
                    {
                        taken = craneState1[oldInd].Skip(taken.Count - num).ToList();
                        craneState1[oldInd] = craneState1[oldInd].Take(craneState1[oldInd].Count - num).ToList();
                    }
                    else
                    {
                        craneState1[oldInd].Clear();
                    }

                    taken.Reverse();
                    craneState1[newInd].AddRange(taken);


                    var taken2 = craneState2[oldInd].ToList();
                    if (num < taken2.Count)
                    {
                        taken2 = craneState2[oldInd].Skip(taken2.Count - num).ToList();
                        craneState2[oldInd] = craneState2[oldInd].Take(craneState2[oldInd].Count - num).ToList();
                    }
                    else
                    {
                        craneState2[oldInd].Clear();
                    }

                    
                    craneState2[newInd].AddRange(taken2);
                }
            }

            var ret = "";
            for(int i = 0; i<craneState1.Count; i++) 
            { 
                ret+= craneState1[i].Last().ToString();
            }
            Console.WriteLine(String.Format("Part 1: top of piles {0}",ret));

            ret = "";
            for (int i = 0; i < craneState2.Count; i++)
            {
                ret += craneState2[i].Last().ToString();
            }
            Console.WriteLine(String.Format("Part 2: top of piles {0}", ret));

        }
    }
}
