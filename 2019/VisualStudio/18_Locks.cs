using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Advent
{
    public class Locks
    {
        List<string> map;
        Dictionary<string,List<string>> keyRequirements = new Dictionary<string, List<string>>();
        
        Dictionary<string,List<string>> keyRequirementsRecursive = new Dictionary<string, List<string>>();
            
        Tuple<int,int> startingPos;
        List<Tuple<int,int>> startingPositions = new List<Tuple<int, int>>();

        Dictionary<string, Tuple<int,int>> allKeys = new Dictionary<string, Tuple<int,int>>();
        int totalTiles;

        Dictionary<string,List<List<string>>> combos = new Dictionary<string, List<List<string>>>();
        Dictionary<string,List<string>> textComboCache = new Dictionary<string, List<string>>();
        Dictionary<string, Dictionary<string,Tuple<int,List<string>>>> stepsFromKeyToKey = new Dictionary<string, Dictionary<string, Tuple<int, List<string>>>>();
        Dictionary<string, Dictionary<string,int>> bottomUpCombos = new Dictionary<string, Dictionary<string, int>>();
        List<int> completedTiles = new List<int>();
        List<int> incompleteTiles = new List<int>();

        public Locks(List<string> inputs)
        {
            map = inputs;
            for(int i = 0; i < inputs.Count; i++)
            {
                for(int j = 0; j < inputs[i].Count(); j++)
                {
                    if(Regex.Match(inputs[i][j].ToString(),@"[a-z]").Success)
                        allKeys.Add(inputs[i][j].ToString(),new Tuple<int, int>(j,i));
                    if(inputs[i][j] == '@')
                    {
                        startingPos = new Tuple<int, int>(j,i);
                    }

                    if(inputs[i][j] != '#')
                        totalTiles++;
                }                
            }
        }

        public void GetAllKeyRequirements(List<string> map)
        {
            var steps = new List<Tuple<int,int,int>>();
            int currDirection = 1;
            var currPos = startingPos;
            List<string> DoorsFound = new List<string>();
            List<string> KeysFound = new List<string>();
            steps.Add(new Tuple<int, int, int>(startingPos.Item1, startingPos.Item2,0));
            bool wallFound = false;
            List<string> keysNeeded = new List<string>();

            while(KeysFound.Count()< allKeys.Count())
            {
                var goingTo = getNewPos(currPos.Item1, currPos.Item2, currDirection);
                var pixel = map[goingTo.Item2][goingTo.Item1].ToString();
                
                if(pixel == "#")
                {
                    wallFound = true;
                    currDirection = DirToTheRIght(currDirection);
                }
                else
                {
                    var newPos = goingTo;
                    if(Regex.Match(pixel,@"[A-Z]").Success)
                    {
                        if(!keysNeeded.Contains(pixel.ToLower()))
                            keysNeeded.Add(pixel.ToLower());
                        else
                            keysNeeded.Remove(pixel.ToLower());
                    }
                    else if(Regex.Match(pixel,@"[a-z]").Success)
                    {
                        if(!keyRequirements.ContainsKey(pixel))
                            keyRequirements.Add(pixel, keysNeeded.ToList());

                         if(!KeysFound.Contains(pixel))
                            KeysFound.Add(pixel);
                    }

                    currPos = newPos;
                    if(wallFound)
                        currDirection = DirToTheLeft(currDirection);
                }
            }
        }

        public void FindMinSteps()
        {
            List<string> keysObtained = new List<string>();
            var minStep = Int32.MaxValue;
            keyRequirements.Clear();
            stepsFromKeyToKey.Clear();
            GetAllKeyRequirements(map);

            var bigMap = true;

            var sortedKeys = allKeys.Keys.OrderBy(x =>x).ToList();
            for(int i =0; i< sortedKeys.Count(); i++)
            {
                Console.WriteLine("Getting Steps for " + sortedKeys[i]);
                var allsteps = GetAvaiableKeys(new List<string>(){sortedKeys[i]}, sortedKeys[i] ,0).ToDictionary(x=> x.Item1, x=> new Tuple<int, List<string>>(x.Item2,x.Item3));
                stepsFromKeyToKey[sortedKeys[i]] = allsteps;
            }
            Console.WriteLine("Getting Steps for @");
            stepsFromKeyToKey["@"] = GetAvaiableKeys(new List<string>(){"@"}, "@",0).ToDictionary(x=> x.Item1, x=> new Tuple<int, List<string>>(x.Item2,x.Item3));
            
            textComboCache.Clear();
            bottomUpCombos.Clear();
            
            var steps = RecurseBottomUpApproach("@", String.Join("",allKeys.Keys.Where(x => x!="@").OrderBy(x=>keyRequirements[x].Count).Reverse().ToList()));

            Console.WriteLine("Minimum Steps: " + steps);


            UpdateMap();
            stepsFromKeyToKey.Clear();
            sortedKeys = allKeys.Keys.OrderBy(x =>x).ToList();
            for(int i =0; i< sortedKeys.Count(); i++)
            {
                Console.WriteLine("Getting Steps for " + sortedKeys[i]);
                var allsteps = GetAvaiableKeys(new List<string>(){sortedKeys[i]}, sortedKeys[i] ,0).ToDictionary(x=> x.Item1, x=> new Tuple<int, List<string>>(x.Item2,x.Item3));
                stepsFromKeyToKey[sortedKeys[i]] = allsteps;
            }
            
            for(int i = 0; i<4; i++)
            {
                Console.WriteLine("Getting Steps for " + i);
                stepsFromKeyToKey[String.Format("{0}",i)] = GetAvaiableKeys(new List<string>(){String.Format("{0}",i)}, i.ToString() ,0).ToDictionary(x=> x.Item1, x=> new Tuple<int, List<string>>(x.Item2,x.Item3));
            }

            textComboCache.Clear();
            bottomUpCombos.Clear();
            var steps4Robot = new List<int?>();
            for (int i = 0; i < 4; i++)
            {
                steps4Robot.Add(RecurseBottomUpApproach(i.ToString(),String.Join("",stepsFromKeyToKey[i.ToString()].Keys.OrderBy(x=>keyRequirements[x].Count).Reverse().ToList())));  
            }

            Console.WriteLine(steps4Robot.Sum().ToString());


            steps4Robot = new List<int?>();
            for (int i = 0; i < 4; i++)
            {
                textComboCache.Clear();
                bottomUpCombos.Clear();                
                steps4Robot.Add(RecurseBottomUp4Robots("0","1","2","3",String.Join("",allKeys.Keys.Where(x => x!="@").OrderBy(x=>keyRequirements[x].Count).Reverse().ToList())));  
            }            
        }

        private int? RecurseBottomUp4Robots(string last0key, 
                                            string last1key, 
                                            string last2key,
                                            string last3key,
                                            string avaiableKeys)
        {
            var lastKey = "";
            if(avaiableKeys.Length == 1)
            {
                if(stepsFromKeyToKey["0"].ContainsKey(avaiableKeys[0].ToString()))
                { lastKey = last0key;}
                else if(stepsFromKeyToKey["1"].ContainsKey(avaiableKeys[0].ToString()))
                { lastKey = last1key; }
                else if(stepsFromKeyToKey["2"].ContainsKey(avaiableKeys[0].ToString()))
                { lastKey = last2key; }
                else if(stepsFromKeyToKey["3"].ContainsKey(avaiableKeys[0].ToString()))
                { lastKey = last3key; }

                if(!bottomUpCombos.ContainsKey(lastKey))
                    bottomUpCombos.Add(lastKey, new Dictionary<string, int>());
                
                if(!bottomUpCombos[lastKey].ContainsKey(avaiableKeys))
                    bottomUpCombos[lastKey].Add(avaiableKeys, GetSteps(lastKey+avaiableKeys[0]));

                return bottomUpCombos[lastKey][avaiableKeys];
            }

            List<int> thisSteps  = new List<int>();
            for(int i =0; i<avaiableKeys.Count(); i++)
            {
                var nextKey = avaiableKeys.Substring(i,1);
                if(keyRequirements[nextKey].Any(x=> avaiableKeys.Remove(i,1).Contains(x)))
                    continue;
                
                
                string keyJustBefore = "";
                var section = 0;
                if(stepsFromKeyToKey["0"].ContainsKey(nextKey))
                { 
                    keyJustBefore = last0key;
                    section = 0;
                }
                else if(stepsFromKeyToKey["1"].ContainsKey(nextKey))
                {
                    keyJustBefore = last1key; 
                    section = 1;
                }
                else if(stepsFromKeyToKey["2"].ContainsKey(nextKey))
                {
                    keyJustBefore = last2key; 
                    section = 2;
                }
                else if(stepsFromKeyToKey["3"].ContainsKey(nextKey))
                { 
                    keyJustBefore = last3key; 
                    section = 3;
                }


                string keysCopy = avaiableKeys.Remove(i,1);
                var frees = GetKeysForFrees(keyJustBefore,nextKey);
                foreach(var key in frees)
                {
                    if(keysCopy.Contains(key))
                        keysCopy = keysCopy.Replace(key, "");
                }
 


                int? prevSteps = 0;
                if(keysCopy != "")
                {
                    switch(section)
                    {
                        case 0:
                            prevSteps = RecurseBottomUp4Robots(nextKey,last1key,last2key,last3key,keysCopy);
                            break;
                        case 1:
                            prevSteps = RecurseBottomUp4Robots(last0key,nextKey,last2key,last3key,keysCopy);
                            break;
                        case 2:
                            prevSteps = RecurseBottomUp4Robots(last0key,last1key,nextKey,last3key,keysCopy);
                            break;
                        case 3:
                            prevSteps = RecurseBottomUp4Robots(last0key,last1key,last2key,nextKey,keysCopy);
                            break;
                    }
                }
                if(prevSteps == null)
                    continue;

                var steps = prevSteps.Value+GetSteps(keyJustBefore+nextKey);
                thisSteps.Add(steps);

                if(!bottomUpCombos.ContainsKey(keyJustBefore))
                    bottomUpCombos.Add(keyJustBefore, new Dictionary<string, int>());
                if(!bottomUpCombos[keyJustBefore].ContainsKey(avaiableKeys))
                    bottomUpCombos[keyJustBefore].Add(avaiableKeys,int.MaxValue);
                if(bottomUpCombos[keyJustBefore][avaiableKeys] > steps)
                    bottomUpCombos[keyJustBefore][avaiableKeys] = steps;
            }
            if(thisSteps.Count() == 0)
                return null;

            return thisSteps.Min();
        }


        private void UpdateMap()
        {
            startingPositions.Clear();
            map[startingPos.Item2] = map[startingPos.Item2].Replace('@','#');
            map[startingPos.Item2] = map[startingPos.Item2].Replace('.','#');
            
            map[startingPos.Item2-1] = map[startingPos.Item2-1].Remove(startingPos.Item1-1,3);
            map[startingPos.Item2-1] = map[startingPos.Item2-1].Insert(startingPos.Item1-1,"0#1");

            map[startingPos.Item2+1] = map[startingPos.Item2+1].Remove(startingPos.Item1-1,3);
            map[startingPos.Item2+1] = map[startingPos.Item2+1].Insert(startingPos.Item1-1,"2#3");

            startingPositions.Add(new Tuple<int,int>(startingPos.Item1-1,startingPos.Item2-1));
            startingPositions.Add(new Tuple<int,int>(startingPos.Item1+1,startingPos.Item2-1));
            startingPositions.Add(new Tuple<int,int>(startingPos.Item1-1,startingPos.Item2+1));
            startingPositions.Add(new Tuple<int,int>(startingPos.Item1+1,startingPos.Item2+1));
         }

        private int? RecurseBottomUpApproach(string currentKey, string avaiableKeys)
        {
            if(bottomUpCombos.ContainsKey(currentKey) && bottomUpCombos[currentKey].ContainsKey(avaiableKeys))
                return bottomUpCombos[currentKey][avaiableKeys];

            if(avaiableKeys.Count() == 1 && !bottomUpCombos.ContainsKey(currentKey))
            {
                bottomUpCombos.Add(currentKey, new Dictionary<string, int>());
                bottomUpCombos[currentKey].Add(avaiableKeys, GetSteps(currentKey+avaiableKeys[0]));
                return bottomUpCombos[currentKey][avaiableKeys];
            }

            List<int> thisSteps  = new List<int>();
            for(int i =0; i<avaiableKeys.Count(); i++)
            {
                var nextKey = avaiableKeys.Substring(i,1);
                if(keyRequirements[nextKey].Any(x=> avaiableKeys.Remove(i,1).Contains(x)))
                    continue;
                
                string keysCopy = avaiableKeys.Remove(i,1);
                var frees = GetKeysForFrees(currentKey,nextKey);
                foreach(var key in frees)
                {
                    if(keysCopy.Contains(key))
                        keysCopy = keysCopy.Replace(key, "");
                }
 
                int? prevSteps = 0;
                if(keysCopy != "")
                    prevSteps = RecurseBottomUpApproach(nextKey,keysCopy);
                if(prevSteps == null)
                    continue;

                thisSteps.Add(prevSteps.Value+GetSteps(currentKey+nextKey));
            }
            if(thisSteps.Count() == 0)
                return null;

            if(!bottomUpCombos.ContainsKey(currentKey))
            {
                bottomUpCombos.Add(currentKey, new Dictionary<string, int>());
            }

            bottomUpCombos[currentKey].Add(avaiableKeys, thisSteps.Min());
            return bottomUpCombos[currentKey][avaiableKeys];
        }

        private int GetSteps(string keySeq)
        {
            var keysForFree = new List<string>();
            var thisStep = 0;
            for(int i =1; i<keySeq.Count(); i++)
            {
                var key1 = keySeq.Substring(i-1,1);
                var key2 = keySeq.Substring(i,1);

                thisStep+= stepsFromKeyToKey[key1][key2].Item1;
            }

            return thisStep;
        }

        private List<string> GetKeysForFrees(string key1, string key2)
        {
            return stepsFromKeyToKey[key1][key2].Item2;
        }

        public List<Tuple<string,int,List<string>>> GetAvaiableKeys(List<string> keysObtained,string startingKey, int currentSteps)
        {
            Tuple<int,int> beginHere;

            if(startingKey =="@")
                beginHere = startingPos;
            else if(startingKey =="0")
                beginHere = startingPositions[0];
            else if(startingKey =="1")
                beginHere = startingPositions[1];
            else if(startingKey =="2")
                beginHere = startingPositions[2];
            else if(startingKey =="3")
                beginHere = startingPositions[3];
            else
                beginHere= allKeys[startingKey];

            var currPos = new Tuple<int,int>(beginHere.Item1, beginHere.Item2);
            int currDirection = 1;
            List<Tuple<int,int,int,List<string>>> thisSteps = new List<Tuple<int, int, int, List<string>>>(){new Tuple<int, int, int,List<string>>(currPos.Item1, currPos.Item2, currentSteps, new List<string>())};

            List<Tuple<string,int,List<string>>> keysFound = new List<Tuple<string, int,List<string>>>();
            completedTiles.Clear();
            incompleteTiles.Clear();
            incompleteTiles.Add(map[beginHere.Item2].Count()*beginHere.Item2 + beginHere.Item1);

            while(incompleteTiles.Any())
            {
                var goingTo = getNewPos(currPos.Item1, currPos.Item2, currDirection);
                var pixel = map[goingTo.Item2][goingTo.Item1].ToString();
                
                if(pixel != "#")
                {
                    var newPos = goingTo;

                    var pixIndex = goingTo.Item1+map[0].Count()*goingTo.Item2;
                    if(!completedTiles.Contains(pixIndex) && !incompleteTiles.Contains(pixIndex))
                        incompleteTiles.Add(pixIndex);
                    if(!thisSteps.Any(tile => tile.Item1 == newPos.Item1 && tile.Item2 == newPos.Item2))
                    {
                        var posSteps1 = thisSteps.FirstOrDefault(tile => tile.Item1 == currPos.Item1 && tile.Item2 == currPos.Item2);
                        if(posSteps1 != null)
                        {
                            var currentKeys = posSteps1.Item4.ToList();
                            
                            if(Regex.Match(pixel, @"[a-z]").Success)
                            {
                                if(!keysObtained.Any(p => p==pixel))
                                {
                                    keysFound.Add(new Tuple<string,int,List<string>>(pixel, posSteps1.Item3+1,currentKeys));
                                }
                                currentKeys.Add(pixel);
                            }

                            thisSteps.Add(new Tuple<int, int, int,List<string>>(newPos.Item1, newPos.Item2, posSteps1.Item3+1,currentKeys));
                        }
                    }
                }

                var temp = GetFirstIncompleteTile();
                currPos = new Tuple<int, int>(temp.Item1, temp.Item2);
                currDirection  = temp.Item3;

            }
            return keysFound;
        }

        public Tuple<int,int,int> GetFirstIncompleteTile()
        {
            var copy = incompleteTiles.ToList();
            var width = map[0].Count();
            var neighbors = new List<int>();
            foreach(var tileIndex in copy)
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
                    if(map[row][col] != '#' && !completedTiles.Contains(neighborIndex)&&!incompleteTiles.Contains(neighborIndex))
                    {
                        return new Tuple<int, int, int>(tileIndex%width,tileIndex/width,i+1);
                    }

                }
                completedTiles.Add(tileIndex);
                incompleteTiles.Remove(tileIndex);
            }
            

            return new Tuple<int, int, int>(0, 0, 0);
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