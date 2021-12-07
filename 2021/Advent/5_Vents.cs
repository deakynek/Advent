using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    class Vents
    {
        List<int> entries;
        Dictionary<string,string> ventLines = new Dictionary<string, string>();
        Dictionary<string, int> spaces = new Dictionary<string, int>();
        public Vents(List<string> lines)
        {
            foreach(var line in lines)
            {
                var parts = line.Trim().Split("->");
                var part1 = parts[0].Trim();
                var part2 = parts[1].Trim();

                ventLines.Add(part1, part2);
            }

            calcLines(true);
            calcLines(false);
        }

        private void calcLines(bool skipDiagonals)
        {
            spaces.Clear();

            foreach(var entry in ventLines)
            {
                var startX = int.Parse(entry.Key.Split(',')[0]);
                var startY = int.Parse(entry.Key.Split(',')[1]);
                var endX = int.Parse(entry.Value.Split(',')[0]);
                var endY = int.Parse(entry.Value.Split(',')[1]);





                if(skipDiagonals && startX!= endX && startY!= endY)
                    continue;


                int x = startX;
                int y = startY;

                while(x != endX || y!= endY)
                {
                    string thisSquare = String.Format("{0},{1}",x,y);
                    if(spaces.ContainsKey(thisSquare))
                    {
                        spaces[thisSquare] += 1;
                    }
                    else
                    {
                        spaces.Add(thisSquare,1);
                    }

                    if(x != endX)
                    {
                        if(endX > startX)
                            x++;
                        else
                            x--;
                    }
                    if(y != endY)
                    {
                        if(endY > startY)
                            y++;
                        else
                            y--;
                    }
                }
                string final = String.Format("{0},{1}",endX,endY);
                if(spaces.ContainsKey(final))
                {
                    spaces[final] += 1;
                }
                else
                {
                    spaces.Add(final,1);
                }
            }

            var cnt = spaces.Values.Where(x => x>=2).Count();
            Console.WriteLine(String.Format("squares with at least one overlap {0}", cnt));
        }
    }    
}