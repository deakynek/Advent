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

            int day =8;
            var lines = System.IO.File.ReadAllLines(String.Format(@".\Advent{0}Input.txt",day)).ToList<string>();

            switch(day)
            {
                case 1:
                    var day1 = new Sonar(lines);
                    break;

                case 2:
                    var day2 = new SubNav(lines);
                    break;

                case 3:
                    var day3 = new Radiation(lines);
                    break;
                case 4:
                    var day4 = new Bingo(lines);
                    break;
                case 5:
                    var day5 = new Vents(lines);
                    break;
                case 6:
                    var day6 = new GrowthRate(lines);
                    break;
                case 7:
                    var day7 = new SubAlignment(lines);
                    break;
                case 8:
                    var day8 = new SevenSegDisplay(lines);
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
