using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

namespace Advent
{
    public class Day24
    {
        Dictionary<int, List<string>> directions = new Dictionary<int, List<string>>();
        Dictionary<int, List<int>> endTiles = new Dictionary<int, List<int>>();

        public Day24(List<string> inputs)
        {
            var index = 0;
            foreach(var line in inputs)
            {
                if(!directions.ContainsKey(index))
                {
                    directions.Add(index, new List<string>());
                    endTiles.Add(index, new List<int>(){0,0});
                }

                for(int i = 0; i<line.Length; i++)
                {
                    if(line[i]=='e' || line[i]=='w')
                    {
                        directions[index].Add(line[i].ToString());
                    }
                    else
                    {
                        directions[index].Add(line.Substring(i,2));
                        i++;
                    }
                }

                index++;
            }

            foreach(var movement in directions.Keys)
            {
                foreach(var dir in directions[movement])
                {
                    switch(dir)
                    {
                        case "e":
                            endTiles[movement][0] +=2;
                            break;
                        case "ne":
                            endTiles[movement][0] +=1;
                            endTiles[movement][1] +=1;
                            break;
                        case "se":
                            endTiles[movement][0] +=1;
                            endTiles[movement][1] -=1;
                            break;
                        case "w":
                            endTiles[movement][0] -=2;
                            break;
                        case "sw":
                            endTiles[movement][0] -=1;
                            endTiles[movement][1] -=1;
                            break;
                        case "nw":
                            endTiles[movement][0] -=1;
                            endTiles[movement][1] +=1;
                            break;
                    }
                }
            }

            Dictionary<int, List<int>> blackTiles = new Dictionary<int, List<int>>();

            foreach(var endTile in endTiles.Values)
            {
                if(blackTiles.ContainsKey(endTile[0]) &&
                    blackTiles[endTile[0]].Contains(endTile[1]))
                {
                    blackTiles[endTile[0]].Remove(endTile[1]);

                    if(blackTiles[endTile[0]].Count() == 0)
                        blackTiles.Remove(endTile[0]);
                    continue;
                }

                if(!blackTiles.ContainsKey(endTile[0]))
                {
                    blackTiles.Add(endTile[0], new List<int>());
                }

                if(!blackTiles[endTile[0]].Contains(endTile[1]))
                { 
                    blackTiles[endTile[0]].Add(endTile[1]);
                }
            }

            var count = 0;
            foreach(var blackTile in blackTiles.Values)
            {
                count+= blackTile.Count();
            }
            Console.WriteLine("Part 1: black Tiles " + count);

            var iter = 0;
            while(iter<100)
            {
                blackTiles = ShiftBlackTiles(blackTiles);
                iter++;
            }
            
            Console.WriteLine("Part 2: black Tiles " + blackTiles.Select(x=>x.Value.Count()).Sum());
        }

        private Dictionary<int,List<int>> ShiftBlackTiles(Dictionary<int,List<int>> currentBlackTiles)
        {
            var copy = currentBlackTiles.ToDictionary(x => x.Key, y=> y.Value.ToList());

            var Neighbors = new List<Tuple<int,int>>();
            Neighbors.Add(new Tuple<int, int>(1,1));
            Neighbors.Add(new Tuple<int, int>(-1,1));
            Neighbors.Add(new Tuple<int, int>(1,-1));
            Neighbors.Add(new Tuple<int, int>(-1,-1));
            Neighbors.Add(new Tuple<int, int>(2,0));
            Neighbors.Add(new Tuple<int, int>(-2,0));

            var whiteTiles = new Dictionary<int,Dictionary<int,int>>();

            foreach(var x in currentBlackTiles)
            {
                foreach(var y in x.Value)
                {
                    var blackNeighbors = 0;

                    foreach(var neighbor in Neighbors)
                    {
                        var neighborX = x.Key+neighbor.Item1;
                        var neighborY = y+neighbor.Item2;
                        

                        if(currentBlackTiles.ContainsKey(neighborX) && 
                            currentBlackTiles[neighborX].Contains(neighborY))
                        {
                            blackNeighbors++;
                            continue;
                        }

                        if(whiteTiles.ContainsKey(neighborX) &&
                            whiteTiles[neighborX].ContainsKey(neighborY))
                        {
                            whiteTiles[neighborX][neighborY]++;
                            continue;
                        }

                        if(!whiteTiles.ContainsKey(neighborX))
                            whiteTiles.Add(neighborX, new Dictionary<int, int>());

                        if(!whiteTiles[neighborX].ContainsKey(neighborY))
                            whiteTiles[neighborX].Add(neighborY,1);
                    }

                    if(blackNeighbors ==0 || blackNeighbors>2)
                    {
                        copy[x.Key].Remove(y);
                        if(copy[x.Key].Count() ==0)
                            copy.Remove(x.Key);
                    }
                }
            }

            foreach(var x in whiteTiles)
            {
                foreach(var y in x.Value)
                {
                    if(y.Value == 2)
                    {
                        if(!copy.ContainsKey(x.Key))
                        {
                            copy.Add(x.Key,new List<int>(){y.Key});
                        }
                        else
                        {
                            copy[x.Key].Add(y.Key);
                        }
                        
                    }   
                }
            }

            return copy;

        }
    }
}