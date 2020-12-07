using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    public class Scaffold
    {
        IntComputer robot;
        List<string> map = new List<string>();

        public Scaffold(List<string> inputs)
        {
            robot = new IntComputer(inputs);
            GetInitialMap();
            PrintInterSections();
            PrintMap();

            SetInstructionsAndRun();
        }

        private void GetInitialMap()
        {
            robot.ResetProgram();
            string thisLine = "";

            while(!robot.programComplete)
            {
                robot.RunProgram(robot.nextStepIndex,false, true);

                var output = robot.lastOutput;
                switch(output)
                {
                    case 10:
                        map.Add(thisLine);
                        thisLine = "";
                        break;
                    default:
                        thisLine += (char) output;
                        break;
                }
            }
        }

        private void PrintInterSections()
        {
            var intersectionAlignment = 0;
            for(int line=1; line<map.Count()-1; line++)
            {
                if(map[line] == "" || map[line+1] == "")
                    continue;

                for(int pixel = 1; pixel < map[line].Count()-1; pixel++)
                {
                    if(IsScaffold(map[line][pixel]) &&
                        IsScaffold(map[line-1][pixel]) &&
                        IsScaffold(map[line+1][pixel]) &&
                        IsScaffold(map[line][pixel+1]) &&
                        IsScaffold(map[line][pixel-1]))
                    {
                        intersectionAlignment += line*pixel;
                    }
                }
            }

            Console.WriteLine("Part 1 - Intersection Counts "+ intersectionAlignment.ToString());

        }

        private void PrintMap()
        {
            Console.WriteLine("012345678901234567890123456789012345678901234567890");

            var lineNumber = 0;
            foreach(var line in map)
            {
                Console.WriteLine(lineNumber.ToString() + line);
                lineNumber = (lineNumber+1) %10;
            }
        }

        private bool IsScaffold(char pix)
        {
            return pix == '#' || pix=='<' || pix=='>' || pix=='^' || pix =='v';
        }

        private void SetInstructionsAndRun()
        {
            string CommandSequece = "A,A,B,C,B,C,B,C,B,A\n";
            string A = "R,10,L,12,R,6\n";
            string B = "R,6,R,10,R,12,R,6\n";
            string C = "R,10,L,12,L,12\n";
            string feedback = "n\n";

            robot.ResetProgram();
            robot.SetProgramIndex(0,2);

            List<string> commands = new List<string>(){CommandSequece,A,B,C,feedback};
            foreach(var command in commands)
            {
                command.ToList<char>().ForEach(c => robot.AddInput((long)c));
            }
            
            while(!robot.programComplete)
            {
                robot.RunProgram(robot.nextStepIndex, false, true);
            }

            Console.WriteLine("Part 2 - DustCollected " + robot.lastOutput);

        }
        
    }
}