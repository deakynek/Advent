using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    class Radiation
    {
        List<int> entries;

        public Radiation(List<string> lines)
        {
            var readings = lines.Select(x=>x.Trim()).ToList();
            Part1(readings);
            Part2(readings);
        }

        private void Part2(List<string> readings)
        {
            var ox = 0;
            var co2 = 0;

            //Find Ox
            var remaining = readings.ToList();
            var index = 0;
            while(remaining.Count() > 1)
            {
                var most = mostCommon(remaining, index);
                if(most == -1)
                    most = 1;

                remaining = remaining.Where(x => x[index].ToString()==most.ToString()).ToList();
                index++;
            }

            ox = convertBinaryString(remaining.First());

            //Find CO2
            remaining = readings.ToList();
            index = 0;
            while(remaining.Count() > 1)
            {
                var least = leastCommon(remaining, index);
                if(least == -1)
                    least = 0;

                remaining = remaining.Where(x => x[index].ToString()==least.ToString()).ToList();
                index++;
            }
            co2 = convertBinaryString(remaining.First());

            Console.WriteLine("\n");
            Console.WriteLine(String.Format("O2 - {0}  CO2 - {1}", ox, co2));
            Console.WriteLine(String.Format("Answer - {0}", co2*ox));

        }

        private int convertBinaryString(string num)
        {
            var ind = 0;
            var tot = 0;
            foreach(var i in num.Reverse())
            {
                if(i != '0')
                    tot += (int)Math.Pow(2, ind);
                ind++;
            }
            return tot;
        }

        private int mostCommon(List<string> readings, int index)
        {
            var tot = readings.Select(x=> int.Parse(x[index].ToString())).Sum();
            
            if(tot == readings.Count/2.0)
                return -1;
            if(tot > readings.Count/2.0)
                return 1;
            return 0;
        }

        private int leastCommon(List<string> readings, int index)
        {
            var tot = readings.Select(x=> int.Parse(x[index].ToString())).Sum();
            
            if(tot == readings.Count/2.0)
                return -1;
            if(tot> readings.Count/2.0)
                return 0;
            return 1;
        }

        private void Part1(List<string> readings)
        {
            var gamma = 0;
            var ep = 0;

            var readingLength = readings[0].Length;
            for(int i = readingLength-1; i >= 0; i-- )
            {
                var tot = readings.Select(x=> int.Parse(x[i].ToString())).Sum();

                if(tot > readings.Count/2)

                {
                    gamma += (int)Math.Pow(2, readingLength-1-i);
                }
                else
                {
                    ep += (int)Math.Pow(2, readingLength-1-i);
                }
            }

            Console.WriteLine(String.Format("gamma - {0} epsi - {1}", gamma,ep));
            Console.WriteLine(String.Format("Answer - {0}", gamma*ep));

        }
    }    
}