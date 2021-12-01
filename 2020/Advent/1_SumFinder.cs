using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    class SumFinder
    {
        List<long> entries;

        public SumFinder(List<string> lines)
        {
            entries = lines.Select(line => Int64.Parse(line)).ToList();
        }

        public SumFinder(List<long> entries)
        {
            this.entries = entries;
        }

        public List<long> FindSumWithNNumbers(long sum, int n)
        {
            if(entries == null || !entries.Any())
                return null;

            var numbers = RecurseFindSum(sum, n, 0, 0);
            return numbers;
        }

        private List<long> RecurseFindSum(long remainingSum,  int n, int startingIndex, int currentDepth)
        {
            if(remainingSum <= 0)
                return null;

            var index = 0;
            foreach(long entry in entries.Skip(startingIndex))
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
                        return new List<long>(){entry};
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