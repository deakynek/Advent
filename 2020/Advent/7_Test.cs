using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace Advent
{
    class Test
    {
        List<string> instructions = new List<string>();
        long accumulator = 0;

        public Test(List<string> lines)
        {
            instructions = lines;
            RunProgram(instructions);
            PrintPart1(accumulator.ToString());

            FixProgram();
        }

        public int Execute(List<string> instructs, int index)
        {
            var op = instructs[index];

            var ins = op.Substring(0,3);

            var num = Int32.Parse(op.Substring(5));
            if(op.Substring(4,1) == "-")
                num = -1*num;

            switch(ins)
            {
                case "nop":
                    return index+1;
                case "acc":
                    accumulator += num;
                    return index+1;
                case "jmp":
                    return index+num;
            }

            return 0;
        }

        private int RunProgram(List<string> instr)
        {
            accumulator = 0;
            List<int> indexesVisited = new List<int>();
            accumulator = 0;
            int nextOp = 0;

            while(!indexesVisited.Contains(nextOp))
            {
                indexesVisited.Add(nextOp);
                nextOp = Execute(instr,nextOp);

                if(nextOp == instr.Count())
                    break;
            }

            return nextOp;
        }

        private void FixProgram()
        {
            for(int i= 0; i <instructions.Count(); i++)
            {
                var inst = instructions[i].Substring(0,3);
                if(inst == "acc")
                    continue;

                var copyProgram = instructions.ToList();
                if(inst == "nop")
                    copyProgram[i] = "jmp" + instructions[i].Substring(3);
                
                else if(inst == "jmp")
                    copyProgram[i] = "nop" + instructions[i].Substring(3);

                var endingIndex = RunProgram(copyProgram);

                if(endingIndex == instructions.Count())
                    break;

            }

            PrintPart2(accumulator.ToString());

        }

        public void PrintPart1(string ans)
        {
            Console.WriteLine("Part 1: " +ans);
        }

        public void PrintPart2(string ans)
        {
            Console.WriteLine("Part 2: " +ans);
        }        

    }
}