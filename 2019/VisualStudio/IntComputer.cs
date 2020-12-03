using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;


namespace Advent
{
    public class IntComputer
    {
        List<int> Instructions = new List<int>();
        List<int> InitialInstructions = new List<int>();

        public IntComputer(List<string> input)
        {
            var separated = input[0].Split(',');
            foreach(var entry in separated)
            {
                InitialInstructions.Add(Int32.Parse(entry));
                Instructions.Add(Int32.Parse(entry));
            }
        }

        public void UpdateInitials(int one, int two)
        {
            Instructions[1] = one;
            Instructions[2] = two;
        }

        public void ResetProgram()
        {
            Instructions = InitialInstructions.Select(x =>x).ToList<int>();
        }

        public void FindInputsThatProduceValue(int val)
        {
            bool found = false;
            int a = 0;
            int b = 0;

            for(a = 0; a<100; a++)
            {
                for(b=0; b<100; b++)
                {
                    ResetProgram();
                    UpdateInitials(a,b);
                    RunProgram(false);

                    found = Instructions[0] == val;
                    if(found)
                        break;
                }

                if(found)
                    break;
            }

            if(found)
            {
                Console.WriteLine(String.Format("{0} is produced by input : {1:00}{2:00}", val, a, b));
            }
            else
            {
                Console.WriteLine(val.ToString()+" could not be produced.");
            }

        }

        public void RunProgram(bool print)
        {
            int? index = 0;
            while(index != null)
            {
                index = RunOp(index.Value);
            } 

            if(print)
            {
                Console.WriteLine("Final Answer: " + Instructions[0].ToString());
            }
        }

        private int? RunOp(int index)
        {
            int op1 = Instructions[Instructions[index+1]];
            int op2 = Instructions[Instructions[index+2]];

            int outputAddr = Instructions[index+3];


            switch(Instructions[index])
            {
                case 1:
                    Instructions[outputAddr] = op1 +op2;
                    return index+4;
                case 2:
                    Instructions[outputAddr] = op1*op2;
                    return index+4;
                case 99:
                    return null;
                default:
                    return null;

            }

        }
    }
}