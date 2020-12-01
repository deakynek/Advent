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


            string[] lines = System.IO.File.ReadAllLines(@".\Advent1\Advent1Input.txt");

            var Finder = new SumFinder(lines.ToList<string>());

            var start = DateTime.UtcNow;
            Finder.PrintMultOfNNumbersSummingToValue("Sum 2", 2020, 2);
            PrintElapsedTime("Sum 2", start);

            start = DateTime.UtcNow;
            Finder.PrintMultOfNNumbersSummingToValue("Sum 3", 2020,3);
            PrintElapsedTime("Sum 3", start);

            start = DateTime.UtcNow;
            Finder.PrintMultOfNNumbersSummingToValue("Sum 4", 2020, 4);
            PrintElapsedTime("Sum 4", start);

            start = DateTime.UtcNow;
            Finder.PrintMultOfNNumbersSummingToValue("Sum 5", 2020, 5);
            PrintElapsedTime("Sum 5", start);           
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
