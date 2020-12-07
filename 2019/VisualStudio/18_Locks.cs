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
        Dictionary<string, Tuple<int,int>> allKeys = new Dictionary<string, Tuple<int,int>>();
        int totalTiles;

        Dictionary<string,List<List<string>>> combos = new Dictionary<string, List<List<string>>>();
        Dictionary<string, Dictionary<string,int>> stepsFromKeyToKey = new Dictionary<string, Dictionary<string, int>>();

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

                        if(keysNeeded.Contains(pixel.ToLower()))
                            keysNeeded.Remove(pixel.ToLower());
                        else if(!CurrentPosACorner(newPos.Item1,newPos.Item2))
                            keysNeeded.Add(pixel.ToLower());

                            
                    }

                    currPos = newPos;
                    if(wallFound)
                        currDirection = DirToTheLeft(currDirection);
                }
            }
        }

        private void PrintKeyRequirements()
        {
            foreach(var key in keyRequirements.Keys)
            {
                var allReq = RecurseGetNumberOfKeyRequirements(key);

                Console.WriteLine(String.Format("Key {0} requires {1} other keys",key, allReq.Count()));
                Console.WriteLine(String.Join(", ",allReq.ToArray()));

                var usedCount = keyRequirements.Keys.Where(x => RecurseGetNumberOfKeyRequirements(x).Contains(key)).Count();
                Console.WriteLine(String.Format("{0} other keys require Key {1}",usedCount, key));
            }

        }

        private List<List<string>> GetAllValidCombos()
        {
            combos.Clear();
            return RecurseGetAllCombos(allKeys.Keys.OrderBy(x => keyRequirementsRecursive[x].Count()).Reverse().ToList());
        }

        private List<List<string>> RecurseGetAllCombos(List<string> remainingKeys)
        {
            List<List<string>> allCombos = new List<List<string>>();

            if(remainingKeys.Count() == 1)
            {
                allCombos.Add(remainingKeys);
                return allCombos;
            }


            var orderedAlphabetically = String.Join("",remainingKeys.OrderBy(x=>x).ToArray());
            if(combos.ContainsKey(orderedAlphabetically))
            {
                return combos[orderedAlphabetically];
            }

            foreach(var key in remainingKeys)
            {
                var copy = remainingKeys.ToList();
                copy.Remove(key);
                if(copy.Any(x=> keyRequirementsRecursive[key].Contains(x)))
                    continue;

                var allRemainingCombos = RecurseGetAllCombos(copy);
                if(allRemainingCombos.Count() == 0)
                    continue;


                foreach(var combo in allRemainingCombos)
                {
                    var comboCopy = combo.ToList();
                    comboCopy.Insert(0,key);
                    allCombos.Add(comboCopy);
                }
            }
            Console.WriteLine("Returning from level " + remainingKeys.Count());

            combos.Add(orderedAlphabetically,allCombos);
            return allCombos;
        }

        public void FindMinSteps()
        {
            List<string> keysObtained = new List<string>();
            var minStep = Int32.MaxValue;
            keyRequirements.Clear();
            stepsFromKeyToKey.Clear();
            GetAllKeyRequirements(map);

            foreach(var key in allKeys.Keys)
            {
                Console.WriteLine("Getting Steps for " + key);
                var allsteps = GetAvaiableKeys(new List<string>(){key}, key,0,false).ToDictionary(x=> x.Item1, x=>x.Item2);
                stepsFromKeyToKey[key] = allsteps;
            }
            Console.WriteLine("Getting Steps for @");
            stepsFromKeyToKey["@"] = GetAvaiableKeys(new List<string>(){"@"}, "@",0,false).ToDictionary(x=> x.Item1, x=>x.Item2);

            

            
            var AllValidSteps = RecurseGetSteps(new List<string>(),"@");

            Console.WriteLine("Minimum Steps: " + AllValidSteps.Min());
        }

        private List<int> RecurseGetSteps(List<string> keysObtained, string fromKey)
        {
            if(keysObtained.Count() == allKeys.Count()-1)
            {
                var missingKey = allKeys.Keys.First(x => !keysObtained.Contains(x));
                return new List<int>(){stepsFromKeyToKey[fromKey][missingKey]};
            }

            var availableKeys = keyRequirements.Where(key =>!keysObtained.Contains(key.Key) &&
                                                                !key.Value.Any(req => !keysObtained.Contains(req)))
                                                .Select(key => key.Key);
            var possibleSteps = new List<int>();
            foreach(var nextKey in availableKeys)
            {
                var keysObtainedCopy = keysObtained.ToList();
                keysObtainedCopy.Add(nextKey);
                possibleSteps.AddRange( RecurseGetSteps(keysObtainedCopy, nextKey).Select(x => x+stepsFromKeyToKey[fromKey][nextKey]));
            }

            return possibleSteps;
        }

        private List<string> GetDoorList()
        {
            var doors = new List<string>();
            foreach(var row in map)
            {
                foreach(var tile in row)
                {
                    if(Regex.Match(tile.ToString(),@"[A-Z]").Success)
                        doors.Add(tile.ToString());
                }
            }
            return doors;
        }

        private List<string> RecurseGetNumberOfKeyRequirements(string key)
        {
            List<string> thisKeyReq = keyRequirements[key].ToList();
            foreach(var prevKey in keyRequirements[key])
            {
                thisKeyReq.AddRange(RecurseGetNumberOfKeyRequirements(prevKey));
            }

            return thisKeyReq.Distinct().ToList();
        }

        public bool CurrentPosACorner(int x, int y)
        {
            return (map[y-1][x] == '#' &&
                    map[y][x+1] == '#' &&
                    map[y][x-1] == '#') ||

                (map[y+1][x] == '#' &&
                    map[y][x+1] == '#' &&
                    map[y][x-1] == '#') ||

                (map[y][x-1] == '#' &&
                    map[y-1][x] == '#' &&
                    map[y+1][x] == '#') ||

                (map[y][x+1] == '#' &&
                        map[y-1][x] == '#' &&
                        map[y+1][x] == '#');
        }

 

        public List<Tuple<string,int>> GetAvaiableKeys(List<string> keysObtained,string startingKey, int currentSteps, bool stopAtBlockages)
        {
            Tuple<int,int> beginHere;

            if(startingKey =="@")
                beginHere = startingPos;
            else
                beginHere= allKeys[startingKey];

            var currPos = new Tuple<int,int>(beginHere.Item1, beginHere.Item2);
            int currDirection = 1;
            List<Tuple<int,int,int>> thisSteps = new List<Tuple<int, int, int>>(){new Tuple<int, int, int>(currPos.Item1, currPos.Item2, currentSteps)};
            bool wallFound = false;
            var visitedStartCount =0;
            List<Tuple<string,int>> keysFound = new List<Tuple<string, int>>();

            while(stopAtBlockages? visitedStartCount <= 4: thisSteps.Count()<totalTiles)
            {
                var goingTo = getNewPos(currPos.Item1, currPos.Item2, currDirection);
                var pixel = map[goingTo.Item2][goingTo.Item1].ToString();
                
                var treatThisAsWall = stopAtBlockages && 
                                        (Regex.Match(pixel, @"[a-z]").Success && 
                                            !keysObtained.Contains(pixel) 
                                        || Regex.Match(pixel, @"[A-Z]").Success && 
                                            !keysObtained.Contains(pixel.ToLower()));

                if(pixel == "#" || treatThisAsWall)
                {
                    wallFound = true;
                    currDirection = DirToTheRIght(currDirection);

                    if(treatThisAsWall && Regex.Match(pixel, @"[a-z]").Success)
                    {
                        var posSteps1 = thisSteps.FirstOrDefault(tile => tile.Item1 == currPos.Item1 && tile.Item2 == currPos.Item2)?.Item3;
                        if(posSteps1 != null)
                            keysFound.Add(new Tuple<string,int>(pixel, posSteps1.Value+1));
                    }

                }
                else
                {
                    var newPos = goingTo;

                    if(!thisSteps.Any(tile => tile.Item1 == newPos.Item1 && tile.Item2 == newPos.Item2))
                    {
                        var posSteps1 = thisSteps.FirstOrDefault(tile => tile.Item1 == currPos.Item1 && tile.Item2 == currPos.Item2)?.Item3;
                        if(posSteps1 != null)
                        {
                            thisSteps.Add(new Tuple<int, int, int>(newPos.Item1, newPos.Item2, posSteps1.Value+1));

                            if(Regex.Match(pixel, @"[a-z]").Success)
                            {
                                keysFound.Add(new Tuple<string,int>(pixel, posSteps1.Value+1));
                            }
                        }
                    }

                    currPos = newPos;
                    if(wallFound)
                        currDirection = DirToTheLeft(currDirection);
                }

                if(stopAtBlockages&& currPos.Item1 == beginHere.Item1 && currPos.Item2 == beginHere.Item2)
                    visitedStartCount++;
            }
            return keysFound;
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
                    currY += 1;
                    break;
                case 2: //south
                    currY -= 1;
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