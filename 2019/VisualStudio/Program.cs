using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;


namespace Advent
{
    class Program
    {
        static void Main(string[] args)
        {

            int day = 7;
            var lines = (System.IO.File.ReadAllLines(String.Format(@".\Advent{0}Input.txt",day))).ToList<string>();


            switch(day)
            {
                case 1:
                    var Fuel = new FuelCounter(lines);
                    Fuel.PrintTotalFuel1();
                    Fuel.PrintTotalFuel2();
                    break;

                case 2: 
                    var comp = new IntComputer(lines);
                    comp.UpdateInitials(12,2);
                    comp.RunProgram(0,true);

                    comp.FindInputsThatProduceValue(19690720);
                    
                    break;
                case 3:
                    var wires = new Wires(lines);
                    wires.PrintMinDistIntersection();
                    wires.PrintMinStepsIntersections();
                    break;
                case 4:
                    var password = new PasswordVerification(271973, 785961);
                    password.PrintValidPasswordCount(1);
                    password.PrintValidPasswordCount(2);
                    break;
                case 5:
                    comp = new IntComputer(lines);

                    Console.WriteLine("\nPart 1");
                    comp.SetInputs(new List<int>(){1});
                    comp.RunProgram(0,false);

                    Console.WriteLine("\nPart 2");
                    comp.ResetProgram();
                    comp.SetInputs(new List<int>(){5});
                    comp.RunProgram(0,false);
                    break;
                
                case 6:
                    var orbits = new Orbits(lines);
                    orbits.PrintTotalDirectAndIndirectOrbits();
                    orbits.PrintOrbitalTransfersToSanta();
                    break;

                case 7:
                    var engines = new EngineThrusters(lines);
                    //engines.RunThroughPossibleCombos();
                    engines.RunThroughPossibleCombosWithFeedback();
                    break;
                default:
                    break;
            }
        }

        static private void PrintElapsedTime(string operation, DateTime start)
        {
            var span = DateTime.UtcNow - start;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:000}",
                span.Minutes, span.Seconds,
                span.Milliseconds);
            Console.WriteLine("RunTime of " +operation+ ": " + elapsedTime);
        }
    }
}
