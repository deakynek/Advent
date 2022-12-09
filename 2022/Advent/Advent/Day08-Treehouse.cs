using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent
{
    internal class TreeHouse
    {
        List<List<int>> treeMap = new List<List<int>>();
        public TreeHouse(List<string> input)
        {
            var visible = new List<List<bool>>();
            foreach (var line in input)
            {
                treeMap.Add(new List<int>());
                foreach (var tree in line)
                {
                    treeMap.Last().Add(Int32.Parse(tree.ToString()));
                }
            }

            var width = treeMap[0].Count();
            Dictionary<int,bool> counter= new Dictionary<int,bool>(); ;
            for(int i = 0; i<treeMap.Count(); i++)
            {
                var max = -1;
                var line = treeMap[i];
                for(int j = 0; j<line.Count(); j++) 
                {
                    if (line[j] > max)
                    {
                        max = line[j];
                        if(!counter.ContainsKey(i*width+j))
                            counter.Add(i*width+j, true);
                    }
                }
                max = -1;
                for (int j = line.Count()-1; j>=0; j--)
                {
                    if (line[j] > max)
                    {
                        max = line[j];
                        if (!counter.ContainsKey(i * width + j))
                            counter.Add(i * width + j, true);
                    }
                }
            }

            for (int j = 0; j < treeMap[0].Count(); j++)
            {
                var max = -1;
                var line = treeMap.Select(x=>x.ElementAt(j)).ToList();
                for (int i = 0; i < line.Count(); i++)
                {
                    if (line[i] > max)
                    {
                        max = line[i];
                        if (!counter.ContainsKey(i * width + j))
                            counter.Add(i * width + j, true);
                    }
                }
                max = -1;
                for (int i = line.Count() - 1; i >= 0; i--)
                {
                    if (line[i] > max)
                    {
                        max = line[i];
                        if (!counter.ContainsKey(i * width + j))
                            counter.Add(i * width + j, true);
                    }
                }
            }

            Console.WriteLine(String.Format("Part 1: Visible Trees {0}", counter.Keys.Count()));

            var maxScenic = 0;
            for(int i = 0; i < treeMap.Count();i++) 
            {
                var row = treeMap[i];
                for (int j = 0; j < row.Count(); j++) 
                {
                    var col = treeMap.Select(x => x.ElementAt(j)).ToList();
                    var val = treeMap[i][j];
                    var upSc = 0;
                    for(int k=i-1; k>=0; k--)
                    {
                        upSc++;
                        if (treeMap[k][j] >= val)
                            break;
                    }

                    var downSc = 0;
                    for (int k = i + 1; k < col.Count(); k++)
                    {
                        downSc++;
                        if (treeMap[k][j] >= val)
                            break;
                    }

                    var rightSc = 0;
                    for (int k = j+1; k <row.Count(); k++)
                    {
                        rightSc++;
                        if (treeMap[i][k] >= val)
                            break;
                    }

                    var leftSc = 0;
                    for (int k = j-1; k >= 0; k--)
                    {
                        leftSc++;
                        if (treeMap[i][k] >= val)
                            break;
                    }

                    var score = upSc * downSc * rightSc * leftSc;

                    if(score> maxScenic)
                        maxScenic= score;
                }
            }

            Console.WriteLine(String.Format("Part 2: Max Scenic Score {0}", maxScenic));

        }
    }
}
