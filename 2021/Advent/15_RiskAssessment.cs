using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    class RiskAssessment
    {
        List<int> entries;
        List<square> squares = new List<square>();

        List<string> map = new List<string>();
        List<int> risksToEnd = new List<int>();

        public RiskAssessment(List<string> lines)
        {
            var start = DateTime.Now;
            map = lines;

            GetMinToEnd();

            squares.Add(new square(0,0,0));

            var xLen = lines[0].Length;
            var yLen = lines.Count();
            var mult = 1;


            while(!squares.All(x=>x.AllComplete()))
            {
                var copy = squares.Where(x => !x.AllComplete()).ToList();

                
                foreach(var sq in copy)
                {
                    var x = sq.pos.Item1;
                    var y = sq.pos.Item2;

                    var nextX = new List<int>(){x-1,x+1,x,x};
                    var nextY = new List<int>(){y,y,y-1,y+1};

                    for(int i = 0; i< nextX.Count; i++)
                    {



                        if(i ==0 && sq.leftComplete)
                            continue;
                        if(i ==1 && sq.rightComplete)
                            continue;
                        if(i ==2 && sq.upComplete)
                            continue;
                        if(i ==3 && sq.downComplete)
                            continue;

                        if(i==0) //Left
                            sq.leftComplete = true;
                        else if(i==1) //Right
                            sq.rightComplete = true;
                        else if(i==2) //Up
                            sq.upComplete = true;
                        else if (i==3) //Down
                            sq.downComplete = true;

                        if(nextX[i]<0 || nextX[i]>=map[0].Length*mult)
                            continue;
                        if(nextY[i]<0 || nextY[i]>=map.Count()*mult)
                            continue;

                        square newSquare = null;
                        var exists = GetSquare(squares, nextX[i], nextY[i], out newSquare );

                        var nextSqRisk = int.Parse(map[nextY[i]%xLen][nextX[i]%yLen].ToString()) +
                                        Math.Max(nextX[i]/xLen,nextY[i]/yLen);
                        if(nextSqRisk>=10)
                            nextSqRisk -= 9;

                        var newRisk = sq.Ident + nextSqRisk;
                        if(exists)
                        {
                            var oldRisk = newSquare.Ident;
                            if(oldRisk > newRisk)
                            {
                                newSquare.Ident = newRisk;
                                newSquare.ResetComplete();
                            }
                        }
                        else
                        {
                            newSquare.Ident = newRisk;
                            squares.Add(newSquare);
                        }

                        if(i==0) //Left
                            newSquare.rightComplete = true;
                        else if(i==1) //Right
                            newSquare.leftComplete = true;
                        else if(i==2) //Up
                            newSquare.downComplete = true;
                        else if (i==3) //Down
                            newSquare.upComplete = true;

                    }
                }


                
            }

            var endSquare = squares.First(x=> x.pos.Item1 == map[0].Length*mult-1 && x.pos.Item2 == map.Count()*mult-1);
            Console.WriteLine(string.Format("Part 1 - risk {0}",endSquare.Ident));

            var span = DateTime.Now - start;
            Console.WriteLine(span.ToString("c"));
        }

        private bool GetSquare(List<square> squares, int newX, int newY, out square sqOfInterest)
        {
            var existing = squares.Where(g=>g.pos.Item1==newX && g.pos.Item2==newY);
            if(existing.Count()>0)
            {
                sqOfInterest = existing.First();
                return true;
            }

            sqOfInterest = new square(newX, newY, 0);
            return false;
        }

        private long GetMinRiskToEnd(int x, int y, int xLen,int yLen, int mult, long risk)
        {
            var nextSqRisk = int.Parse(map[y%yLen][x%xLen].ToString()) +
                Math.Max(x/xLen,y/yLen);
            if(nextSqRisk>=10)
                nextSqRisk -= 9;

            if(x == xLen*mult-1 && y== yLen*mult-1)
                return risk+nextSqRisk;

            long right = long.MaxValue;
            long down = long.MaxValue;
            if(x<xLen*mult - 1)
                right = GetMinRiskToEnd(x+1,y,xLen,yLen, mult, risk+nextSqRisk);
            if(y<yLen*mult -1)
                down = GetMinRiskToEnd(x,y+1, xLen, yLen, mult, risk + nextSqRisk);

            return Math.Min(right,down);

        }

        private void GetMinToEnd()
        {
            var mult = 5;
            var xLen = map[0].Length;
            var yLen = map.Count();

            var xMax = xLen*mult-1;
            var yMax = yLen*mult-1;


            Dictionary<Tuple<int,int>,int> minRiskDiag = new Dictionary<Tuple<int, int>, int>();
            minRiskDiag.Add(new Tuple<int, int>(xMax,yMax),0);

            while(minRiskDiag.Keys.FirstOrDefault(x=>x.Item1 == 0 && x.Item2 == 0) == null)
            {   
                Dictionary<Tuple<int,int>,int> newRisk=  new Dictionary<Tuple<int,int>,int>();
                var keys = minRiskDiag.Keys.ToList();
                
                bool valid = true;
                for(int i = 0; i< keys.Count; i++)
                {
                    var thisX = keys[i].Item1;
                    var thisY = keys[i].Item2;
                    var thisRisk = int.Parse(map[thisY%yLen][thisX%xLen].ToString());
                    thisRisk += thisY/yLen + thisX/xLen;
                    while(thisRisk >= 10)
                        thisRisk-=9;

                    if(i==0 && thisX-1>=0)
                    {
                        newRisk.Add(new Tuple<int, int>(thisX-1, thisY), minRiskDiag[keys[i]]+ thisRisk);
                    }
                    if(i == keys.Count()-1 && thisY -1 >= 0)
                    {
                        newRisk.Add(new Tuple<int, int>(thisX, thisY-1), minRiskDiag[keys[i]]+ thisRisk);
                    }
                    if(i!=keys.Count()-1)
                    {
                        var otherRisk =  int.Parse(map[keys[i+1].Item2%yLen][keys[i+1].Item1%xLen].ToString());
                        otherRisk += keys[i+1].Item2/yLen +  keys[i+1].Item1/xLen;
                        while(otherRisk >=10)
                            otherRisk -= 9;

                        var nextSqRisk =  int.Parse(map[(thisY-1)%yLen][thisX%xLen].ToString());
                        nextSqRisk += (thisY-1)/yLen +  thisX/xLen;
                        while(nextSqRisk >=10)
                            nextSqRisk -= 9;

                        if(minRiskDiag[keys[i+1]]+ otherRisk >= minRiskDiag[keys[i]]+ thisRisk)
                        {if(minRiskDiag[keys[i]]+ thisRisk+nextSqRisk < minRiskDiag[keys[i+1]] )
                            
                            {    
                                minRiskDiag[keys[i+1]] = minRiskDiag[keys[i]]+ thisRisk+nextSqRisk;
                                valid = false;
                                break;
                            }

                            newRisk.Add(new Tuple<int, int>(thisX, thisY-1), minRiskDiag[keys[i]]+ thisRisk);
                        }
                        else
                        {
                            if(minRiskDiag[keys[i+1]]+ otherRisk+nextSqRisk < minRiskDiag[keys[i]] )
                            {    
                                minRiskDiag[keys[i]] = minRiskDiag[keys[i+1]]+ otherRisk+nextSqRisk;
                                valid = false;
                                break;
                            }

                            newRisk.Add(new Tuple<int, int>(thisX, thisY-1), minRiskDiag[keys[i+1]]+ otherRisk);
                        }
                    }
                }

                if(valid)
                    minRiskDiag = newRisk.OrderBy(x=>x.Key.Item1).ToDictionary(x =>x.Key, y=>y.Value);
            }

            Console.WriteLine(string.Format("Part 2 - risk {0}", minRiskDiag.First().Value));

            
        }

    }

}