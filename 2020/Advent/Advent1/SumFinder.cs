using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    class SumFinder
    {
        List<int> entries;

        public SumFinder(List<string> lines)
        {
            entries = lines.Select(line => Int32.Parse(line)).ToList<int>();
        }

        public SumFinder(List<int> entries)
        {
            this.entries = entries;
        }

        public List<int> FindSumWithNNumbers(int sum, int n)
        {
            if(entries == null || !entries.Any())
                return null;

            var numbers = RecurseFindSum(sum, n, 0, 0);
            return numbers;
        }

        private List<int> RecurseFindSum(int remainingSum,  int n, int startingIndex, int currentDepth)
        {
            if(remainingSum <= 0)
                return null;

            var index = 0;
            foreach(int entry in entries.Skip(startingIndex))
            {
                if(currentDepth != n-1)
                {
                    var depthSum = RecurseFindSum(remainingSum-entry, n, startingIndex+index+1, currentDepth+1);
                    if(depthSum ==null )
                    {
                        index++;
                        continue;
                    }
                    else
                    {
                        depthSum.Add(entry);
                        return depthSum;
                    }
                }
                else
                {
                    if(entry == remainingSum)
                    {
                        return new List<int>(){entry};
                    }
                }

                index++;
            }

            return null;
        }

        public void PrintMultOfNNumbersSummingToValue(string opText, int sum, int n)
        {
            var numberList = FindSumWithNNumbers(sum, n);

            if(numberList == null)
            {
                Console.WriteLine(String.Format("{0}: Could not find {1} numbers in file that would sum to {2}",opText,n, sum));
                return;            
            }

            Int64 mult = 1;
            numberList.ForEach(num => mult *= num);
            var output = opText +": "+ mult.ToString() +" =\t\t" + string.Join(" * ", numberList);
            Console.WriteLine(output);
        }

    }    
}