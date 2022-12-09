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
            var HPos = new List<int>();
            HPos.Add(0);
            HPos.Add(0);
            var TPos = new List<int>();
            TPos.Add(0);
            TPos.Add(0);
            THist.Add(0, new List<int>());

            foreach (var command in input)
            {
                var parsed = command.Split(' ');
                var ammount = int.Parse(parsed[1]);

                for (int i = 0; i < ammount; i++)
                {
                    if (parsed[0] == "U")
                        HPos[0] += 1;
                    else if (parsed[0] == "D")
                        HPos[0] -= 1;
                    else if (parsed[0] == "R")
                        HPos[1] += 1;
                    else if (parsed[0] == "L")
                        HPos[1] -= 1;

                    if (Math.Abs(TPos[0] - HPos[0]) > 1 ||
                        Math.Abs(TPos[1] - HPos[1]) > 1)
                    {
                        if (Math.Abs(TPos[0] - HPos[0]) < Math.Abs(TPos[1] - HPos[1]))
                        {
                            TPos[0] = HPos[0];
                            if (parsed[0] == "R")
                                TPos[1] = HPos[1] - 1;
                            else if (parsed[0] == "L")
                                TPos[1] = HPos[1] + 1;
                        }
                        else
                        {
                            TPos[1] = HPos[1];
                            if (parsed[0] == "U")
                                TPos[0] = HPos[0] - 1;
                            else if (parsed[0] == "D")
                                TPos[0] = HPos[0] + 1;
                        }
                    }

                    if (!THist.ContainsKey(TPos[0]))
                        THist.Add(TPos[0], new List<int>());
                    if (!THist[TPos[0]].Contains(TPos[1]))
                        THist[TPos[0]].Add(TPos[1]);
                }
            }
            var sum = THist.Select(x => x.Value.Count()).Sum();
            Console.WriteLine(String.Format("Part 1: Distinct Tail Pos Count = {0}", sum));


            THist.Clear();
            THist.Add(0, new List<int>());
            List<List<int>> allKnots = new List<List<int>>();
            for (int i = 0; i < 10; i++)
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

            sum = THist.Select(x => x.Value.Count()).Sum();
            Console.WriteLine(String.Format("Part 2: Distinct Tail Pos Count = {0}", sum));


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
