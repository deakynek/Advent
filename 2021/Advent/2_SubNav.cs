using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    class SubNav
    {
        List<int> entries;

        public SubNav(List<string> commands)
        {
            Part1(commands);
            Part2(commands);
        }

        private void Part1(List<string> commands)
        {
            var xPos = 0;
            var depth = 0;
            foreach(string line in commands)
            {
                var pieces = line.Trim().Split(' ');

                switch(pieces[0])
                {
                    case "forward":
                        xPos += int.Parse(pieces[1]);
                        break;
                    case "down":
                        depth += int.Parse(pieces[1]);
                        break;
                    case "up":
                        depth -= int.Parse(pieces[1]);
                        break;
                    default:
                        break;
                }
            }
                
            Console.WriteLine(String.Format("Part 1: \tfinal xPos: {0}\tdepth: {1}\tans: {2}", xPos, depth, xPos*depth));
        }

        private void Part2(List<string> commands)
        {
            var xPos = 0;
            var depth = 0;
            var aim = 0;
            foreach(string line in commands)
            {
                var pieces = line.Trim().Split(' ');

                switch(pieces[0])
                {
                    case "forward":
                        xPos += int.Parse(pieces[1]);
                        depth += aim*int.Parse(pieces[1]);
                        break;
                    case "down":
                        aim += int.Parse(pieces[1]);
                        break;
                    case "up":
                        aim -= int.Parse(pieces[1]);
                        break;
                    default:
                        break;
                }
            }
                
            Console.WriteLine(String.Format("Part 2: \tfinal xPos: {0}\tdepth: {1}\tans: {2}", xPos, depth, xPos*depth));
        }        
    }    
}