using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Advent
{
    public class TractorBeam
    {
        List<string> map = new List<string>();
        public TractorBeam(List<string> inputs)
        {
            map.Clear();
            var scan = new  IntComputer(inputs);
            var tractorCounts = 0;

            scan.ResetProgram();
            for(int i = 0; i < 50; i++)
            {
                var thisLine = "";
                for(int j = 0; j < 50; j++)
                {
                    scan.ResetProgram();
                    scan.AddInput(i);
                    scan.AddInput(j);

                    scan.RunProgram(scan.nextStepIndex,false, true);
                    if(scan.lastOutput == 1)
                    {
                        tractorCounts++;
                        thisLine+="#";
                    }
                    else
                    {
                        thisLine += ".";
                    }
                }
                map.Add(thisLine);          
            }


            foreach(var line in map)
            {
                Console.WriteLine(line);
            }
            Console.WriteLine("Part 1 - squares :" + tractorCounts);

            Find100x100(scan);
        }

        public void Find100x100(IntComputer scanner)
        {
            var x = 0;
            var y = 10;

            var found  =false;
            while(!found)
            {

                var foundTractor = false;
                var lostTractor = false;

                while(!lostTractor)
                {
                    scanner.ResetProgram();
                    scanner.AddInput(x);
                    scanner.AddInput(y);
                    scanner.RunProgram(0,false,true);

                    if(!foundTractor && scanner.lastOutput == 1)
                        foundTractor = true;

                    if(foundTractor && scanner.lastOutput == 0)
                    {
                        lostTractor = true;
                        x-=1;
                        break;
                    }

                    x +=1;
                }

                if(x<100 || y< 100)
                {
                    y++;
                    continue;
                }

                var boundariesX = new List<int>{x-99, x-99, x};
                var boundariesY = new List<int>{y, y+99, y+99};

                found = true;
                for(int i =0; i< boundariesX.Count(); i++)
                {
                    scanner.ResetProgram();
                    scanner.AddInput(boundariesX[i]);
                    scanner.AddInput(boundariesY[i]);
                    scanner.RunProgram(0,false,true);

                    if(scanner.lastOutput == 0)
                    {
                        found = false;
                        break;
                    }
                }

                if(found)
                {
                    Console.WriteLine(String.Format("x-{0}  y-{1}",x-99,y));
                }
                y++;
                
            }
        }
    }
}