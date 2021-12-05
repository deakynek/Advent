using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

namespace Advent
{
    public class ElfGame
    {

        Dictionary<int, List<int>> numbers = new Dictionary<int, List<int>>();
        public ElfGame(List<string> inputs)
        {
            var initial = inputs[0].Split(',');

            var initialList = initial.Select(x => Int32.Parse(x)).ToList();
            for(int i =0; i<initialList.Count(); i++)
            {
                var num = initialList[i];
                if(!numbers.ContainsKey(num))
                    numbers.Add(num, new List<int>(){i+1});
                else
                    numbers[num].Add(i+1);
                
            }

            PlayGame(2020);
            PlayGame(30000000);
        }

        private void PlayGame(long endingTurn)
        {
            var lastTurn = numbers.Values.Select(v => v.Count()).Sum();

            int nextTurn = lastTurn+1;
            var lastNumberAdded = numbers.Keys.Last();
            while(nextTurn <= endingTurn)
            {
                if(!numbers.ContainsKey(lastNumberAdded)|| numbers[lastNumberAdded].Count()<1)
                {
                    if(!numbers.ContainsKey(0))
                    {
                        numbers.Add(0, new List<int>(){nextTurn});
                    }
                    else
                    {
                        numbers[0].Add(nextTurn);
                    }
                    lastNumberAdded = 0;
                }
                else
                {
                    var prevIndexes = numbers[lastNumberAdded].TakeLast(2);
                    var numberSpoken = prevIndexes.Last()-prevIndexes.First();
                    if(!numbers.ContainsKey(numberSpoken))
                    {
                        numbers.Add(numberSpoken, new List<int>(){nextTurn});
                    }
                    else
                    {
                        numbers[numberSpoken].Add(nextTurn);
                    }

                    lastNumberAdded = numberSpoken;
                }

                nextTurn++;
            }

            Console.WriteLine("2020th number spoken: "+lastNumberAdded);
        }
    }
}