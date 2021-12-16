using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    class Folding
    {
        List<int> entries;
        List<Tuple<string,int>> instructions = new List<Tuple<string, int>>();
        List<Tuple<int,int>> coordinates = new List<Tuple<int, int>>();

        public Folding(List<string> lines)
        {
            var instruction = false;
            foreach(var line in lines.Select(x =>x.Trim()))
            {
                if(String.IsNullOrEmpty(line))
                {
                    instruction = true;
                    continue;
                }

                if(instruction)
                {
                    var words = line.Split(' ');
                    var inst = words.Last().Split('=');
                    instructions.Add(new Tuple<string, int>(inst[0], int.Parse(inst[1])));
                }
                else
                {
                    var coord = line.Split(',');
                    var x = int.Parse(coord[0]);
                    var y = int.Parse(coord[1]);

                    coordinates.Add(new Tuple<int, int>(x,y));
                }
            }



            var flip1 = DoFold(instructions.First().Item1 == "x", instructions.First().Item2, coordinates);

            Console.WriteLine(flip1.Count());

            var final = coordinates.ToList();
            foreach(var inst in instructions)
            {
                final = DoFold(inst.Item1 == "x",inst.Item2,final);
            }

            for(int y = 0; y<=final.Select(g=>g.Item2).Max(); y++)
            {
                string line = "";
                for(int x = 0; x<=final.Select(g=>g.Item1).Max(); x++)
                {
                    if(final.Any(g => g.Item1==x && g.Item2==y))
                    {
                        line+="x";
                    }
                    else
                    {
                        line+=" ";
                    }
                }
                Console.WriteLine(String.Format(line+ "   {0}",y));
            }

        }

        private List<Tuple<int, int>> DoFold(bool isX, int num, List<Tuple<int,int>> coords)
        {
            List<Tuple<int,int>> newMap;

            if(isX)
            {
                newMap = coords.Where(t => t.Item1<= num).ToList();

                foreach(var flipped in coords.Where(t => t.Item1> num).ToList())
                {                    
                    var newX = num*2-flipped.Item1;
                    var newY = flipped.Item2;

                    if(newMap.Any(j => j.Item1==newX && j.Item2==newY))
                        continue;

                    newMap.Add(new Tuple<int, int>(newX,newY));
                }
            }
            else
            {
                newMap = coords.Where(t => t.Item2 <= num).ToList();

                foreach(var flipped in coords.Where(t => t.Item2>num).ToList())
                {
                    var newX = flipped.Item1;
                    var newY = num*2-flipped.Item2;

                    if(newMap.Any(j => j.Item1==newX && j.Item2==newY))
                        continue;

                    newMap.Add(new Tuple<int, int>(newX,newY));
                }
            }

            return newMap;
        }
    }    
}