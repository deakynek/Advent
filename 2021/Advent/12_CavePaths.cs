using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace Advent
{
    class CavePaths
    {
        List<int> entries;
        Dictionary<string, List<string>> paths = new Dictionary<string, List<string>>();

        Regex rx = new Regex(@"[a-z]+");

        public CavePaths(List<string> lines)
        {
            foreach(var line in lines.Select(x=>x.Trim()))
            {
                var parts = line.Split('-').ToList();

                if(paths.Keys.ToList().Contains(parts[0]))
                {
                    paths[parts[0]].Add(parts[1]);
                }
                else
                {
                    paths.Add(parts[0], new List<string>(){parts[1]});
                }

                if(paths.Keys.ToList().Contains(parts[1]))
                {
                    paths[parts[1]].Add(parts[0]);
                }
                else
                {
                    paths.Add(parts[1], new List<string>(){parts[0]});
                }
            }

            Console.WriteLine(RecurseGetPaths(new List<string>(){"start"}));
            
            Console.WriteLine(RecurseGetPathsPart2(new List<string>(){"start"},false));
        }

        private int RecurseGetPaths(List<string> currPath)
        {
            var lastKey = currPath.Last();
            if(lastKey == "end")
                return 1;

            if(!paths.Keys.Contains(lastKey))
                return 0;

            var pathCount = 0;
            foreach(var next in paths[lastKey])
            {
                if(String.IsNullOrEmpty(rx.Match(next).Value) || !currPath.Contains(next))
                {
                    var nextList = currPath.ToList();
                    nextList.Add(next);
                    pathCount += RecurseGetPaths(nextList);
                }
            }

            return pathCount;
        }

       private int RecurseGetPathsPart2(List<string> currPath, bool doubleFound)
        {
            var lastKey = currPath.Last();
            if(lastKey == "end")
                return 1;

            if(!paths.Keys.Contains(lastKey))
                return 0;


            doubleFound = doubleFound || 
                            (!String.IsNullOrEmpty(rx.Match(currPath.Last()).Value) 
                                    &&currPath.Count(x=>x==currPath.Last())>1);
            var pathCount = 0;
            foreach(var next in paths[lastKey])
            {
                if(next != "start" && 
                    (String.IsNullOrEmpty(rx.Match(next).Value) || !currPath.Contains(next)|| !doubleFound))
                {
                    var nextList = currPath.ToList();
                    nextList.Add(next);
                    pathCount += RecurseGetPathsPart2(nextList,doubleFound);
                }
            }

            return pathCount;
        }        
    }    
}