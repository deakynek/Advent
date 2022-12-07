using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Advent
{
    internal class FileSys
    {
        Dictionary<string, Tuple<List<string>, List<Int64>>> dirs = new Dictionary<string, Tuple<List<string>, List<long>>>();
            
        public FileSys(List<string> inputs) 
        {
            bool lsOp = false;
            List<string> currDirs = new List<string>();
            currDirs.Add("/");
            
            var blank = new Tuple<List<string>, List<long>>(new List<string>(), new List<long>());
            dirs.Add(currDirs.First(), blank);


            for (int i = 0; i < inputs.Count; i++) 
            {
                var parsedIn = inputs[i].Split(' ').ToList();

                if (parsedIn.First() == "$")
                {
                    lsOp = false;

                    if(parsedIn.ElementAt(1) == "cd")
                    {
                        if (parsedIn.ElementAt(2) == "/")
                        {
                            currDirs = currDirs.Take(1).ToList();
                            continue;
                        }
                        else if (parsedIn.ElementAt(2) == "..")
                        {
                            currDirs = currDirs.Take(currDirs.Count() - 1).ToList();
                            if (!currDirs.Any())
                                currDirs.Add("/");
                            continue;
                        }

                        currDirs.Add(parsedIn.ElementAt(2));
                        continue;
                    }
                    else if(parsedIn.ElementAt(1) == "ls")
                    {
                        lsOp= true;
                        continue;
                    }
                }

                if (!lsOp)
                    continue;
                
                if(parsedIn.First() == "dir")
                {
                    if (!dirs.ContainsKey(parsedIn.ElementAt(1)))
                    {
                        blank = new Tuple<List<string>, List<long>>(new List<string>(), new List<long>());
                        dirs.Add(parsedIn.ElementAt(1), blank);
                    }
                    dirs[currDirs.Last()].Item1.Add(parsedIn.ElementAt(1));
                }
                else
                {
                    dirs[currDirs.Last()].Item2.Add(Int64.Parse(parsedIn.ElementAt(0)));
                }
            }

            var size = new Dictionary<string, long>();
            RecurseGetDirSum("/", size);
            var sum = size.Where(x => x.Value <= 100000).Sum(x=>x.Value);

            Console.WriteLine(String.Format("Part 1: Tot of all dirs <= 100000 {0}",sum));
        }

        public void RecurseGetDirSum(string currDir,Dictionary<string, long> size)
        {
            if (!dirs.ContainsKey(currDir)||size.ContainsKey(currDir))
                return;


            var currDirBase = dirs[currDir].Item2.Sum();
            long other = 0;
            foreach(var dir in dirs[currDir].Item1)
            {
                if (size.ContainsKey(dir))
                    other += size[dir];
                else
                { 
                    RecurseGetDirSum(dir, size);
                    other += size[dir];
                }
            }

            size.Add(currDir, currDirBase + other);
        }
    }
}
