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

            int day = 2;
            string[] lines = System.IO.File.ReadAllLines(String.Format(@".\Advent{0}\Advent{0}Input.txt",day));

            switch(day)
            {
                case 1:
                    var Finder = new SumFinder(lines.ToList<string>());

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
                    var passwordParsed = new PasswordParser(lines.ToList<string>());
                    Console.WriteLine(String.Format("{0} Valid passwords found by range", passwordParsed.FindValidPasswordCountByRange()));
                    Console.WriteLine(String.Format("{0} Valid passwords found by position", passwordParsed.FindValidPasswordCountByPosition()));
                    break;
                case 3:
                case 4:
                case 5:
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
