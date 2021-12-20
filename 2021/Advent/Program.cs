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

            int day =20;
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
                case 9:
                    var day9 = new SmokeBasin(lines);
                    break;
                case 10:
                    var day10 = new Brackets(lines);
                    break;
                case 11:
                    var day11 = new OctoFlash(lines);
                    break;
                case 12:
                    var day12 = new CavePaths(lines);
                    break;
                case 13:
                    var day13 = new Folding(lines);
                    break;
                case 14:
                    var day14 = new Polymers(lines);
                    break;
                case 15:
                    var day15 = new RiskAssessment(lines);
                    break;
                case 16:
                    var day16 = new BinaryOperators(lines);
                    break;
                case 17:
                    var day17 = new ProbeLauching(lines);
                    break;
                case 18:
                    var day18 = new SnailMath(lines);
                    break;
                case 19:
                    var day19 = new SensorTracking(lines);
                    break;
                case 20:
                    var day20 = new TrenchMap(lines);
                    break;
                case 21:
                    var day21= new Temp21(lines);
                    break;
                case 22:
                    var day22 = new Temp22(lines);
                    break;
                case 23:
                    var day23 = new Temp23(lines);
                    break;
                case 24:
                    var day24 = new Temp24(lines);
                    break;
                case 25:
                    var day25 = new Temp25(lines);
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
