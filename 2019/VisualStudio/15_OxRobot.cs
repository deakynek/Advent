using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    public class OxRobot
    {
        List<Tuple<int,int,int>> map = new List<Tuple<int,int,int>>();
        IntComputer robot;

        public OxRobot(List<string> inputs)
        {
            robot = new IntComputer(inputs);
        }

        public void TraverseMapFromStart()
        {
            map.Clear();

            bool sensorFound = false;
            int currDirection = 1;
            map.Add(new Tuple<int, int, int>(0,0,0));
            var currPos = new Tuple<int,int>(0,0);
            int foundSensorCount = 0;
            int? sensorSteps = 0;

            while(foundSensorCount < 5)
            {
                robot.AddInput(currDirection);
                robot.RunProgram(robot.nextStepIndex, false, true);
                int feedBack = (int)robot.lastOutput;

                if(feedBack != 0)
                {
                    var newPos = getNewPos(currPos.Item1, currPos.Item2,currDirection);
                    if(!map.Any(tile => tile.Item1 == newPos.Item1 && tile.Item2 == newPos.Item2))
                    {
                        var posSteps1 = map.FirstOrDefault(tile => tile.Item1 == currPos.Item1 && tile.Item2 == currPos.Item2)?.Item3;
                        if(posSteps1 != null)
                            map.Add(new Tuple<int, int, int>(newPos.Item1, newPos.Item2, posSteps1.Value+1));
                    }
                    currPos = newPos;
                    currDirection = DirToTheLeft(currDirection);
                }
                else
                {
                    currDirection = DirToTheRIght(currDirection);
                }

                if(feedBack == 2)
                {
                    if(!sensorFound)
                    {
                        sensorFound = true;
                        sensorSteps = map.FirstOrDefault(tile => tile.Item1 == currPos.Item1 && tile.Item2 == currPos.Item2)?.Item3;
                        
                        map.Clear();
                        map.Add(new Tuple<int,int,int>(currPos.Item1, currPos.Item2,0));
                        
                    }
                    foundSensorCount++;
                }
            }


            
            Console.WriteLine(String.Format("Steps to sensor : {0}", sensorSteps.Value));

            var MaxMinutes = map.Select(tile => tile.Item3).Max();
            Console.WriteLine(String.Format("Max minutes to fill with O2 = {0}", MaxMinutes));
        }

        public int DirToTheRIght(int currDirection)
        {
            switch(currDirection)
            {
                case 1: //north
                    return 4;
                case 2: //south
                    return 3;
                case 3: //west
                    return 1;
                case 4: //east
                    return 2;
            }

            return 0;
        }

        public int DirToTheLeft(int currDirection)
        {
            switch(currDirection)
            {
                case 1: //north
                    return 3;
                case 2: //south
                    return 4;
                case 3: //west
                    return 2;
                case 4: //east
                    return 1;
            }

            return 0;
        }

        public Tuple<int,int> getNewPos(int currX, int currY, int currDir)
        {
            switch(currDir)
            {
                case 1: //north
                    currY += 1;
                    break;
                case 2: //south
                    currY -= 1;
                    break;
                case 3: //west
                    currX -= 1;
                    break;
                case 4: //east
                    currX += 1;
                    break;
            }

            return new Tuple<int, int>(currX, currY);
        }
    }


}