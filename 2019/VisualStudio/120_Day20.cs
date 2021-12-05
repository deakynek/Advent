using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

namespace Advent
{
    public class Day20
    {

        Dictionary<int,List<string>> tiles = new Dictionary<int, List<string>>();

        Dictionary<int, List<List<int>>> TileOrientaions = new Dictionary<int, List<List<int>>>();

        Dictionary<int, Tuple<int, bool, int>> realMap = new Dictionary<int, Tuple<int, bool, int>>();
        public Day20(List<string> inputs)
        {
            var tileNumber = 0;
            var currentTile = new List<string>();
            for(int i = 0; i<inputs.Count; i++)
            {
                var line = inputs[i];
                if(line.Contains("Tile "))
                {
                    tileNumber = Int32.Parse(line.Substring(5,4));
                    continue;
                }

                if(String.IsNullOrEmpty(line))
                {
                    tiles.Add(tileNumber, currentTile.ToList());
                    currentTile.Clear();
                    continue;
                }

                currentTile.Add(line);

                if(i == inputs.Count-1)
                {
                    tiles.Add(tileNumber, currentTile.ToList());
                }


            }
            fillTileOrientations();
            var corners = findCornerTiles();

            long mult = 1;
            foreach(var corner in corners)
            {
                mult = corner*mult;
            }

            Console.WriteLine("Part 1 corners multiplied: " + mult);

            var Test = new List<string>(){"01234","56789","ABCDE","FGHIJ","KLMNO"};
            for(int i=0; i<=3; i++)
            {
                var non = GetTile(Test,false,i,false);
                var flipped = GetTile(Test,true,i,false);

                Console.WriteLine("");
                foreach(var line in non)
                {
                    Console.WriteLine(line);
                }
                Console.WriteLine("");
                foreach(var line in flipped)
                {
                    Console.WriteLine(line);
                }
            }

            realMap = FillEdges(12, corners);
            fillMap();
            
            Console.WriteLine("Part 2 non Monster Tiles: " + HuntMonsters());
        }

        private int HuntMonsters()
        {
            var stringTiles = new Dictionary<int, List<string>>();

            foreach(var tile in realMap.OrderBy(x => x.Key))
            {
                stringTiles.Add(tile.Key, GetTile(tiles[tile.Value.Item1], tile.Value.Item2, tile.Value.Item3,true));
            }

            List<string> stringMap = new List<string>();
            var tileWidth = (int)Math.Sqrt(stringTiles.Count);
            foreach(var tile in stringTiles.OrderBy(x => x.Key))
            {
                var baseLine = (tile.Key/tileWidth)*tile.Value.Count();
                for(int i =0; i<tile.Value.Count(); i++)
                {
                    if(stringMap.Count()<baseLine+i+1)
                        stringMap.Add("");

                    stringMap[baseLine+i] += tile.Value[i];
                }
            }

            int HashTiles = 0;
            foreach(var line in stringMap)
            {
                HashTiles += line.Count(x => x=='#');
            }

            var monsterCount = 0;
            for(int rot = 0; rot<=3; rot++)
            {
                monsterCount = CountMonsterTiles(GetTile(stringMap, false, rot, false));

                if(monsterCount > 0)
                {
                    Console.WriteLine("Rot: "+ rot + "Not Flipped, count: " +monsterCount);
                    break;
                }

                monsterCount = CountMonsterTiles(GetTile(stringMap, true, rot, false));

                if(monsterCount > 0)
                {
                    Console.WriteLine("Rot: "+ rot + " Flipped, count: " +monsterCount);
                    break;
                }

            }

            return HashTiles-monsterCount;

        }

        private int CountMonsterTiles(List<string> currentMap)
        {
            int monsterLength = 20;
            List<List<int>> monsterDef = new List<List<int>>();
            monsterDef.Add(new List<int>(){-18,-13,-12,-7,-6,-1,0,1});
            monsterDef.Add(new List<int>(){-17,-14,-11,-8,-5,-2});
            List<int> indexes = new List<int>();


            for(int y = 0; y < currentMap.Count()-2; y++)
            {
                for(int x = monsterLength-2; x<currentMap[y].Length-2; x++)
                {
                    if(currentMap[y][x] == '#')
                    {
                        var yDiff = 0;
                        var didNotMeetConditions = false;
                        var thisMonsterIndexes = new List<int>(){y*currentMap[0].Length + x};
                        foreach(var monsterY in monsterDef)
                        {
                            yDiff++;
                            foreach(var monsterX in monsterY)
                            {
                                if(currentMap[y+yDiff][x+monsterX] != '#')
                                {
                                    didNotMeetConditions = true;
                                    break;
                                }

                                thisMonsterIndexes.Add((y+yDiff)*currentMap[0].Length + x+monsterX);
                            }

                            if(didNotMeetConditions)
                            {
                                break;
                            }
                        }

                        if(didNotMeetConditions)
                            continue;

                        indexes.AddRange(thisMonsterIndexes);
                    }
                }
            }

            if(indexes.Count()>0)
            {
                foreach(var line in currentMap)
                {
                    Console.WriteLine(line);
                }
            }

            return indexes.Distinct().Count();
        }

        public List<string> GetTile(List<string> tile, bool flip, int rot, bool removeBorder)
        {
            var ret = new List<string>();
            var lastIndex = tile[0].Length-1;

            for(int i = 0; i<tile.Count(); i++)
            {
                var line = "";
                for(int j = 0; j<tile[i].Count(); j++)
                {
                    if(flip)
                    {
                        switch(rot)
                        {
                            case 0:
                                line += tile[i].Substring(lastIndex-j,1);
                                break;
                            case 1:
                                line += tile[lastIndex-j].Substring(lastIndex-i,1);
                                break;
                            case 2:
                                line += tile[lastIndex-i].Substring(j,1);
                                break;
                            case 3:
                                line += tile[j].Substring(i,1);
                                break;

                        }
                    }
                    else
                    {
                        switch(rot)
                        {
                            case 0:
                                line += tile[i].Substring(j,1);
                                break;
                            case 1:
                                line += tile[lastIndex-j].Substring(i,1);
                                break;
                            case 2:
                                line += tile[lastIndex-i].Substring(lastIndex-j,1);
                                break;
                            case 3:
                                line += tile[j].Substring(lastIndex-i,1);
                                break;
                        }
                    }

                }

                ret.Add(line);
            }

            if(!removeBorder)
                return ret;


            var newRet = new List<string>();
            for(int i = 1; i<ret.Count()-1; i++)
            {
                newRet.Add(ret[i].Substring(1,lastIndex-1));
            }

            return newRet;
        }

        private Dictionary<int, Tuple<int,bool,int>> FillEdges(int sideLength, List<int> corners)
        {
            List<int> edgeTiles = new List<int>();
            foreach(var tile in TileOrientaions)
            {
                var OtherTopBottoms = TileOrientaions.Where(x => x.Key != tile.Key).Select(x => x.Value).Select(z => z.SelectMany(o =>o.GetRange(0,2).ToList())).ToList();
                var OtherRightLefts = TileOrientaions.Where(x => x.Key != tile.Key).Select(x => x.Value).Select(z => z.SelectMany(o =>o.GetRange(2,2).ToList())).ToList();
            
                foreach(var orientations in tile.Value)
                {
                    var topDoesNotMatch = orientations.GetRange(0,1).Any(x=> !OtherTopBottoms.Any(y => y.Contains(x)));
                    var bottomDoesNotMatch = orientations.GetRange(1,1).Any(x=> !OtherTopBottoms.Any(y => y.Contains(x)));
                    var rightDoesNotMatch = orientations.GetRange(2,1).Any(x=> !OtherRightLefts.Any(y => y.Contains(x)));
                    var leftDoesNotMatch = orientations.GetRange(3,1).Any(x=> !OtherRightLefts.Any(y => y.Contains(x)));

                    if(((topDoesNotMatch ^ bottomDoesNotMatch)&& !rightDoesNotMatch && ! leftDoesNotMatch) ||
                        ((rightDoesNotMatch ^ leftDoesNotMatch) && !bottomDoesNotMatch && !topDoesNotMatch))
                    {
                        edgeTiles.Add(tile.Key);
                        break;
                    }
                }
            }

            var map = new Dictionary<int, Tuple<int,bool,int>>();
            map.Add(0, new Tuple<int, bool, int>(realMap[0].Item1,realMap[0].Item2,realMap[0].Item3));

            var index = 1;
            while(index<=143)
            {
                if((index>=12 && index<132) && index%12 != 0 && index%12 != 11)
                {
                    index++;
                    continue;
                }
                if(index == 11 || index == 132 || index == 143)
                {
                    foreach(var tile in corners.Where(t => !map.Values.Select(x =>x.Item1).Contains(t)))
                    {
                        var prevTileEdges1 = new List<int>();
                        if(index ==11)
                            prevTileEdges1 = GetTileEdges(10, map);
                        else if(index == 132)
                            prevTileEdges1 = GetTileEdges(120, map);
                        else if(index == 143)
                            prevTileEdges1 = GetTileEdges(131, map);

                        var OtherTopBottoms = TileOrientaions.Where(x => x.Key != tile).Select(x => x.Value).Select(z => z.SelectMany(o =>o.GetRange(0,2).ToList())).ToList();
                        var OtherRightLefts = TileOrientaions.Where(x => x.Key != tile).Select(x => x.Value).Select(z => z.SelectMany(o =>o.GetRange(2,2).ToList())).ToList();
                        
                        foreach(var orientations in TileOrientaions[tile])
                        {
                            var topDoesNotMatch = orientations.GetRange(0,1).Any(x=> !OtherTopBottoms.Any(y => y.Contains(x)));
                            var bottomDoesNotMatch = orientations.GetRange(1,1).Any(x=> !OtherTopBottoms.Any(y => y.Contains(x)));
                            var rightDoesNotMatch = orientations.GetRange(2,1).Any(x=> !OtherRightLefts.Any(y => y.Contains(x)));
                            var leftDoesNotMatch = orientations.GetRange(3,1).Any(x=> !OtherRightLefts.Any(y => y.Contains(x)));

                            var leftMatches = orientations[3] == prevTileEdges1[2];
                            var topMatches = orientations[0] == prevTileEdges1[1];

                            if(index == 11)
                            {
                                if(leftMatches && rightDoesNotMatch&& topDoesNotMatch)
                                {
                                    map.Add(11, GetTileTuple(tile, TileOrientaions[tile].IndexOf(orientations)));
                                    break;
                                }
                                continue;
                            }

                            if(index == 132)
                            {
                                if(topMatches && leftDoesNotMatch && bottomDoesNotMatch)
                                {
                                    map.Add(132, GetTileTuple(tile, TileOrientaions[tile].IndexOf(orientations)));
                                    break;
                                }
                                continue;
                            }

                            if(index == 143)
                            {
                                if(topMatches && rightDoesNotMatch && bottomDoesNotMatch)
                                {
                                    map.Add(143, GetTileTuple(tile, TileOrientaions[tile].IndexOf(orientations)));
                                    break;
                                }
                                continue;
                            }                            
                        }

                        if(map.ContainsKey(index))
                            continue;
                    }

                    index++;
                    continue;
                }

                var prevTileEdges = new List<int>();
                if(index < 11 || index/12 == 11)
                    prevTileEdges = GetTileEdges(index-1, map);
                else if(index%12 == 0 || index%12 == 11)
                    prevTileEdges = GetTileEdges(index- 12, map);
                

                var possibleEdges = edgeTiles.Where(t => TileOrientaions[t].Where(x=> x[3] == prevTileEdges[2]).Count() != 0).ToList();
                foreach(var tile in edgeTiles.Where(t => !map.Values.Select(x =>x.Item1).Contains(t)))
                {
                    var OtherTopBottoms = TileOrientaions.Where(x => x.Key != tile).Select(x => x.Value).Select(z => z.SelectMany(o =>o.GetRange(0,2).ToList())).ToList();
                    var OtherRightLefts = TileOrientaions.Where(x => x.Key != tile).Select(x => x.Value).Select(z => z.SelectMany(o =>o.GetRange(2,2).ToList())).ToList();
                
                    foreach(var orientations in TileOrientaions[tile])
                    {
                        var topDoesNotMatch = orientations.GetRange(0,1).Any(x=> !OtherTopBottoms.Any(y => y.Contains(x)));
                        var bottomDoesNotMatch = orientations.GetRange(1,1).Any(x=> !OtherTopBottoms.Any(y => y.Contains(x)));
                        var rightDoesNotMatch = orientations.GetRange(2,1).Any(x=> !OtherRightLefts.Any(y => y.Contains(x)));
                        var leftDoesNotMatch = orientations.GetRange(3,1).Any(x=> !OtherRightLefts.Any(y => y.Contains(x)));

                        var leftMatches = orientations[3] == prevTileEdges[2];
                        var topMatches = orientations[0] == prevTileEdges[1];

                        if(index < 11) 
                        {
                            if(topDoesNotMatch && leftMatches)
                            {
                                map.Add(index, GetTileTuple(tile, TileOrientaions[tile].IndexOf(orientations)));
                                break;
                            }
                            continue;
                        }
                        else if(index /12 == 11)
                        {
                            if(bottomDoesNotMatch && leftMatches)
                            {
                                map.Add(index, GetTileTuple(tile, TileOrientaions[tile].IndexOf(orientations)));
                                break;
                            }
                            continue;
                        }
                        else if(index%12 == 0)
                        {
                            if(topMatches && leftDoesNotMatch)
                            {
                                map.Add(index, GetTileTuple(tile, TileOrientaions[tile].IndexOf(orientations)));
                                break;
                            }
                            continue;
                        }
                        else if(index%12 == 11)
                        {
                            if(topMatches && rightDoesNotMatch)
                            {
                                map.Add(index, GetTileTuple(tile, TileOrientaions[tile].IndexOf(orientations)));
                                break;
                            }
                            continue;
                        }
                    }

                    if(map.ContainsKey(index))
                        break;
                }

                index++;
            }


            return map;

        }

        private Dictionary<int, List<int>> FindDuplicates()
        {
            var matches = new Dictionary<int, List<int>>();

            foreach(var tile in TileOrientaions)
            {
                foreach(var orien in tile.Value)
                {
                    var otherTileOrientations = TileOrientaions.Where(x =>x.Key != tile.Key);

                    foreach(var otherTile in otherTileOrientations)
                    {
                        if(orien.Any(x=> otherTile.Value.Any(y => y.Contains(x))))
                        {
                            if(!matches.ContainsKey(tile.Key))
                                matches.Add(tile.Key, new List<int>(){otherTile.Key});
                            else if(!matches[tile.Key].Contains(otherTile.Key))
                                matches[tile.Key].Add(otherTile.Key);

                            continue;    
                        }
                    }

                }

            }

            return matches;
        }

        private void fillMap()
        {
            for(int i =12; i<131; i++)
            {
                if(i%12 == 11)
                    continue;
                
                FillRight(i);
            }
        }

        private void FillRight(int startingIndex)
        { 
            var tileEdges = GetTileEdges(startingIndex);
            var prevTileEdges = new List<int>();
            if(startingIndex>11)
                prevTileEdges = GetTileEdges(startingIndex-11);


            var test = TileOrientaions.Where(x => !realMap.Select(y=>y.Value.Item1).Contains(x.Key)).Where(x => x.Value.Any(y => y[0] == prevTileEdges[1] )).Select(x => x.Key).ToList();
            var test22 = TileOrientaions.Where(x => !realMap.Select(y=>y.Value.Item1).Contains(x.Key)).Where(x => x.Value.Any(y => y[3]==tileEdges[2] )).Select(x => x.Key).ToList();
            foreach(var otherTile in TileOrientaions.Where(x => !realMap.Select(y=>y.Value.Item1).Contains(x.Key)))
            {
                var OtherTopBottoms = TileOrientaions.Where(x => x.Key != otherTile.Key).Select(x => x.Value).Select(z => z.SelectMany(o =>o.GetRange(0,2).ToList())).ToList();
                var OtherRightLefts = TileOrientaions.Where(x => x.Key != otherTile.Key).Select(x => x.Value).Select(z => z.SelectMany(o =>o.GetRange(2,2).ToList())).ToList();


                foreach(var otherTileOrientation in TileOrientaions[otherTile.Key])
                {
                    var bottomDoesNotMatch = otherTileOrientation.GetRange(1,1).Any(x=> !OtherTopBottoms.Any(y => y.Contains(x)));
                    var rightDoesNotMatch = otherTileOrientation.GetRange(2,1).Any(x=> !OtherRightLefts.Any(y => y.Contains(x)));

                    var topMatches = false;
                    if(startingIndex < 11)
                    {
                        topMatches =  otherTileOrientation.GetRange(0,1).Any(x=> !OtherTopBottoms.Any(y => y.Contains(x)));
                    }
                    else
                    {
                        topMatches = otherTileOrientation[0]== prevTileEdges[1];
                    }

                    var leftMatches = otherTileOrientation[3] == tileEdges[2];

                    bool checkNoMatchRight = startingIndex%12 == 11;
                    bool checkNoMatchBottom = startingIndex >= 12*11;

                    if(topMatches && leftMatches)
                    {
                        if(checkNoMatchBottom && !bottomDoesNotMatch)
                            continue;

                        if(checkNoMatchRight && !rightDoesNotMatch)
                            continue;


                        var index = TileOrientaions[otherTile.Key].IndexOf(otherTileOrientation);
                        var rot = index%4;
                        var flip = index/4;

                        bool flipped = false;
                        if(flip == 1)
                        {
                            flipped = true;
                        }

                            
                        realMap.Add(startingIndex+1,new Tuple<int, bool, int>(otherTile.Key, flipped, rot));
                        return;
                    }
                }
            }
        }

        private Tuple<int, bool, int> GetTileTuple(int index, int orientIndex)
        {
            var rot = orientIndex%4;
            var flip = orientIndex/4;

            var flipped = false;

            if(flip == 1)
            {
                flipped = true;
            }

            return new Tuple<int, bool, int>(index, flipped, rot);
        }

        private void FillBelow(int startingIndex)
        {
            var tileEdges = GetTileEdges(startingIndex);

            foreach(var otherTile in TileOrientaions.Where(x => !realMap.Select(y=>y.Value.Item1).Contains(x.Key)))
            {
                var OtherTopBottoms = TileOrientaions.Where(x => x.Key != otherTile.Key).Select(x => x.Value).Select(z => z.SelectMany(o =>o.GetRange(0,2).ToList())).ToList();
                var OtherRightLefts = TileOrientaions.Where(x => x.Key != otherTile.Key).Select(x => x.Value).Select(z => z.SelectMany(o =>o.GetRange(2,2).ToList())).ToList();

                foreach(var otherTileOrientation in TileOrientaions[otherTile.Key])
                {
                    var leftDoesNotMatch = otherTileOrientation.GetRange(3,1).Any(x=> !OtherRightLefts.Any(y => y.Contains(x)));

                    var topMatches = otherTileOrientation[0]== tileEdges[1];

                    if(topMatches && leftDoesNotMatch)
                    {
                        var index = TileOrientaions[otherTile.Key].IndexOf(otherTileOrientation);
                        var rot = index%4;
                        var flip = index/4;

                        var flipped = false;

                        if(flip == 1)
                        {
                            flipped = true;
                        }

                            
                        realMap.Add(startingIndex+12,new Tuple<int, bool, int>(otherTile.Key, flipped, rot));
                        return;
                    }
                }
            }
        }

        private List<int> GetTileEdges(int tileIndex, Dictionary<int,Tuple<int,bool,int>> map = null)
        {
            if(map == null)
            {
                var tileOrientation = (realMap[tileIndex].Item2 ? 1:0)*4 + realMap[tileIndex].Item3;  
                return TileOrientaions[realMap[tileIndex].Item1][tileOrientation];
            }
            else
            {
                var tileOrientation = (map[tileIndex].Item2 ? 1:0)*4 +  map[tileIndex].Item3;  
                return TileOrientaions[map[tileIndex].Item1][tileOrientation];
            }
        }

        private List<int> findCornerTiles()
        {
            var tileIndexes = new List<int>();
            foreach(var tile in TileOrientaions)
            {
                var OtherTopBottoms = TileOrientaions.Where(x => x.Key != tile.Key).Select(x => x.Value).Select(z => z.SelectMany(o =>o.GetRange(0,2).ToList())).ToList();
                var OtherRightLefts = TileOrientaions.Where(x => x.Key != tile.Key).Select(x => x.Value).Select(z => z.SelectMany(o =>o.GetRange(2,2).ToList())).ToList();
            
                foreach(var orientations in tile.Value)
                {
                    var topDoesNotMatch = orientations.GetRange(0,1).Any(x=> !OtherTopBottoms.Any(y => y.Contains(x)));
                    var bottomDoesNotMatch = orientations.GetRange(1,1).Any(x=> !OtherTopBottoms.Any(y => y.Contains(x)));
                    var rightDoesNotMatch = orientations.GetRange(2,1).Any(x=> !OtherRightLefts.Any(y => y.Contains(x)));
                    var leftDoesNotMatch = orientations.GetRange(3,1).Any(x=> !OtherRightLefts.Any(y => y.Contains(x)));

                    if((topDoesNotMatch ^ bottomDoesNotMatch)&& (rightDoesNotMatch ^ leftDoesNotMatch))
                    {
                        if(topDoesNotMatch&&leftDoesNotMatch && !realMap.Keys.Contains(0))
                        {   
                            var index = tile.Value.IndexOf(orientations);
                            var rot = index%4;
                            var flip = index/4;

                            var flipped = false;

                            if(flip == 1)
                            {
                                flipped = true;
                            }
                            
                            realMap.Add(0,new Tuple<int, bool, int>(tile.Key, flipped, rot));
                        }
                        tileIndexes.Add(tile.Key);
                        break;
                    }
                }
            }

            return tileIndexes;
        }

        public void fillTileOrientations()
        {
            List<List<int>> orientations = new List<List<int>>();
            foreach(var tile in tiles)
            {
                orientations.Clear();
                var topString = tile.Value.First().Replace("#","1").Replace(".","0");
                var bottomString = tile.Value.Last().Replace("#","1").Replace(".","0");
                var topRevString = tile.Value.First().Reverse().Select(x => x.ToString() == "#"?"1":"0").ToList();
                var bottomRevString = tile.Value.Last().Reverse().Select(x => x.ToString() == "#"?"1":"0").ToList();

                var top = Convert.ToInt32(topString,2);
                var bottom = Convert.ToInt32(bottomString,2);
                var topRev = ConvertBinaryStringListToInt(topRevString);
                var bottomRev = ConvertBinaryStringListToInt(bottomRevString);

                var leftString = tile.Value.Select(x => x.Substring(0,1) == "#"? "1":"0").ToList();
                var rightString = tile.Value.Select(x => x.Substring(tile.Value.Count-1) == "#"? "1":"0").ToList();

                var left = ConvertBinaryStringListToInt(leftString);
                var right = ConvertBinaryStringListToInt(rightString);
                var leftReverse = ConvertBinaryStringListToInt(leftString.Select(x=>x).Reverse().ToList());
                var rightReverse = ConvertBinaryStringListToInt(rightString.Select(x=>x).Reverse().ToList());
            
            
                orientations.Add(new List<int>(){top,           bottom,         right,          left});
                orientations.Add(new List<int>(){leftReverse,   rightReverse,   top,            bottom});
                orientations.Add(new List<int>(){bottomRev,     topRev,         leftReverse,    rightReverse});
                orientations.Add(new List<int>(){right,         left,           bottomRev,      topRev});

                orientations.Add(new List<int>(){topRev,         bottomRev,          left,        right});
                orientations.Add(new List<int>(){rightReverse,   leftReverse,        topRev,      bottomRev});
                orientations.Add(new List<int>(){bottom,         top,                rightReverse,  leftReverse});
                orientations.Add(new List<int>(){left,          right,                bottom,       top});

                TileOrientaions.Add(tile.Key, orientations.ToList());
            }
        }

        public List<List<int>> GetRotations(int top, int bottom, int right, int left)
        {
            var ret = new List<List<int>>();

            ret.Add(new List<int>(){top,   bottom, right,  left});
            ret.Add(new List<int>(){left,  right,  top,    bottom});
            ret.Add(new List<int>(){bottom,top,    left,   right});
            ret.Add(new List<int>(){right, left,   bottom, top});

            return ret;

        }

        public int ConvertBinaryStringListToInt(List<string> binaryStringList)
        {
            int num = 0;

            for(int i = 0; i< binaryStringList.Count; i++)
            {
                if(binaryStringList[i] == "0")
                    continue;
                if(binaryStringList[i] == "1")
                    num += (int)Math.Pow(2, binaryStringList.Count-1-i);
            }

            return num;
        }
    }
}
