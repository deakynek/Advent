using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    class Temp20
    {
        string lookup = "";
        List<string> map = new List<string>();

        public Temp20(List<string> lines)
        {
            lookup = lines[0];

            for(int i = 2; i<lines.Count; i++)
            {
                map.Add(lines[i]);
            }

            var part1 = map.ToList();
            for(int i = 0; i<2; i++)
            {
                part1 = Enhance(part1, i);
            }

            var tot = 0;
            foreach(var line in part1)
            {
                tot += line.Count(x=>x=='#');
            }
            Console.WriteLine(tot);

            var part2 = map.ToList();
            for(int i = 0; i<50; i++)
            {
                part2 = Enhance(part2, i);
            }

            tot = 0;
            foreach(var line in part2)
            {
                tot += line.Count(x=>x=='#');
            }
            Console.WriteLine(tot);
        }

        private List<string> Enhance(List<string> input, int mult)
        {
            var exChar = ".";
            if(mult%2 == 1)
                exChar = "#";

            var edge = "";
            for(int i = 0; i< input[0].Length; i++)
            {
                edge += exChar;
            }

            var map = input.ToList();
            map.Add(edge);
            map.Reverse();
            map.Add(edge);
            map.Reverse();

            map = map.Select(x=> exChar+x+exChar).ToList();
            var newMap = map.ToList();
            var offsets = new List<int>(){-1,0,1};

            for(int y=0; y<map.Count();y++)
            {
                var newString = "";
                for(int x=0; x<map[0].Length; x++)
                {
                    string binary = "";
                    foreach(var yAdd in offsets)
                    {
                        foreach(var xAdd in offsets)
                        {
                            if(yAdd+y < 0 || yAdd+y >map.Count()-1 ||
                                xAdd+x<0 || xAdd+x > map[0].Length-1)
                            {
                                if(mult%2 == 0)
                                    binary+="0";
                                else
                                    binary+="1";
                            }
                            else
                            {
                                if(map[yAdd+y][xAdd+x]=='.')
                                    binary+="0";
                                else
                                    binary+="1";
                            }
                        }
                    }

                    var num = Convert.ToInt32(binary, 2);
                    newString += lookup[num].ToString();
                }
                newMap[y] = newString;
            }

            //Console.WriteLine();
            //foreach(var line in newMap)
            //    Console.WriteLine(line);

            return newMap;
        }

    }    
}