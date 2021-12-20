using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    class ProbeLauching
    {
        List<int> entries;
        int maxX = 176;
        int minX = 119;
        int maxY = -84;
        int minY = -141;

        public ProbeLauching(List<string> lines)
        {
            var startingX = 0;
            var startingY = 0;

            var maxXVel = maxX;
            var minXVel =0;
            while(minXVel*(minXVel+1)/2 <minX)
            {
                minXVel +=1;
            }

            Dictionary<int, List<int>> xSpeeds = new Dictionary<int, List<int>>();
            var maxYReached =0;
            for(int xVel = minXVel; xVel<=maxXVel; xVel++)
            {
                var stepsInRange = new List<int>();
                var step = 0;
                var xpos = 0;
                while(xpos<= maxX)
                {
                    bool zeroVel = xVel - step <= 0;
                    if(!zeroVel)
                        xpos+= xVel-step;
                    step+=1;
                    if(xpos<= maxX && xpos >= minX)
                    {
                        if(zeroVel)
                        {
                            stepsInRange.Add(0);
                            break;
                        }
                        else
                        {
                            stepsInRange.Add(step);
                        }
                    }    
                }
                if(stepsInRange.Any())
                    xSpeeds.Add(xVel,stepsInRange.ToList());
            }

            Dictionary<int, List<int>> ySpeeds = new Dictionary<int, List<int>>();
            var maxYsp = 0;
            
            var foundFirst = false;
            var totCombos = 0;
            for(int yvel = Math.Abs(minY)-1; yvel>=minY; yvel--)
            {
                int ypos = 0;
                var steps = 0;

                while(ypos>= minY)
                {
                    ypos = ypos+yvel-steps;
                    steps+=1;

                    if(ypos<=maxY && ypos>=minY)
                    {
                        foreach(var xsp in xSpeeds)
                        {
                            if(xsp.Value.Contains(steps) || 
                                xsp.Value.Last()==0 && xsp.Value.ElementAt(xsp.Value.Count()-2)<steps)
                            {

                                if(!ySpeeds.ContainsKey(yvel))
                                    ySpeeds.Add(yvel,new List<int>());

                                if(!ySpeeds[yvel].Contains(xsp.Key))
                                    ySpeeds[yvel].Add(xsp.Key);

                                if(!foundFirst)
                                {
                                    foundFirst = true;
                                    Console.WriteLine(yvel*(yvel+1)/2);
                                }
                            }

                        }

                    }

                }
            }


            totCombos = ySpeeds.Values.Select(g => g.Count()).Sum();
            Console.WriteLine(totCombos);
            //2727 too low
        }
    }    
}