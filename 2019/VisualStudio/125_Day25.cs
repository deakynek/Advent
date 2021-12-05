using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

namespace Advent
{
    public class Day25
    {
        List<long> loopCounts = new List<long>();
        List<long> publicKeys = new List<long>();
        public Day25(List<string> inputs)
        {
            foreach(var input in inputs)
            {
                publicKeys.Add(Int64.Parse(input));
                loopCounts.Add(GetLoopCount(publicKeys.Last()));
            }

            Console.WriteLine("Part 1: Expected verification: " + TrasformSubjectNumber(publicKeys[0],loopCounts[1]));

        }

        public long GetLoopCount(long privateKey)
        {
            long currentVal = 1;
            var subjectNumber = 7;
            var loopCount =0;
            var mod = 20201227;

            while(currentVal != privateKey)
            {
                currentVal = (currentVal*subjectNumber)%mod;
                loopCount++;
            }

            return loopCount;
        }

        public long TrasformSubjectNumber(long subject, long loopCount)
        {
            long currentVal = 1;
            var mod = 20201227;
            long thisLoopCount = 0;

            while(thisLoopCount < loopCount)
            {
                currentVal = (currentVal*subject)%mod;
                thisLoopCount++;
            }

            return currentVal;

        }
    }
}