using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

namespace Advent
{
    public class Day17
    {

        Dictionary<int, List<string>> map = new Dictionary<int, List<string>>();
        Dictionary<int, Dictionary<int,List<string>>> map4D = new Dictionary<int, Dictionary<int, List<string>>>();
        public Day17(List<string> inputs)
        {
            map.Add(0,inputs);
            map4D.Add(0,new Dictionary<int, List<string>>());
            map4D[0].Add(0,inputs);

            int cycles = 0;
            int maxCycle = 6;
            while(cycles< maxCycle)
            {
                cycle4D();
                cycles++;
            }

            var count = 0;
            foreach(var w in map4D)
            {
                foreach(var z in map4D[w.Key])
                {
                    count += z.Value.Select(y => y.Where(x => x=='#').Count()).Sum();
                }
            }
            Console.WriteLine("Active Count :" +count);
        }

        private void cycle3D()
        {
            //expandMap
            var xs = "";
            while(xs.Length < map[0].First().Length+2)
            {
                xs+=".";
            }
            List<string>  ys = new List<string>();
            while(ys.Count() < map[0].Count()+2)
            {
                ys.Add(xs);
            }

            var zMax = map.Keys.Max();
            var zMin = map.Keys.Min();




            //Expand Existing Levels
            foreach(var zValue in map)
            {
                for(int i = 0; i< zValue.Value.Count(); i++)
                {
                    map[zValue.Key][i]=map[zValue.Key][i].Insert(0,".");
                    map[zValue.Key][i] += ".";
                }


                map[zValue.Key].Insert(0,xs);
                map[zValue.Key].Add(xs);
            }

            map.Add(zMax+1, ys.ToList());
            map.Add(zMin-1, ys.ToList());
            
            var copy = map.ToDictionary(x=>x.Key, x=>x.Value.ToList());
            foreach(var z in copy.Keys.ToList())
            {
                for(int y = 0; y< copy[z].Count(); y++)
                {
                    for(int x = 0; x<copy[z][y].Count(); x++)
                    {
                        var neighborActiveCount = 0;
                        bool isActive = copy[z][y][x].ToString() == "#";

                        for(int newZ = -1; newZ<=1; newZ++)
                        {
                            for(int newY = -1; newY<=1; newY++)
                            {
                                for(int newX = -1; newX<=1; newX++)
                                {
                                    if(newX==0 && newY ==0 && newZ==0)
                                        continue;
                                    if(!copy.Keys.Contains(newZ+z))
                                        continue;
                                    if((y+newY)<0 || (y+newY)>= copy[newZ+z].Count())
                                        continue;
                                    if((x+newX)<0 || (x+newX)>= copy[newZ+z][newY+y].Count())
                                        continue;

                                    var presentNeighborState = copy[newZ+z][newY+y][newX+x].ToString();
                                    if(presentNeighborState == "#")
                                        neighborActiveCount++;
                                }
                            }
                        }

                        if(isActive && neighborActiveCount != 2 && neighborActiveCount != 3)
                        {
                            SetState(x,y,z,false);
                        }
                        else if(!isActive && neighborActiveCount == 3)
                        {
                            SetState(x,y,z,true);
                        }
                    }
                }
            }
        }

        private void SetState(int x, int y, int z,bool active)
        {
            map[z][y] = map[z][y].Remove(x,1);
            map[z][y] = map[z][y].Insert(x,active?"#":".");
        }

        private void cycle4D()
        {
            //expandMap
            var xs = "";
            while(xs.Length < map4D[0][0].First().Length+2)
            {
                xs+=".";
            }
            List<string>  ys = new List<string>();
            while(ys.Count() < map4D[0][0].Count()+2)
            {
                ys.Add(xs);
            }


            Dictionary<int,List<string>> zs = new Dictionary<int, List<string>>();

            var zMax = map4D[0].Keys.Max();
            var zMin = map4D[0].Keys.Min();
            for(int z = zMin-1; z<= zMax+1; z++)
            {
                zs.Add(z, ys);
            }

            var wMax = map4D.Keys.Max();
            var wMin = map4D.Keys.Min();

            //Expand Existing Levels
            foreach(var wValue in map4D)
            {
                foreach(var zValue in wValue.Value)
                {
                    for(int i = 0; i< zValue.Value.Count(); i++)
                    {
                        map4D[wValue.Key][zValue.Key][i]=map4D[wValue.Key][zValue.Key][i].Insert(0,".");
                        map4D[wValue.Key][zValue.Key][i] += ".";
                    }

                    map4D[wValue.Key][zValue.Key].Insert(0,xs);
                    map4D[wValue.Key][zValue.Key].Add(xs);
                }

                for(int z = zMin-1; z<= zMax+1; z++)
                {
                    if(!map4D[wValue.Key].ContainsKey(z))
                        map4D[wValue.Key].Add(z, ys.ToList());
                }
            }

            map4D.Add(wMax+1, zs.ToDictionary(xs=>xs.Key, ys=>ys.Value.ToList()));
            map4D.Add(wMin-1, zs.ToDictionary(xs=>xs.Key, ys=>ys.Value.ToList()));


            
            var copy = map4D.ToDictionary(x=>x.Key, x=>x.Value.ToDictionary(x=>x.Key, x=>x.Value.ToList()));
            foreach( var w in copy.Keys.ToList())
            {
                foreach(var z in copy[w].Keys.ToList())
                {
                    for(int y = 0; y< copy[w][z].Count(); y++)
                    {
                        for(int x = 0; x<copy[w][z][y].Count(); x++)
                        {
                            var neighborActiveCount = 0;
                            bool isActive = copy[w][z][y][x].ToString() == "#";

                            for(int newW = -1; newW <=1; newW++)
                            {
                                for(int newZ = -1; newZ<=1; newZ++)
                                {
                                    for(int newY = -1; newY<=1; newY++)
                                    {
                                        for(int newX = -1; newX<=1; newX++)
                                        {
                                            if(newX==0 && newY ==0 && newZ==0 && newW==0)
                                                continue;
                                            if(!copy.ContainsKey(w+newW))
                                                continue;
                                            if(!copy[w+newW].Keys.Contains(newZ+z))
                                                continue;
                                            if((y+newY)<0 || (y+newY)>= copy[newW+w][newZ+z].Count())
                                                continue;
                                            if((x+newX)<0 || (x+newX)>= copy[newW+w][newZ+z][newY+y].Count())
                                                continue;

                                            var presentNeighborState = copy[newW+w][newZ+z][newY+y][newX+x].ToString();
                                            if(presentNeighborState == "#")
                                                neighborActiveCount++;
                                        }
                                    }
                                }
                            }

                            if(isActive && neighborActiveCount != 2 && neighborActiveCount != 3)
                            {
                                SetState4D(x,y,z,w,false);
                            }
                            else if(!isActive && neighborActiveCount == 3)
                            {
                                SetState4D(x,y,z,w,true);
                            }
                        }
                    }
                }  
            }
            
        }

        private void SetState4D(int x, int y, int z,int w,bool active)
        {
            map4D[w][z][y] = map4D[w][z][y].Remove(x,1);
            map4D[w][z][y] = map4D[w][z][y].Insert(x,active?"#":".");
        }
    }
}