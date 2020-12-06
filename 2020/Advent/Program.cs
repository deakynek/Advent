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

            int day = 6;
            var lines = System.IO.File.ReadAllLines(String.Format(@".\Advent{0}Input.txt",day)).ToList<string>();

            switch(day)
            {
                case 1:
                    var Finder = new SumFinder(lines);

                    var start = DateTime.UtcNow;

                    var ns = new List<int>(){2,3,4,5,6,7,8,9,10};
                    foreach(var n in ns)
                    {
                        start = DateTime.UtcNow;
                        Finder.PrintMultOfNNumbersSummingToValue("Sum "+n.ToString(), 2020, n);
                        PrintElapsedTime("Sum "+n.ToString(), start);

                    }
                    break;

                case 2: 
                    var passwordParsed = new PasswordParser(lines);
                    Console.WriteLine(String.Format("{0} Valid passwords found by range", passwordParsed.FindValidPasswordCountByRange()));
                    Console.WriteLine(String.Format("{0} Valid passwords found by position", passwordParsed.FindValidPasswordCountByPosition()));
                    break;
                case 3:
                    var skiing = new Skiing(lines);
                    var trees =  skiing.Part1_GetAndPrintTrees(3,1);
                    skiing.Part2_PrintTreeCount();
                    break;
                    
                case 4:
                    var passValidator = new PassPortVerification(lines);
                    passValidator.ValidatePassPortsPrintInvalid();
                    break;
                case 5:
                    var seatFinder = new SeatFinder(lines);
                    seatFinder.GetTicketIds();
                    break;
                case 6:
                    var customs = new Customs(lines);
                    customs.PrintGroupsTotal();
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
