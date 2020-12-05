using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    public class Asteriods
    {
        List<string> AssSpace = new List<string>();
        public Asteriods(List<string> inputs)
        {
            AssSpace = inputs;
        }

        public void PrintMaxAss()
        {
            int maxAss = 0;
            int maxX = 0;
            int maxY = 0;

            for(int y1 = 0; y1 < AssSpace.Count; y1++)
            {
                for(int x1 = 0; x1 < AssSpace[y1].Count(); x1++)
                {
                    if(AssSpace[y1][x1] != '#')
                        continue;

                    int thisAss = 0;

                    for(int y2 = 0; y2 < AssSpace.Count; y2++)
                    {
                        for(int x2 = 0; x2 < AssSpace[y2].Count(); x2++)
                        {
                            if(y1==y2 && x1==x2)
                                continue;

                            if(AssSpace[y2][x2] != '#')
                                continue;

                            if(CanAss1SeeAss2(x1,y1,x2,y2))
                            {
                                thisAss++;
                            }
                        }
                    }

                    if(thisAss > maxAss)
                    {
                        maxAss = thisAss;
                        maxX = x1;
                        maxY = y1;
                    }
                }
            }

            Console.WriteLine("Part 1: Found the Max Ass: " +maxAss);

            FireEverything(maxX, maxY);
        }

        private bool CanAss1SeeAss2(int x1, int y1, int x2, int y2)
        {
            int gcd = (int)MoreMath.getGCD(Math.Abs(x2-x1),Math.Abs(y2-y1));

            int xstep = (x2-x1)/gcd;
            int ystep = (y2-y1)/gcd;

            int checkx = x1+xstep;
            int checky = y1+ystep;

            while(!(checky == y2 && checkx == x2))
            {
                if(AssSpace[checky][checkx] == '#')
                    return false;

                checky += ystep;
                checkx += xstep;
            }

            return true;
        }




        private void FireEverything(int x, int y)
        {
            var copyMap = AssSpace.ToList<string>();

            List<Tuple<int, int, double,double>> asteriodsFromCenter = new List<Tuple<int, int, double, double>>();
            for(int y2 = 0; y2 < AssSpace.Count(); y2++)
            {
                for(int x2 = 0; x2 < AssSpace[y2].Count(); x2++)
                {
                    if(AssSpace[y2][x2] != '#')
                        continue;

                    if(y2 == y && x2 ==x)
                        continue;

                    var angy = y-y2;
                    var angx = x2-x;
                    var ang = Math.Atan2(angy,angx);

                    if(ang < 0)
                        ang+= 2*Math.PI;

                    ang = ang*180/Math.PI;
                    var dist = Math.Abs(Math.Sqrt(Math.Pow(x2-x,2)+Math.Pow(y2-y,2)));
                    asteriodsFromCenter.Add(new Tuple<int,int, double,double>(x2,y2, ang, dist));
                }
            }

            asteriodsFromCenter =  asteriodsFromCenter.OrderBy(x => x.Item3).Reverse().ToList();


            var assteriodsDestroyed = new List<Tuple<int,int>>();
            var uniqueAngles = asteriodsFromCenter.Select(x => x.Item3).Distinct().ToList();
            var sortedAngles = uniqueAngles.Where(x => x <= 90).ToList();
            sortedAngles.AddRange(uniqueAngles.Where(x=> x> 90));

            while(assteriodsDestroyed.Count != asteriodsFromCenter.Count)
            {
                foreach(var angle in sortedAngles)
                {
                    var assWithThisAngle = asteriodsFromCenter.Where(x => x.Item3==angle && 
                                                                    !assteriodsDestroyed.Any(d => d.Item1 == x.Item1 && d.Item2 == x.Item2))
                                                                .OrderBy(x => x.Item4)
                                                                .ToList();
                    if(!assWithThisAngle.Any())
                        continue;


                    var assToDestroy = assWithThisAngle.First();
                    assteriodsDestroyed.Add(new Tuple<int, int>(assToDestroy.Item1,assToDestroy.Item2));
                    Console.WriteLine(String.Format("#{0} destroyed: x - {1}, y - {2}", assteriodsDestroyed.Count, assToDestroy.Item1, assToDestroy.Item2));
                }
            }
        }
    }
}