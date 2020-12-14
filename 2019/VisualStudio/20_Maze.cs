using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Advent
{
    public class Maze
    {
        List<string> map = new List<string>();

        Dictionary<string,Tuple<int,int,int,int>> Portals = new Dictionary<string, Tuple<int, int, int, int>>();
        Dictionary<int,List<int>> incompleteTiles = new Dictionary<int, List<int>>();
        Dictionary<int,List<int>> completedTiles = new Dictionary<int, List<int>>();
        Dictionary<int,Dictionary<long,long>> steps = new Dictionary<int, Dictionary<long, long>>();
        int width = 0;
        Dictionary<string,Dictionary<string,Tuple<bool,long>>> portalToPortal = new Dictionary<string, Dictionary<string, Tuple<bool, long>>>();

        int startingIndex = 0;
        int endingIndex = 0;
        public Maze(List<string> inputs)
        {
            map = inputs;
            width = map[0].Count();
 
            var firstY = map.IndexOf(map.First(x => x.Contains("#")));
            var firstX = map[firstY].IndexOf("#");
            var lastX = map[firstY].LastIndexOf("#");
            var lastY = map.IndexOf(map.Last(x => x.Contains("#")));

            for(int i = firstX; i < lastX; i++)
            {
                if(map[firstY][i] == '.')
                {
                    var name = map[0].Substring(i,1) + map[1].Substring(i,1);
                    Portals.Add(name.ToString(), new Tuple<int, int, int, int>(i,firstY-1,0,0));
                }
                if(map[lastY][i] == '.')
                {
                    var name = map[lastY+1].Substring(i,1) + map[lastY+2].Substring(i,1);
                    Portals.Add(name.ToString(), new Tuple<int, int, int, int>(i,lastY+1,0,0));
                }                
            }
            for(int i = firstY; i < lastY; i++)
            {
                if(map[i][firstX] == '.')
                {
                    var name = map[i].Substring(firstX-2,1) + map[i].Substring(firstX-1,1);
                    Portals.Add(name.ToString(), new Tuple<int, int, int, int>(firstX-1,i,0,0));
                }
                if(map[i][lastX] == '.')
                {
                    var name = map[i].Substring(lastX+1,1) + map[i].Substring(lastX+2,1);
                    Portals.Add(name.ToString(), new Tuple<int, int, int, int>(lastX+1,i,0,0));
                }                
            }

            //Inner Portals
            var firstInnerY = map .IndexOf(map.Skip(firstY).First(x => x.Substring(firstX,lastX-firstX).Contains(' ')));
            var lastInnerY = map.LastIndexOf(map.Skip(firstY).Take(lastY-firstY).Last(x => x.Substring(firstX,lastX-firstX).Contains(' ')));
            var firstInnerX = map[firstInnerY].Substring(firstX,lastX-firstX).IndexOf(' ')+firstX;
            var lastInnerX = map[firstInnerY].Substring(firstX,lastX-firstX).LastIndexOf(' ')+firstX;
        
            for(int i = firstInnerX; i < lastInnerX; i++)
            {
                if(map[firstInnerY-1][i] == '.')
                {
                    var name = map[firstInnerY].Substring(i,1) + map[firstInnerY+1].Substring(i,1);
                    
                    Portals[name] = new Tuple<int, int, int, int>(Portals[name].Item1, Portals[name].Item2, i, firstInnerY);
                }
                if(map[lastInnerY+1][i] == '.')
                {
                    var name = map[lastInnerY-1].Substring(i,1) + map[lastInnerY].Substring(i,1);
                    Portals[name] = new Tuple<int, int, int, int>(Portals[name].Item1, Portals[name].Item2, i, lastInnerY);
                }                
            }
            for(int i = firstInnerY; i < lastInnerY; i++)
            {
                if(map[i][firstInnerX-1] == '.')
                {
                    var name = map[i].Substring(firstInnerX,1) + map[i].Substring(firstInnerX+1,1);
                    Portals[name] = new Tuple<int, int, int, int>(Portals[name].Item1, Portals[name].Item2, firstInnerX, i);
                }
                if(map[i][lastInnerX+1] == '.')
                {
                    var name = map[i].Substring(lastInnerX-1,1) + map[i].Substring(lastInnerX,1);
                    Portals[name] = new Tuple<int, int, int, int>(Portals[name].Item1, Portals[name].Item2, lastInnerX, i);
                }                
            }

            endingIndex = Portals["ZZ"].Item2*width + Portals["ZZ"].Item1;
            startingIndex = Portals["AA"].Item2*width + Portals["AA"].Item1;
            
            FillPortalSteps();
            //GoAcrossMap();
            UseLayers();
        }

        private void UseLayers()
        {
            List<Tuple<int,string,long>> downPortals = new List<Tuple<int,string,long>>(){new Tuple<int, string, long>(1,"AAout",0)};


            long steps = 0;
            while(downPortals.Any())
            {
                
                var copy = downPortals.ToList();
                foreach(var portal in copy.OrderBy(x => x.Item1))
                {
                    var level = portal.Item1;
                    Console.WriteLine("Going through " + portal.Item2 + " on level " + level);

                    if(!portalToPortal.ContainsKey(portal.Item2))
                        continue;

                    var thisDown = portalToPortal[portal.Item2].Keys.Where(x => x.Contains("in")).ToList();
                    var thisUp = portalToPortal[portal.Item2].Keys.Where(x => x.Contains("out")).ToList();

                    foreach(var up in thisUp)
                    {
                        if((up == "AAout" || up == "ZZout") && level !=1)
                            continue;

                        if(level == 0)
                            continue;

                        var upSteps = portal.Item3 + portalToPortal[portal.Item2][up].Item2;
                        Console.WriteLine("\tTrying to go up portal " + up);
                        
                        var moreDown = new List<Tuple<int,string,long>>();
                        if(TryGoUp(level-1, up, ref upSteps, out moreDown))
                        {
                            steps = upSteps;
                            break;
                        }
                        else
                        {
                            foreach(var port in moreDown)
                            {
                                Console.WriteLine("    Adding down portal "+port.Item2.Replace("in","out")+ " level-" + port.Item1+" steps-"+port.Item3);
                                downPortals.Add(new Tuple<int, string, long>(port.Item1,port.Item2.Replace("in","out"),port.Item3));
                            }
                        }
                    }

                    if(steps != 0)
                        break;
                    
                    downPortals.Remove(portal);                    
                    foreach(var down in thisDown)
                    {
                        var downSteps = portal.Item3 + portalToPortal[portal.Item2][down].Item2;
                        Console.WriteLine("  Going down portal " + down + "  steps - "+ (downSteps-portal.Item3) + "  total - " + downSteps);
                        downPortals.Add(new Tuple<int,string, long>(level+1,down.Replace("in","out"), downSteps));
                    }
                }

                if(steps != 0)
                    break;

            }

            Console.WriteLine("Steps found: " + steps);
        }

        

        private bool TryGoUp(int level, string portalName, ref long steps, out List<Tuple<int,string,long>>downPortals)
        {
            downPortals = new List<Tuple<int,string,long>>();
            
            if(level < 0)
                return false;
            if(!portalName.Contains("out"))
                return false;

            portalName = portalName.Replace("out","in");

            foreach(var down in portalToPortal[portalName].Where(x => x.Key.Contains("in")))
            {
                Console.WriteLine("  #Steps from " + portalName +" to " + down.Key + " steps:" +down.Value.Item2);
                downPortals.Add(new Tuple<int,string, long>(level+1,down.Key, down.Value.Item2+steps));
            }

            if(level == 1)
            {
                if(portalToPortal[portalName].Keys.Contains("ZZout"))
                {
                    steps += portalToPortal[portalName]["ZZout"].Item2 -1;
                    Console.WriteLine("***** - Found exit.  Steps from portal "+portalName +" to ZZout "+ (portalToPortal[portalName]["ZZout"].Item2 -1));
                    return true;
                }
                else
                {
                    Console.WriteLine("\t\tNo luck");
                    return false;
                }
            }

            
            foreach(var outPortal in portalToPortal[portalName].Where(x=> x.Value.Item1))
            {
                if(outPortal.Key == "ZZout")
                    continue;

                var upperPortals = new List<Tuple<int,string,long>>(); 

                Console.WriteLine("Going up "+ portalName + " to "+ outPortal.Key+ " on level " + level);

                var thisSteps = (steps+outPortal.Value.Item2);
                if(TryGoUp(level-1 , outPortal.Key, ref thisSteps, out upperPortals))
                {
                    Console.WriteLine("   Adding up steps to " + outPortal.Key+ " : "+outPortal.Value.Item2);
                    steps = thisSteps;
                    return true;
                }
                downPortals.AddRange(upperPortals);
            }
            return false;
        }

        private void FillPortalSteps()
        {
            foreach(var portal in Portals)
            {
                if(portal.Key == "ZZ")
                    continue;
                
                Console.WriteLine("Getting portals for " + portal.Key+"out");
                TraverseMapToPortals(portal.Key+"out", true, portal.Value.Item1, portal.Value.Item2);

                if(portal.Key != "AA")
                {
                    Console.WriteLine("Getting portals for " + portal.Key+"in");
                    TraverseMapToPortals(portal.Key+"in", false, portal.Value.Item3, portal.Value.Item4);
                }
            }
        }

        private void TraverseMapToPortals(string portalName, bool outerPortal, int x, int y)
        {
            incompleteTiles.Clear();
            var currX = x;
            var currY = y;
            var dir = 2;

            List<int> xs = new List<int>(){x,x,x-1,x+1};
            List<int> ys = new List<int>(){y+1,y-1,y,y};
            for(int i = 0; i <xs.Count(); i++)
            {
                if(map[ys[i]][xs[i]]=='.')
                {
                    currX=xs[i];
                    currY=ys[i];
                    dir = i+1;
                }
            }

            incompleteTiles.Add(1, new List<int>(){currY*width + currX });
            completedTiles.Clear();
            completedTiles.Add(1, new List<int>(){y*width +x});

            var thisSteps = new Dictionary<long,long>();
            thisSteps.Add(currY*width+currX, 0);



            while(incompleteTiles[1].Any())
            {
                var goingTo = getNewPos(currX, currY, dir);
                var goingX = goingTo.Item1;
                var goingY = goingTo.Item2;
                var goingToIndex = goingY*width +goingX;

                if(Portals.Values.Any(x => goingX== x.Item1 && goingY == x.Item2))
                {
                    var portal = Portals.First(x => goingX== x.Value.Item1 && goingY == x.Value.Item2);
                    if(!portalToPortal.ContainsKey(portalName))
                        portalToPortal.Add(portalName, new Dictionary<string, Tuple<bool, long>>());

                    var goingToPortalName = portal.Key + "out";
                    if(goingToPortalName != portalName && !portalToPortal[portalName].ContainsKey(goingToPortalName))
                        portalToPortal[portalName].Add(goingToPortalName, new Tuple<bool, long>(true,thisSteps[currY*width+currX]+1));

                    if(incompleteTiles[1].Contains(goingToIndex))
                        incompleteTiles[1].Remove(goingToIndex);
                    if(!completedTiles[1].Contains(goingToIndex))
                        completedTiles[1].Add(goingToIndex);
                }
                else if(Portals.Values.Any(x => goingX== x.Item3 && goingY == x.Item4))
                {
                    var portal = Portals.First(x => goingX== x.Value.Item3 && goingY == x.Value.Item4);
                    if(!portalToPortal.ContainsKey(portalName))
                        portalToPortal.Add(portalName, new Dictionary<string, Tuple<bool, long>>());

                    var goingToPortalName = portal.Key + "in";
                    if(goingToPortalName!= portalName && !portalToPortal[portalName].ContainsKey(goingToPortalName))
                        portalToPortal[portalName].Add(goingToPortalName, new Tuple<bool, long>(false,thisSteps[currY*width+currX]+1));

                    if(incompleteTiles[1].Contains(goingToIndex))
                        incompleteTiles[1].Remove(goingToIndex);
                    if(!completedTiles[1].Contains(goingToIndex))
                        completedTiles[1].Add(goingToIndex);
                }

                if(thisSteps.ContainsKey(currY*width+currX) && !thisSteps.ContainsKey(goingY*width+goingX))
                {
                    thisSteps.Add(goingY*width+goingX, thisSteps[currY*width+currX]+1);
                }

                var nextTile = GetFirstIncompleteTile(false, portalName.Substring(0,2));
                currX = nextTile.Item1;
                currY = nextTile.Item2;
                dir = nextTile.Item3;
            }
            
        }


        public Tuple<int,int,int> GetFirstIncompleteTile(bool goThroughPortals = true, string startingPortal="")
        {
            var copy = incompleteTiles.ToDictionary(x=> x.Key, x=>x.Value.ToList());
            var neighbors = new List<int>();
            
            foreach(var level in copy.Keys)
            {
                foreach(var tileIndex in copy[level])
                {
                    neighbors.Clear();
                    neighbors.Add(tileIndex-width); //North
                    neighbors.Add(tileIndex+width); //South
                    neighbors.Add(tileIndex-1); //West
                    neighbors.Add(tileIndex+1); //East

                    for(int i = 0; i < neighbors.Count(); i++)
                    {
                        var neighborIndex = neighbors[i];
                        var row = neighborIndex/width;
                        var col = neighborIndex%width;

                        if(neighborIndex == startingIndex)
                            continue;
                        if(goThroughPortals && neighborIndex == endingIndex)
                            return new Tuple<int, int, int>(tileIndex%width,tileIndex/width, i+1);


                        var portalOut = "";
                        if(Portals.Values.Any(x => x.Item1==col && x.Item2 == row))
                        {
                            portalOut = Portals.Keys.First(x => Portals[x].Item1==col && Portals[x].Item2 == row);
                            if(goThroughPortals)
                            {
                                completedTiles[level].Add(neighborIndex);
                                incompleteTiles[level].Add(Portals[portalOut].Item4*width + Portals[portalOut].Item3);
                            
                                var output = FindPortalOutput(Portals[portalOut].Item3, Portals[portalOut].Item4);
                                col = output.Item1;
                                row = output.Item2;
                            }
                        }
                        else if(Portals.Values.Any(x => x.Item3==col && x.Item4 == row))
                        {
                            portalOut = Portals.Keys.First(x => Portals[x].Item3==col && Portals[x].Item4 == row);
                            
                            if(goThroughPortals)
                            {
                                completedTiles[level].Add(neighborIndex);
                                incompleteTiles[level].Add(Portals[portalOut].Item2*width + Portals[portalOut].Item1);
                                
                                var output = FindPortalOutput(Portals[portalOut].Item1, Portals[portalOut].Item2);
                                col = output.Item1;
                                row = output.Item2;
                            }
                        }                        

                        var index = row*width+col;                          
                        if((map[row][col] == '.' || (!goThroughPortals && Regex.Match(map[row][col].ToString(), @"[A-Z]").Success && portalOut!= startingPortal))
                            && !incompleteTiles[1].Contains(index)
                            && !completedTiles[1].Contains(index))
                        {
                            if(goThroughPortals && portalOut.Count() > 0)
                            {
                                Console.WriteLine("Going through portal " + portalOut);
                                
                                // completedTiles[level].Add(tileIndex);
                                // incompleteTiles[level].Remove(tileIndex);
                            }
                            else
                            {
                                incompleteTiles[1].Add(row*width+col);
                            }

                            return new Tuple<int, int, int>(tileIndex%width,tileIndex/width,i+1);
                        }

                    }
                    completedTiles[level].Add(tileIndex);
                    incompleteTiles[level].Remove(tileIndex);
                }
            }

            return new Tuple<int, int, int>(0, 0, 0);
        }

        private void GoAcrossMap()
        {
            steps = new Dictionary<int, Dictionary<long, long>>();


            completedTiles.Clear();
            incompleteTiles.Clear();
            completedTiles.Add(1, new List<int>(){startingIndex});
            incompleteTiles.Add(1, new List<int>(){startingIndex+width});
            

            steps.Add(1, new Dictionary<long, long>());
            steps[1].Add(startingIndex+width, 0);
            
            var currIndex = incompleteTiles[1].First();
            var direction = 2;
            bool gotToEnd = false;

            while(incompleteTiles.All(x => x.Value.Count() >0))
            {
                var currX = currIndex%width;
                var currY = currIndex/width;

                var goingTo = getNewPos(currX,currY, direction);
                var goingToIndex = goingTo.Item2*width+goingTo.Item1;


                if(goingToIndex == endingIndex)
                {
                    var maxSteps = steps[1].Select(x =>x.Value).Max();
                    Console.WriteLine("Got to the end steps: " + maxSteps.ToString());
                    steps[1].Add(endingIndex,maxSteps);
                    break;
                }

                if(!steps.ContainsKey(1))
                    steps.Add(1, new Dictionary<long,long>());

                if(steps[1].ContainsKey(currIndex) && !steps[1].ContainsKey(goingToIndex))
                {
                    steps[1].Add(goingToIndex, steps[1][currIndex]+1);
                    if(Portals.Any( x => x.Value.Item2==goingTo.Item2 && x.Value.Item1==goingTo.Item1))
                    {
                        var newPoint = Portals.First( x => x.Value.Item2==goingTo.Item2 && x.Value.Item1==goingTo.Item1);
                        Console.WriteLine("Steps to outer portal " + newPoint.Key + " " +(steps[1][currIndex]+1));                       

                        var portalOut = FindPortalOutput(newPoint.Value.Item3, newPoint.Value.Item4);
                        var goingToIndex2 = portalOut.Item1+portalOut.Item2*width;
                        steps[1].Add(goingToIndex2, steps[1][goingToIndex]);
                        completedTiles[1].Add(currIndex);
                        incompleteTiles[1].Remove(currIndex);
                        incompleteTiles[1].Add(goingToIndex2);
                    }
                    else if(Portals.Any( x => x.Value.Item4==goingTo.Item2 && x.Value.Item3==goingTo.Item1))
                    {
                        var newPoint = Portals.First( x => x.Value.Item4==goingTo.Item2 && x.Value.Item3==goingTo.Item1);
                        Console.WriteLine("Steps to inner portal " + newPoint.Key + " " +(steps[1][currIndex]+1));     

                        var portalOut = FindPortalOutput(newPoint.Value.Item1, newPoint.Value.Item2);
                        var goingToIndex2 = portalOut.Item1+portalOut.Item2*width;
                        steps[1].Add(goingToIndex2, steps[1][goingToIndex]);
                        
                        completedTiles[1].Add(currIndex);
                        incompleteTiles[1].Remove(currIndex);
                        incompleteTiles[1].Add(goingToIndex2);
                    }
                }

                // if(!completedTiles[1].Contains(goingToIndex)&& 
                //     !incompleteTiles[1].Contains(goingToIndex))
                //     incompleteTiles[1].Add(goingToIndex);


                var nextTile = GetFirstIncompleteTile();
                currIndex = nextTile.Item2*width+ nextTile.Item1;
                direction = nextTile.Item3;

            }
            

            Console.WriteLine(String.Format("Steps from AA to ZZ {0}",steps[1][endingIndex]));
        }


        public Tuple<int,int> FindPortalOutput(int x, int y)
        {
            var xs = new List<int>(){x,x,x+1,x-1};
            var ys = new List<int>(){y+1, y-1,y,y};

            for(int i = 0; i<xs.Count(); i++)
            {
                if(map[ys[i]][xs[i]] == '.')
                    return new Tuple<int, int>(xs[i],ys[i]);
            }

            return new Tuple<int, int>(0,0);

        }

        public int DirToTheRIght(int currDirection)
        {
            switch(currDirection)
            {
                case 1: //north
                    return 4;
                case 2: //south
                    return 3;
                case 3: //west
                    return 1;
                case 4: //east
                    return 2;
            }

            return 0;
        }

        public int DirToTheLeft(int currDirection)
        {
            switch(currDirection)
            {
                case 1: //north
                    return 3;
                case 2: //south
                    return 4;
                case 3: //west
                    return 2;
                case 4: //east
                    return 1;
            }

            return 0;
        }

        public Tuple<int,int> getNewPos(int currX, int currY, int currDir)
        {
            switch(currDir)
            {
                case 1: //north
                    currY -= 1;
                    break;
                case 2: //south
                    currY += 1;
                    break;
                case 3: //west
                    currX -= 1;
                    break;
                case 4: //east
                    currX += 1;
                    break;
            }

            return new Tuple<int, int>(currX, currY);
        }
    }
}