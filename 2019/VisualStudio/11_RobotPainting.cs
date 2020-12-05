using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    public class PaintingRobot
    {
        IntComputer robotProg;
        List<Tuple<int,int,int>> PaintedSquares;
        int FacingDirection = 1;


        public PaintingRobot(List<string> inputs)
        {
            robotProg = new IntComputer(inputs);
            PaintedSquares = new List<Tuple<int, int, int>>();
        }

        public void PaintSquares()
        {
            int currentx = 0;
            int currenty = 0;
            PaintedSquares.Add(new Tuple<int, int, int>(0,0,1));
            robotProg.ResetProgram();
            
            while(!robotProg.programComplete)
            {
                int color = 0;
                if(PaintedSquares.Any(x => x.Item1==currentx && x.Item2==currenty))
                {
                    color = PaintedSquares.First(x => x.Item1==currentx && x.Item2==currenty).Item3;
                }

                robotProg.AddInput(color);
                robotProg.RunProgram(robotProg.nextStepIndex,false,true);
                var paint = robotProg.lastOutput;
                robotProg.RunProgram(robotProg.nextStepIndex,false,true);
                var changeDir = robotProg.lastOutput;

                if(PaintedSquares.Any(x => x.Item1==currentx && x.Item2==currenty))
                {
                    PaintedSquares.Remove(PaintedSquares.First(x => x.Item1==currentx && x.Item2 == currenty));
                }

                PaintedSquares.Add(new Tuple<int, int, int>(currentx,currenty,(int)paint));

                if(changeDir == 0)
                    FacingDirection = (4+FacingDirection-1)%4;
                else
                    FacingDirection = (4+FacingDirection+1)%4;

                switch(FacingDirection)
                {
                    case 0:
                        currentx+=1;
                        break;
                    case 1:
                        currenty+=1;
                        break;
                    case 2:
                        currentx -=1;
                        break;
                    case 3:
                        currenty -=1;
                        break;
                }
                
            }

            Console.WriteLine(String.Format("Part 1 - Painted Squares = " + PaintedSquares.Count().ToString()));
            PrintOnHull();
        }
        private void PrintOnHull()
        {
            Console.WriteLine("\nPart 2");
            int minX = PaintedSquares.Select(x => x.Item1).Min();
            int maxX = PaintedSquares.Select(x => x.Item1).Max();

            int minY = PaintedSquares.Select(x => x.Item2).Min();
            int maxY = PaintedSquares.Select(x => x.Item2).Max();

            for(int y = maxY; y>=minY; y--)
            {
                string line = "";
                for(int x =maxX; x>=minX; x--)
                {
                    bool paint = false;
                    if(PaintedSquares.Any(s => s.Item1==x && s.Item2==y))
                    {
                        paint = PaintedSquares.First(s => s.Item1== x && s.Item2== y).Item3 == 1;
                    }

                    if(paint)
                        line = line + "#";
                    else
                        line = line + " ";
                }


                Console.WriteLine(line);

            }

        }

    }

}