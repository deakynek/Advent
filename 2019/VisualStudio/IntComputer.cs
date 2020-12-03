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

        List<int> Inputs = new List<int>();
        int inputIndex = 0;

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

        public void SetInputs(List<int> input)
        {
            Inputs = input;
            inputIndex = 0;
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
            var instruction = getNumberAtIndexIfExists(index);
            if(!instruction.HasValue)
                return null;

            string parameterModes = String.Format("{0:00000}",instruction.Value);
            int command = Int32.Parse(parameterModes.Substring(3));

            int numOfOperands = 4;
            if(command==3 || command ==4)
                numOfOperands = 2;
            if(command==5 || command == 6)
                numOfOperands = 3;

            var op1 = getNumberAtIndexIfExists(index+1);
            var op2 = getNumberAtIndexIfExists(index+2);
            var op3 = getNumberAtIndexIfExists(index+3);



            if(parameterModes[2] == '0' && numOfOperands > 1 && op1.HasValue)
            {
                op1 = getNumberAtIndexIfExists(op1.Value);
            }   
            if(parameterModes[1] == '0' && numOfOperands > 2 && op2.HasValue)
            {
                op2 = getNumberAtIndexIfExists(op2.Value);
            }                     
            if(parameterModes[0] == '0' && numOfOperands > 3 &&op3.HasValue )
            {
                op3 = getNumberAtIndexIfExists(op3.Value);
            }

            switch(command)
            {
                case 1:
                    AddOp(  op1,
                            op2,
                            getNumberAtIndexIfExists(index+3));
                    break;
                case 2:
                    MultOp( op1,
                            op2,
                            getNumberAtIndexIfExists(index+3));
                     break;
                case 3:
                    StoreInputOp(getNumberAtIndexIfExists(index+1), true);
                    break;
                case 4:
                    OutputOp(op1);
                    break;

                case 5:
                    if(!op1.HasValue || !op2.HasValue)
                    {
                        Console.WriteLine("Invalid non-zero check");
                        return null;
                    }
                    if(op1.Value != 0)
                        return op2.Value;

                    break;
                case 6:
                    if(!op1.HasValue || !op2.HasValue)
                    {
                        Console.WriteLine("Invalid zero check");
                        return null;
                    }
                    if(op1.Value == 0)
                        return op2.Value;

                    break;
                
                case 7:
                    if(!op1.HasValue || !op2.HasValue)
                    {
                        Console.WriteLine("Invalid less than check");
                        return null;
                    }
                    if(op1.Value <  op2.Value)
                        StoreInputOp(getNumberAtIndexIfExists(index+3),false,1);
                    else
                        StoreInputOp(getNumberAtIndexIfExists(index+3),false,0);

                    break;

                case 8:
                    if(!op1.HasValue || !op2.HasValue)
                    {
                        Console.WriteLine("Invalid less than check");
                        return null;
                    }
                    if(op1.Value == op2.Value)
                        StoreInputOp(getNumberAtIndexIfExists(index+3),false,1);
                    else
                        StoreInputOp(getNumberAtIndexIfExists(index+3),false,0);

                    break;

                case 99:
                    return null;
                default:
                    return null;
            }

            return index+numOfOperands;

        }

        private int? getNumberAtIndexIfExists(int? index)
        {
            if(!index.HasValue || index>= Instructions.Count)
                return null;
            
            return Instructions[index.Value];
        }

        private void AddOp(int? op1, int? op2, int? outputAddr)
        {
            if(!op1.HasValue || !op2.HasValue || !outputAddr.HasValue || outputAddr.Value >= Instructions.Count)
            {
                Console.WriteLine("Add operation failed");
                return;
            }

            Instructions[outputAddr.Value] = op1.Value+op2.Value;
        }


        private void MultOp(int? op1, int? op2, int? outputAddr)
        {
            if(!op1.HasValue || !op2.HasValue || !outputAddr.HasValue || outputAddr.Value >= Instructions.Count)
            {
                Console.WriteLine("Multiplication operation failed");
                return;
            }

            Instructions[outputAddr.Value] = op1.Value*op2.Value;
        }

        private void StoreInputOp(int? outputAddr, bool getInputFromUser, int? input = 0)
        {
            if(!outputAddr.HasValue || outputAddr.Value >= Instructions.Count)
            {
                Console.WriteLine("Store Input op failed");
                return;
            }

            if(!getInputFromUser)
            {
                if(!input.HasValue)
                {
                    Console.WriteLine("invalid input given");
                    return;
                }
            }
            else
            {
                if(inputIndex >= Inputs.Count)
                {
                    Console.WriteLine("Enter input: ");
                    int temp;
                    if(!Int32.TryParse(Console.ReadLine(), out temp))
                    {
                        Console.WriteLine("Not a valid Input");
                        return;
                    }
                    input = temp;
                }
                else
                {
                    input = Inputs[inputIndex];
                    Console.WriteLine(String.Format("Using preset Input: {0}",input));
                    inputIndex++;
                }

            }

            Instructions[outputAddr.Value] = input.Value;
        }

        private void OutputOp(int? infoAddr)
        {
            if(!infoAddr.HasValue)
            {
                Console.WriteLine("Output op failed");
                return;
            }

            Console.WriteLine(String.Format("Output Command: {0}",infoAddr.Value));
        }        
    }
}