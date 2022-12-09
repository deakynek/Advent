using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent
{
    internal class Ropes
    {
        Dictionary<int, List<int>> THist = new Dictionary<int, List<int>>();

        public Ropes(List<string> input)
        {
            RopeTrick(input, 2, "Part 1");
            RopeTrick(input, 10, "Part 2");
        }

        private void RopeTrick(List<string>input,int knotsCount, string logheader)
        {
            THist.Clear();
            THist.Add(0, new List<int>());
            List<List<int>> allKnots = new List<List<int>>();
            for (int i = 0; i < knotsCount; i++)
            {
                allKnots.Add(new List<int>() { 0, 0 });
            }

            foreach (var command in input)
            {
                var parsed = command.Split(' ');
                var ammount = int.Parse(parsed[1]);

                for (int i = 0; i < ammount; i++)
                {
                    if (parsed[0] == "U")
                        allKnots[0][0] += 1;
                    else if (parsed[0] == "D")
                        allKnots[0][0] -= 1;
                    else if (parsed[0] == "R")
                        allKnots[0][1] += 1;
                    else if (parsed[0] == "L")
                        allKnots[0][1] -= 1;


                    for (int j = 1; j < allKnots.Count(); j++)
                    {
                        allKnots[j] = GetNewPos(allKnots[j - 1], allKnots[j]);
                    }

                    if (!THist.ContainsKey(allKnots.Last()[0]))
                        THist.Add(allKnots.Last()[0], new List<int>());
                    if (!THist[allKnots.Last()[0]].Contains(allKnots.Last()[1]))
                        THist[allKnots.Last()[0]].Add(allKnots.Last()[1]);
                }
            }

            var sum = THist.Select(x => x.Value.Count()).Sum();
            Console.WriteLine(String.Format("Part {0}: Distinct Tail Pos Count = {1}",logheader, sum));
        }

        private List<int> GetNewPos(List<int> hknot, List<int> tknot)
        {
            if (Math.Abs(hknot[0] - tknot[0]) <= 1 &&
                Math.Abs(hknot[1] - tknot[1]) <= 1)
            {
                return tknot;
            }

            if (Math.Abs(hknot[0] - tknot[0]) > Math.Abs(hknot[1] - tknot[1]))
            {
                tknot[1] = hknot[1];
                if (hknot[0] > tknot[0])
                    tknot[0]++;
                else
                    tknot[0]--;
            }
            else if (Math.Abs(hknot[0] - tknot[0]) < Math.Abs(hknot[1] - tknot[1]))
            {
                tknot[0] = hknot[0];
                if (hknot[1] > tknot[1])
                    tknot[1]++;
                else
                    tknot[1]--;
            }
            else
            {
                if (hknot[0] > tknot[0])
                    tknot[0]++;
                else
                    tknot[0]--;

                if (hknot[1] > tknot[1])
                    tknot[1]++;
                else
                    tknot[1]--;
            }
            return tknot;
        }
    }
}
