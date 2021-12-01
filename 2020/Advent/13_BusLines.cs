using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace Advent
{
    class BusLines
    {
        int earliestTime = 0;
        List<int> buses = new List<int>();
        List<string> busesString = new List<string>();
        public BusLines(List<string> lines)
        {
            busesString = lines[1].Split(',').ToList();
            earliestTime = Int32.Parse(lines[0]);
            buses = lines[1].Split(',').Where(n => n!= "x").Select(x => Int32.Parse(x)).ToList();

            var busTime = earliestTime;
            bool found = false;
            var foundBus = 0;

            while(!found)
            {
                foreach(var bus in buses)
                {
                    if(busTime%bus == 0)
                    {
                        foundBus = bus;
                        found = true;
                        break;
                    }
                }
                if(found)
                    break;

                busTime++;
            }

            
            Console.WriteLine("Part 1 - earliest time = " + busTime);
            Console.WriteLine("Part 1 - bus = " + foundBus);
            Console.WriteLine("Part 1 - ans = " + ((busTime-earliestTime)*foundBus));


            List<Tuple<int,int>> multiples = new List<Tuple<int,int>>();
            List<Tuple<int,int,int,int>> newMultiples = new List<Tuple<int, int,int,int>>();
            int maxMult = 0;
            int maxInd = 0;
            for(int i = 0; i< busesString.Count(); i++)
            {
                if(busesString[i] == "x")
                    continue;


                var thisMult = Int32.Parse(busesString[i]);
                newMultiples.Add(GetMultiple(Int32.Parse(busesString[0]), thisMult, i));

                if(thisMult>maxMult)
                {
                    maxMult = thisMult;
                    maxInd = i;
                }
                multiples.Add(new Tuple<int, int>(thisMult,i));
            }

            long lcm = newMultiples.First().Item1;
            long magicNumber = 0;
            Tuple<int,int,int,int> maxLCMMult = null;
            foreach(var thisMult in newMultiples)
            {
                if(thisMult == newMultiples.First())
                    continue;

                var prevLCM = lcm;
                lcm = MoreMath.getLCM((long)thisMult.Item1, lcm);

                long num = 0;
                while(num < lcm)
                {
                    if((num+magicNumber+thisMult.Item2)%thisMult.Item1 == 0)
                    {
                        magicNumber += num;
                        break;
                    }

                    num += prevLCM;
                }
            }



            Console.WriteLine("Earliest Timestamp = " + (magicNumber));

        }

        

        private Tuple<int,int,int,int> GetMultiple(int first, int second, int space)
        {
            int firstmult = 0;
            int secondmult = 0;

            int mult = 1;
            while(secondmult == 0)
            {
                if((mult*first+space)%second == 0)
                {
                    if(firstmult == 0)
                        firstmult = mult;
                    else if(secondmult == 0)
                    {
                        secondmult = mult;
                    }
                }
                mult++;
            }

            return new Tuple<int, int,int,int>(second,space,firstmult,(secondmult-firstmult));

        }

    }

}