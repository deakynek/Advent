using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Advent
{
    internal class MonkeyInTheMiddle
    {
        Dictionary<int, List<long>> monkeyCarries = new Dictionary<int, List<long>>(); 
        Dictionary<int, List<long>> monkeyCarries2 = new Dictionary<int, List<long>>();
        Dictionary<int, string> monkeyOperation = new Dictionary<int, string>();
        Dictionary<int, int> monkeyTest = new Dictionary<int, int>();
        Dictionary<int, List<int>> monkeyLoc = new Dictionary<int, List<int>>();
        Dictionary<int, long> monkeyInspectionCount = new Dictionary<int, long>();

        public MonkeyInTheMiddle(List<string> input)
        {
            for (int i = 0; i < input.Count; i++)
            {
                var monkeyNum = int.Parse(input[i][7].ToString());
                i++;
                var itemsList = input[i].Trim().Split(':').ElementAt(1).Replace(" ", "").Split(',')
                                        .Select(x => long.Parse(x))
                                        .ToList();
                i++;
                var op = input[i].Trim().Split("=").ElementAt(1);
                i++;
                var test = int.Parse(input[i].Trim().Split(' ').Last());
                i++;
                var ifTrue = int.Parse(input[i].Trim().Split(' ').Last());
                i++;
                var ifFalse = int.Parse(input[i].Trim().Split(' ').Last());
                i++;

                monkeyCarries[monkeyNum] = itemsList;
                monkeyCarries2[monkeyNum] = itemsList.ToList();
                monkeyOperation[monkeyNum] = op;
                monkeyTest[monkeyNum] = test;
                monkeyLoc[monkeyNum] = new List<int>() { ifTrue, ifFalse };
                monkeyInspectionCount[monkeyNum] = 0;
            }


            CalcRounds(20, 1,monkeyCarries, "Part 1"); 
            foreach(var item in monkeyInspectionCount)
            {
                monkeyInspectionCount[item.Key] = 0;
            }
            CalcRounds(10000, Lcm(monkeyTest.Values.ToList()), monkeyCarries2, "Part 2");
        }

        private void CalcRounds(int roundCount, int part, Dictionary<int, List<long>> carries,string header)
        { 
            for(int round = 1; round<= roundCount; round++) 
            { 
                foreach(var monk in carries.Keys)
                {
                    foreach(var item in carries[monk])
                    {
                        
                        var newItem = eval(monkeyOperation[monk].Replace("old", item.ToString()));
                        if (part == 1)
                            newItem = (int)(newItem / 3);
                        else
                        {
                            if (newItem < 0)
                                newItem += part;
                            newItem = (int)(newItem % part);
                        }

                        if(newItem % monkeyTest[monk] == 0)
                            carries[monkeyLoc[monk][0]].Add(newItem);
                        else
                            carries[monkeyLoc[monk][1]].Add(newItem);

                        monkeyInspectionCount[monk]++;
                    }

                    carries[monk].Clear();
                }
            }

            var sortedInspections = monkeyInspectionCount.Values.OrderByDescending(X => X).ToList();
            var mult = sortedInspections[0] * sortedInspections[1];
            Console.WriteLine(String.Format(header + ": mult of two highest inspections: {0}", mult));
        }

        private long eval(string op)
        {
            op = op.Trim();
            var splitOp = op.Split(' ');
            long ret = 0;
            if (splitOp[1] == "+")
                ret =  long.Parse(splitOp[0]) + long.Parse(splitOp[2]);
            else if (splitOp[1] == "-")
                ret =  long.Parse(splitOp[0]) - long.Parse(splitOp[2]);
            else if (splitOp[1] == "*")
                ret =  long.Parse(splitOp[0]) * long.Parse(splitOp[2]);
            else if (splitOp[1] == "/")
                ret = long.Parse(splitOp[0]) / long.Parse(splitOp[2]);

            return ret;
        }



        static int Gcf(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        static int Lcm(int a, int b)
        {
            return (a / Gcf(a, b)) * b;
        }

        static int Lcm(List<int> val)
        {
            var ret = 1;
            for(int i =0; i < val.Count; i++) 
            {
                ret = Lcm(ret, val[i]);
            }
            return ret;
        }
    }
}
