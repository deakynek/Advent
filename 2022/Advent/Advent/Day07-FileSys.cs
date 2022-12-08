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

                    if (parsedIn.ElementAt(1) == "cd")
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
                    else if (parsedIn.ElementAt(1) == "ls")
                    {
                        lsOp = true;
                        continue;
                    }
                }

                if (!lsOp)
                    continue;

                if (parsedIn.First() == "dir")
                {
                    if (!dirs.ContainsKey(parsedIn.ElementAt(1)))
                    {
                        blank = new Tuple<List<string>, List<long>>(new List<string>(), new List<long>());
                        var copy = currDirs.ToList();
                        copy.Add(parsedIn.ElementAt(1));
                        dirs.Add(String.Join(' ',copy), blank);
                    }
                    dirs[String.Join(' ',currDirs)].Item1.Add(parsedIn.ElementAt(1));
                }
                else
                {
                    dirs[String.Join(' ', currDirs)].Item2.Add(Int64.Parse(parsedIn.ElementAt(0)));
                }
            }

            var size = new Dictionary<string, long>();
            FillSize(size);
            var sum = size.Where(x => x.Value <= 100000).Sum(x => x.Value);

            Console.WriteLine(String.Format("Part 1: Tot of all dirs <= 100000 \t= {0}", sum));

            var remSize = 30000000 - (70000000 - size["/"]);
            var delSize = size.Values.Where(x => x >= remSize).OrderBy(x => x).First();
            Console.WriteLine(String.Format("Part 2: Size of smallest dir to delete \t= {0}", delSize));
        }


        public void FillSize(Dictionary<string, long> size)
        {
            foreach(var dir in dirs.Keys.OrderByDescending(x=>x.Length))
            {
                long memSize= dirs[dir].Item2.Sum();

                foreach(var d in dirs[dir].Item1)
                {
                    memSize += size[dir+" "+d];
                }
                size.Add(dir, memSize);
            }

        }
    }
}
