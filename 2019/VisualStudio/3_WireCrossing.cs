using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    public class Wires
    {
        List<string> Wire1 = new List<string>();
        List<string> Wire2 = new List<string>();
        List<Tuple<int,int,int>> intersections = new List<Tuple<int, int, int>>();

        public Wires(List<string> input)
        {
            Wire1 = input[0].Split(',').ToList<string>();
            Wire2 = input[1].Split(',').ToList<string>();
            GetIntersections();
        }

        public void GetIntersections()
        {
            var Wire1Indicies = GetWireLocations(Wire1);
            var Wire2Indicies = GetWireLocations(Wire2);

            for(var index = 0; index<Wire1Indicies.Count-1; index++)
            {
                for(var ind2 = 0; ind2 < Wire2Indicies.Count -1 ; ind2++)
                {
                    var x = 0;
                    var y = 0;
                    var totalsteps = 0;
                    if(Wire1Indicies[index].Item1 == Wire1Indicies[index+1].Item1 &&
                        IsValueBetweenTwoOthers(Wire1Indicies[index].Item1, Wire2Indicies[ind2].Item1, Wire2Indicies[ind2+1].Item1) &&
                        IsValueBetweenTwoOthers(Wire2Indicies[ind2].Item2, Wire1Indicies[index].Item2, Wire1Indicies[index+1].Item2))
                    {
                        
                        //Wire1 vertical
                        //Wire2 horizontal
                        x = Wire1Indicies[index].Item1;
                        y = Wire2Indicies[ind2].Item2;
                        totalsteps = Wire1Indicies[index].Item3 + Math.Abs(y - Wire1Indicies[index].Item2) +
                                    Wire2Indicies[ind2].Item3 + Math.Abs(x - Wire2Indicies[ind2].Item1);

                    }
                    else if(IsValueBetweenTwoOthers(Wire1Indicies[index].Item2, Wire2Indicies[ind2].Item2, Wire2Indicies[ind2+1].Item2) &&
                            IsValueBetweenTwoOthers(Wire2Indicies[ind2].Item1, Wire1Indicies[index].Item1, Wire1Indicies[index+1].Item1))
                    {
                        //Wire1 horizontal
                        //Wire2 vertical
                        x = Wire2Indicies[ind2].Item1;
                        y = Wire1Indicies[index].Item2;
                        totalsteps = Wire1Indicies[index].Item3 + Math.Abs(x - Wire1Indicies[index].Item1) +
                                    Wire2Indicies[ind2].Item3 + Math.Abs(y - Wire2Indicies[ind2].Item2);                            

                    }

                    if(!(x==0&&y==0) && !intersections.Any(point => point.Item1==x&& point.Item2 ==y))
                    {
                        intersections.Add(new Tuple<int, int, int>(x,y,totalsteps));
                    }
                    
                }
            }

        }

        private bool IsValueBetweenTwoOthers(int val, int compare1, int compare2)
        {
            if(compare1 > compare2)
            {
                return val <= compare1 && val >= compare2;
            }
            else
            {
                return val <= compare2 && val >= compare1;
            }
        }

        public List<Tuple<int,int,int>> GetWireLocations(List<string> wire)
        {
            List<Tuple<int,int,int>> indicies = new List<Tuple<int, int, int>>();
            indicies.Add(new Tuple<int, int, int>(0,0,0));

            foreach(var step in wire)
            {
                var steps = Int32.Parse(step.Substring(1));
                var prev =  indicies.Last();
                var next = new Tuple<int,int,int>(prev.Item1,prev.Item2, prev.Item3);

                switch(step[0])
                {
                    case 'L':
                        indicies.Add(new Tuple<int, int, int>(prev.Item1-steps,prev.Item2, prev.Item3+steps));
                        break;
                    case 'R':
                        indicies.Add(new Tuple<int, int, int>(prev.Item1+steps,prev.Item2, prev.Item3+steps));
                        break;
                    case 'U':
                        indicies.Add(new Tuple<int, int, int>(prev.Item1,prev.Item2+steps, prev.Item3+steps));
                        break;
                    case 'D':
                        indicies.Add(new Tuple<int, int, int>(prev.Item1,prev.Item2-steps, prev.Item3+steps));
                        break;
                }
            }

            return indicies;
        }

        public void PrintMinDistIntersection()
        {
            var dists = intersections.Select(point => Math.Abs(point.Item1)+Math.Abs(point.Item2));
            Console.WriteLine(String.Format("Minimum dist of intersection is : {0}", dists.Min()));
        }

        public void PrintMinStepsIntersections()
        {
            Console.WriteLine(String.Format("Minimum steps to intersection is : {0}", intersections.Select(point => point.Item3).Min()));
        }
    }

}