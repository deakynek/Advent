using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

namespace Advent
{
    public class Day23
    {
        public Day23(List<string> inputs)
        {
            List<int> cups = new List<int>();
            foreach(var cupLabel in inputs[0])
            {
                cups.Add(Int32.Parse(cupLabel.ToString()));
            }

            var part1 = RotateCups(cups.ToList(), 100);
            Console.WriteLine("Part 1: " + String.Join("",part1));

            for(int i = 10; i<=1000000; i++)
            {
                cups.Add(i);
            }

            var part2 = RotateCups(cups.ToList(),10000000);
            var index = part2.IndexOf(1);

            Console.WriteLine("Part 2:");
            Console.WriteLine("1st after 1 : "+ cups[index+1]);
            Console.WriteLine("2nd after 1 : "+ cups[index+2]);
            
            Console.WriteLine("\nmult: "+ ((long)cups[index+1] * (long)cups[index+2]));


        }

        public List<int> RotateCups(List<int> cups, int iterations)
        {
            var iterCount = 0;
            var currentIndex = 0;
            var maxCups = cups.Max();
            var cupsCount = cups.Count();
            var movedCups = new List<int>();
            var movedCupsIndex = new List<int>();

            while(iterCount<iterations)
            {
                //var currentIndex = iterCount%cups.Count();
                movedCupsIndex.Clear();
                movedCups.Clear();

                var currentIndexLabel = cups[currentIndex];
                for(int i=1; i<4; i++)
                {
                    var index = (currentIndex+i+cupsCount)%cupsCount;
                    movedCupsIndex.Add(index);
                    movedCups.Add(cups[index]);
                }

                var placementValue = currentIndexLabel;
                do
                {
                    placementValue -= 1;
                    if(placementValue < 1)
                        placementValue = maxCups;
                }
                while(movedCupsIndex.Any(x => cups[x]== placementValue) || placementValue == 0);

                
                var UpperCount = movedCupsIndex.Where(X => X>currentIndex).Count();
                var Lowercount = movedCups.Count()-UpperCount;
                
                if(UpperCount>0)
                {
                    cups.RemoveRange(movedCupsIndex[0],UpperCount);
                }
                if(Lowercount>0)
                {
                    cups.RemoveRange(0,Lowercount);
                }
                cups.InsertRange(cups.IndexOf(placementValue)+1,movedCups);

                // List<int> newCups = new List<int>();
                // var startingIndex = (cups.IndexOf(currentIndexLabel)-currentIndex+cups.Count())%cups.Count();
                // for(int i = 0; i< cups.Count(); i++)
                // {
                //     var index = startingIndex+i;
                //     if(index > cups.Count()-1)
                //         index -= cups.Count();

                //     newCups.Add(cups[index]);
                // }
                // cups = newCups.ToList();

                currentIndex = (cups.IndexOf(currentIndexLabel)+1)%cupsCount;
                iterCount++;

                if(iterCount%(iterations/100)==0)
                    Console.WriteLine((iterCount/100)+" percent"); 
            }

            return cups;
        }
    }
}