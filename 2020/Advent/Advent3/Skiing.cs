using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    class Skiing
    {
        List<List<string>> map = new List<List<string>>();

        public Skiing(List<string> lines)
        {
            
            foreach(var line in lines)
            {
                map.Add(line.Select(x => x.ToString()).ToList<string>());
            }

        }

        private int TraverseMapCountTrees(int rightStep, int downStep)
        {
            int x = 0;
            int y = 0;
            int treeCount = 0;

            while(y < map.Count)
            {
                if(x >= map[y].Count)
                    x -= map[y].Count;

                if(map[y][x] == "#")
                {
                    treeCount++;
                }

                x+=rightStep;
                y+=downStep;
            }

            return treeCount;

        }

        public int Part1_GetAndPrintTrees(int rightStep, int downStep)
        {
            int trees = TraverseMapCountTrees(rightStep,downStep);
            Console.WriteLine(String.Format("Trees encountered going right {0}, down {1} : {2}", rightStep, downStep, trees));
            return trees;
        }

        public void Part2_PrintTreeCount()
        {
            List<int> xs = new  List<int>(){1,3,5,7,1};
            List<int> ys = new  List<int>(){1,1,1,1,2};

            var multTrees = 1;
            for(int ind = 0; ind< xs.Count; ind++)
            {
                multTrees *= Part1_GetAndPrintTrees(xs[ind],ys[ind]);
            }

            Console.WriteLine(String.Format("Trees encountered and multiplied : {0}", multTrees));
        }
    }    
}