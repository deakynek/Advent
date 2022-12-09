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
            
            int day = 9;

            for (var i = 1; i <= day; i++)
            {
                Console.WriteLine("Day {0}", i);

                var start = DateTime.Now;
                var lines = (System.IO.File.ReadAllLines(String.Format(@"..\..\..\Advent{0}Input.txt", i.ToString("00")))).ToList<string>();

                switch (i)
                {
                    case 1:
                        var day1 = new JungleFood(lines);
                        break;
                    case 2:
                        var day2 = new RockPaperSissors(lines);
                        break;
                    case 3:
                        var day3 = new Packing(lines);
                        break;
                    case 4:
                        var day4 = new Cleaning(lines);
                        break;
                    case 5:
                        var day5 = new CraneOps(lines);
                        break;
                    case 6:
                        var day6 = new Radios(lines);
                        break;
                    case 7:
                        var day7 = new FileSys(lines);
                        break;
                    case 8:
                        var day8 = new TreeHouse(lines);
                        break;
                    case 9:
                        var day9 = new Ropes(lines);
                        break;
                    default:
                        break;
                }

                PrintElapsedTime(String.Format("Day {0} running both days", i), start);
                Console.WriteLine();
            }
        }

        static private void PrintElapsedTime(string operation, DateTime start)
        {
            var span = DateTime.UtcNow - start;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:000}",
                span.Minutes, span.Seconds,
                span.Milliseconds);
            Console.WriteLine("RunTime of " + operation + ": " + elapsedTime);
        }
    }
}
