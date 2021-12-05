using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

namespace Advent
{
    public class Jumper
    {
        IntComputer robot;
        public Jumper(List<string> inputs)
        {
            robot = new IntComputer(inputs);

            
            RunRobotRun();
    
        }

        private void WalkingRobot()
        {

            var commands = "NOT T T\nAND A T\nAND C T\nNOT T J\nNOT B T\nOR T J\nAND D J\nWALK\n";
            var ascii = Encoding.ASCII.GetBytes(commands);
            robot.SetInputs(ascii.Select(x=> (long)x).ToList());

            string outputString = "";
            long outputDamage = 0;
            while(!robot.programComplete)
            {
                robot.RunProgram(robot.nextStepIndex, false, true);
                var output = robot.lastOutput;

                if(output < 127)
                {
                    outputString += ((char)output).ToString();
                }
                else
                {
                    outputDamage = output;
                }
            }

            Console.WriteLine("Final Damage - " + outputDamage);
            Console.WriteLine(outputString);
        }

        private void RunRobotRun()
        {

            var commands = "NOT T T\nAND A T\nAND C T\nNOT T J\nNOT B T\nOR T J\nAND D J\nNOT E T\nNOT T T\nOR H T\nAND T J\nRUN\n";
            var ascii = Encoding.ASCII.GetBytes(commands);
            robot.SetInputs(ascii.Select(x=> (long)x).ToList());

            string outputString = "";
            long outputDamage = 0;
            while(!robot.programComplete)
            {
                robot.RunProgram(robot.nextStepIndex, false, true);
                var output = robot.lastOutput;

                if(output < 127)
                {
                    outputString += ((char)output).ToString();
                }
                else
                {
                    outputDamage = output;
                }
            }

            Console.WriteLine("Final Damage - " + outputDamage);
            Console.WriteLine(outputString);
        }
    }
}