using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace Advent
{
    class Seating
    {

        List<string> map = new List<string>();
        public Seating(List<string> lines)
        {
            map = lines.ToList();
            PrintMap();
            UpdateMap1();

            map = lines.ToList();
            UpdateMap2();
        }

        public void ChangeIndex(int index)
        {
            var y = index/map[0].Length;
            var x = index%map[0].Length;

            var state = map[y].Substring(x,1);
            if(state == "L")
                state = "#";
            else if(state == "#")
                state = "L";

            map[y] = map[y].Remove(x,1).Insert(x, state);

        }

        public void UpdateMap1()
        {
            var updatedIndexes = new List<int>();
            var change = 1;
            do
            {
                change++;
                updatedIndexes.Clear();
                for(int y = 0; y< map.Count(); y++)
                {
                    for(int x = 0; x< map[y].Count(); x++)
                    {
                        if(DoesChange(x,y))
                        {
                            updatedIndexes.Add(y*map[y].Count() +x);
                        }
                    }
                }

                foreach(var index in updatedIndexes)
                {
                    ChangeIndex(index);
                }

                //PrintMap();
            }
            while(updatedIndexes.Count()>0);

            var occCount = 0;
            foreach(var row in map)
            {
                occCount += row.Count(x=>x=='#');
            }

            Console.WriteLine("Part 1 - seats occupied: "+occCount);
        }

        public void UpdateMap2()
        {
            var updatedIndexes = new List<int>();
            do
            {
                updatedIndexes.Clear();
                for(int y = 0; y< map.Count(); y++)
                {
                    for(int x = 0; x< map[y].Count(); x++)
                    {
                        if(DoesChangeBasedOnLineOfSight(x,y))
                        {
                            updatedIndexes.Add(y*map[y].Count() +x);
                        }
                    }
                }

                foreach(var index in updatedIndexes)
                {
                    ChangeIndex(index);
                }

                //PrintMap();
            }
            while(updatedIndexes.Count()>0);

            var occCount = 0;
            foreach(var row in map)
            {
                occCount += row.Count(x=>x=='#');
            }

            Console.WriteLine("Part 2 - seats occupied: "+occCount);
        }

        public void PrintMap()
        {
            foreach(var row in map)
            {
                Console.WriteLine(row);
            }
        }

        public  bool DoesChangeBasedOnLineOfSight(int x, int y)
        {
            if(map[y][x] == '.')
                return false;

            List<Tuple<int,int>> adjacent = new List<Tuple<int, int>>();
            adjacent.Add(new Tuple<int, int>(-1, -1)); //UL
            adjacent.Add(new Tuple<int, int>(0, -1)); //U
            adjacent.Add(new Tuple<int, int>(1, -1)); //UR
            adjacent.Add(new Tuple<int, int>(1, 0)); //R
            adjacent.Add(new Tuple<int, int>(1, 1)); //BR
            adjacent.Add(new Tuple<int, int>(0, 1)); //B
            adjacent.Add(new Tuple<int, int>(-1, 1)); //BL
            adjacent.Add(new Tuple<int, int>(-1, 0)); //L

            var occupied = 0;
            foreach(var coor in adjacent)
            {
                var newY = y+coor.Item2;
                var newX = x+coor.Item1;
                var found = false;
                while(newY>=0 && newY <map.Count() && newX>=0 && newX < map[newY].Count())
                {
                    if(map[newY][newX] != '.')
                    {
                        found = true;
                        break;
                    }

                    newY += coor.Item2;
                    newX += coor.Item1; 
                }

                if(!found)
                    continue;


                if(map[newY][newX] == '#')
                    occupied++;
            }

            if(map[y][x] == '#' && occupied>=5)
            {
                return true;
            }
            if(map[y][x] == 'L' && occupied == 0)
            {
                return true;
            }

            return false;
        }

        

        public  bool DoesChange(int x, int y)
        {
            if(map[y][x] == '.')
                return false;

            List<Tuple<int,int>> adjacent = new List<Tuple<int, int>>();
            adjacent.Add(new Tuple<int, int>(x-1, y-1)); //UL
            adjacent.Add(new Tuple<int, int>(x, y-1)); //U
            adjacent.Add(new Tuple<int, int>(x+1, y-1)); //UR
            adjacent.Add(new Tuple<int, int>(x+1, y)); //R
            adjacent.Add(new Tuple<int, int>(x+1, y+1)); //BR
            adjacent.Add(new Tuple<int, int>(x, y+1)); //B
            adjacent.Add(new Tuple<int, int>(x-1, y+1)); //BL
            adjacent.Add(new Tuple<int, int>(x-1, y)); //L

            var empty = 0;
            var occupied = 0;
            foreach(var coor in adjacent)
            {
                if(coor.Item2 < 0 || coor.Item2 >= map.Count() ||
                    coor.Item1 < 0 || coor.Item1 >= map[coor.Item2].Count())
                {continue;}

                if(map[coor.Item2][coor.Item1] == '#')
                    occupied++;
                if(map[coor.Item2][coor.Item1] == 'L')
                    empty++;
            }

            if(map[y][x] == '#' && occupied>=4)
            {
                return true;
            }
            if(map[y][x] == 'L' && occupied == 0)
            {
                return true;
            }

            return false;
        }


    }

}