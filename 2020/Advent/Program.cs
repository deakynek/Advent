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

            var ns = new List<int>(){2,3,4,5,6,7,8,9,10};
            foreach(var n in ns)
            {
                start = DateTime.UtcNow;
                Finder.PrintMultOfNNumbersSummingToValue("Sum "+n.ToString(), 5000, n);
                PrintElapsedTime("Sum "+n.ToString(), start);

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
