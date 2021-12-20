using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    class SmokeBasin
    {
        List<string> map;   
        Dictionary<Tuple<int,int>,bool> lowPoints = new Dictionary<Tuple<int, int>, bool>();

        public SmokeBasin(List<string> lines)
        {
            map = lines.Select(X=> X.Replace('9',' ')).ToList();
            
            List<square> squares = new List<square>();
            int latestIdent = 1;
            squares.Add(new square(0,0,latestIdent));
            var non9 = lines.Select(x=>x.Count(y=>y!='9')).Sum();
            List<int> areaSize = new List<int>(){0};
            int ypos = 0;
            int xpos = 0;
            lowPoints = part1(lines);

            while(lowPoints.Values.Any(x=>!x) || !squares.All(x=>x.AllComplete()))
            {
                var thissq = squares.FirstOrDefault(x=>!x.AllComplete());
                if(thissq == null)
                {
                    areaSize.Add(squares.Where(g =>g.Ident==latestIdent).Count());
                    latestIdent++;

                    var nextLowPoint = lowPoints.Keys.FirstOrDefault(x=> !lowPoints[x]);
                    if(nextLowPoint != null)
                    {
                        lowPoints[nextLowPoint]= true;
                        thissq = new square(nextLowPoint.Item1,nextLowPoint.Item2,latestIdent);
                        Console.WriteLine(areaSize.Sum()/(double)non9);
                        squares.Add(thissq);
                    }
                }

                if(thissq ==null)
                {
                    break;
                }

                var xLen = lines[0].Length;
                var yLen = lines.Count();
                var x = thissq.pos.Item1;
                var y = thissq.pos.Item2;
                var newX = 0;
                var newY =0;
                
                if(!thissq.leftComplete)
                {
                    thissq.leftComplete = true;
                    if(x-1 >=0)
                    {
                        newX = x-1;
                        newY = y;

                      square next;
                        var exist = GetSquare(lines,squares, newX, newY,out next);
                        if(next!=null)
                        {
                            next.rightComplete = true;

                            if(!exist)
                            {
                                next.Ident = thissq.Ident;
                                squares.Add(next);
                            }
                        }

                    }
                }
                if(!thissq.rightComplete)
                {
                    thissq.rightComplete = true;
                    if(x+1< xLen)
                    {
                        newX = x+1;
                        newY = y;

                        square next;
                        var exist = GetSquare(lines,squares, newX, newY,out next);

                        if(next!=null)
                        {
                            next.leftComplete = true; 

                            if(!exist)
                            {
                                next.Ident = thissq.Ident;
                                squares.Add(next);
                            }
                        }
                    }
                }
                if(!thissq.upComplete)
                {
                    thissq.upComplete = true;
                    if(y-1>=0)
                    {
                        newX = x;
                        newY = y-1;

                      square next;
                        var exist = GetSquare(lines,squares, newX, newY,out next);

                        if(next!=null)
                        {
                            next.downComplete = true;

                            if(!exist)
                            {
                                next.Ident = thissq.Ident;
                                squares.Add(next);
                            }
                        }
                    }
                }
                if(!thissq.downComplete)
                {
                    thissq.downComplete = true;
                    if(y+1<yLen)
                    {
                        newX = x;
                        newY = y+1;

                        square next;
                        var exist = GetSquare(lines,squares, newX, newY,out next);

                        if(next!=null)
                        {
                            next.upComplete = true; 

                            if(!exist)
                            {
                                next.Ident = thissq.Ident;
                                squares.Add(next);
                            }
                        }
                    }
                }


            }
            areaSize.Add(squares.Where(g =>g.Ident==latestIdent).Count());
            var top = areaSize.OrderBy(x=>x).Reverse().Take(3).ToList();
            Console.WriteLine(top[0]*top[1]*top[2]);

        }

        private bool GetSquare(List<string> lines, List<square> squares, int newX, int newY, out square sqOfInterest)
        {
            var exist = squares.FirstOrDefault(g=>g.pos.Item1==newX && g.pos.Item2==newY);
            if(exist != null)
            {
                sqOfInterest = exist;
                return true;
            }

            if(lines[newY][newX] == '9')
            {
                sqOfInterest = null;
                return false;
            }

            var lowPoint = lowPoints.Keys.FirstOrDefault(x=>x.Item1 == newX && x.Item2== newY);
            if(lowPoint!= null)
                lowPoints[lowPoint] = true;

            sqOfInterest = new square(newX, newY, 0);
            return false;
        }

        private Dictionary<Tuple<int,int>,bool> part1(List<string> lines)
        {
            var xTot = lines[0].Length;
            var yTot = lines.Count();
            var tot = 0;
            Dictionary<Tuple<int,int>,bool> lowPoints = new Dictionary<Tuple<int,int>, bool>();

            for(int x = 0; x< xTot; x++)
            {
                for(int y = 0; y< yTot; y++)
                {
                    var thisSq = int.Parse(lines[y][x].ToString());
                    var top = int.MaxValue;
                    var bot = int.MaxValue;
                    var left = int.MaxValue;
                    var right = int.MaxValue;

                    if(y!= 0)
                        top = int.Parse(lines[y-1][x].ToString());
                    if(thisSq>=top)
                        continue;
            
                    if(y != yTot-1)
                        bot = int.Parse(lines[y+1][x].ToString());
                    if(thisSq>=bot)
                        continue;

                    if(x != 0)
                        left = int.Parse(lines[y][x-1].ToString());
                    if(thisSq>=left)
                        continue;

                    if(x != xTot-1)
                        right = int.Parse(lines[y][x+1].ToString());
                    if(thisSq>=right)
                        continue;
                    
                    lowPoints.Add(new Tuple<int,int>(x,y), false);
                    tot+= thisSq+1;
                }
                
            }

            Console.WriteLine(tot);
            return lowPoints;
        }


    }    

    class square
    {
        public bool upComplete = false;
        public bool downComplete = false;
        public bool leftComplete = false;
        public bool rightComplete = false;

        public int Ident = 0;

        public Tuple<int,int> pos = new Tuple<int, int>(0,0);
        public square(int x,int y, int ident)
        {
            pos = new Tuple<int, int>(x,y);
            Ident = ident;
        }

        public bool AllComplete()
        {
            return upComplete && downComplete && leftComplete &&rightComplete;
        }
        public void SetAllComplete()
        {
            upComplete = true;
            downComplete = true;
            leftComplete = true;
            rightComplete = true;
        }

        public void ResetComplete()
        {
            upComplete = false;
            downComplete = false;
            leftComplete = false;
            rightComplete = false;
        }

    }
}