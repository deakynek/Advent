using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;


namespace Advent1
{
    class Program
    {
        static void Main(string[] args)
        {

            string[] lines = System.IO.File.ReadAllLines(@"C:\Users\DeepThought\Documents\AdventCode\2020\Advent1\Advent1Input.txt");

            var index = 1;
            foreach (string line in lines)
            {
                ///Part 1
                // var entry = Int64.Parse(line);
                // var complement = 2020 - entry;

                // if(lines.Contains(complement.ToString()))
                // {
                //     Console.WriteLine("Found!" + complement.ToString() + " & " + entry.ToString());

                //     Console.WriteLine((entry*(2020-entry)).ToString());
                // }

                //Part 2
                var found = false;
                var entry1 = Int64.Parse(line);
                foreach(string nextLine in lines.Skip(index).ToArray())
                {
                    var entry2 = Int64.Parse(nextLine);
                    var complement = 2020 - entry2 - entry1;

                    if(lines.Contains(complement.ToString()))
                    {
                        Console.WriteLine("Found!" + " "+ complement.ToString() + " & " + entry1.ToString() + " & " + entry2.ToString());

                        Console.WriteLine((entry1*entry2*complement).ToString());
                        found = true;
                    }

                    if(found){break;}                
                }

                if(found){break;}    
                index++;
            }

        }
    }
}
