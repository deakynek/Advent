using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    public class IntComputer
    {
        List<long> Instructions = new List<long>();
        List<long> InitialInstructions = new List<long>();

        List<long> Inputs = new List<long>();
        public int inputIndex = 0;
        public long lastOutput=0;
        bool stopOnOutput = false;
        public bool programComplete = false;    
        public int nextStepIndex = 0;
        public bool debugMessages = true;

        private long relativebase = 0;
        public IntComputer(List<string> input)
        {
            var separated = input[0].Split(',');
            foreach(var entry in separated)
            {
                InitialInstructions.Add(Int64.Parse(entry));
                Instructions.Add(Int64.Parse(entry));
            }
        }

        public void UpdateInitials(long one, long two)
        {
            Instructions[1] = one;
            Instructions[2] = two;
        }

        public void SetInputs(List<long> input)
        {
            Inputs = input;
            inputIndex = 0;
        }

        public void AddInput(long input)
        {
            Inputs.Add(input);
        }

        public void SetProgramIndex(int index, long val)
        {
            Instructions[index] = val;
        }

        public void ResetProgram()
        {
            Instructions = InitialInstructions.Select(x =>x).ToList<long>();
            programComplete = false;
            stopOnOutput = false;
            inputIndex = 0;
            Inputs = new List<long>();
            nextStepIndex =0;
            relativebase = 0;
        }

        public void FindInputsThatProduceValue(long val)
        {
            bool found = false;
            long a = 0;
            long b = 0;

            for(a = 0; a<100; a++)
            {
                for(b=0; b<100; b++)
                {
                    ResetProgram();
                    UpdateInitials(a,b);
                    RunProgram(0,false);

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

        public void RunProgram(int? index, bool prlong, bool pauseOnOutput = false )
        {
            stopOnOutput = pauseOnOutput;
            while(index != null)
            {
                index = RunOp(index.Value);
            } 

            if(prlong)
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
            long command = Int32.Parse(parameterModes.Substring(3));

            int numOfOperands = 4;
            if(command==3 || command ==4 || command == 9)
                numOfOperands = 2;
            else if(command==5 || command == 6)
                numOfOperands = 3;
            else if(command == 99)
                numOfOperands = 1;

            var op1 = GetParameterModeValue(parameterModes[2],
                                            numOfOperands > 1,
                                            getNumberAtIndexIfExists(index+1));
            var op2 = GetParameterModeValue(parameterModes[1],
                                            numOfOperands > 2,
                                            getNumberAtIndexIfExists(index+2));
            var op3 = GetParameterModeValue(parameterModes[0],
                                            numOfOperands > 3,
                                            getNumberAtIndexIfExists(index+3));

            int op1Index = GetParameterModeIndex(parameterModes[2],
                                            numOfOperands > 1,
                                            getNumberAtIndexIfExists(index+1));
            int op2Index = GetParameterModeIndex(parameterModes[1],
                                            numOfOperands > 2,
                                            getNumberAtIndexIfExists(index+2));
            int op3Index = GetParameterModeIndex(parameterModes[0],
                                            numOfOperands > 3,
                                            getNumberAtIndexIfExists(index+3));

            switch(command)
            {
                case 1:
                    AddOp(  op1,
                            op2,
                            op3Index);
                    break;
                case 2:
                    MultOp( op1,
                            op2,
                            op3Index);
                     break;
                case 3:
                    StoreInputOp(op1Index, true);
                    break;
                case 4:
                    OutputOp(op1);
                    if(stopOnOutput)
                    {
                        nextStepIndex = index += numOfOperands;
                        return null;
                    }
                    break;

                case 5:
                    if(!op1.HasValue || !op2.HasValue)
                    {
                        Console.WriteLine("Invalid non-zero check");
                        return null;
                    }
                    if(op1.Value != 0)
                        return (int)op2.Value;

                    break;
                case 6:
                    if(!op1.HasValue || !op2.HasValue)
                    {
                        Console.WriteLine("Invalid zero check");
                        return null;
                    }
                    if(op1.Value == 0)
                        return (int)op2.Value;

                    break;
                
                case 7:
                    if(!op1.HasValue || !op2.HasValue)
                    {
                        Console.WriteLine("Invalid less than check");
                        return null;
                    }
                    if(op1.Value <  op2.Value)
                        StoreInputOp(op3Index,false,1);
                    else
                        StoreInputOp(op3Index,false,0);

                    break;

                case 8:
                    if(!op1.HasValue || !op2.HasValue)
                    {
                        Console.WriteLine("Invalid less than check");
                        return null;
                    }
                    if(op1.Value == op2.Value)
                        StoreInputOp(op3Index,false,1);
                    else
                        StoreInputOp(op3Index,false,0);

                    break;

                case 9:
                    relativebase+= op1.Value;
                    break;
                case 99:
                    programComplete = true;
                    return null;
                default:
                    return null;
            }

            return index+numOfOperands;

        }

        private long? GetParameterModeValue(char parameterMode, bool validbyOperands, long? val)
        {
            if(validbyOperands
                 && val.HasValue)
            {
                if(parameterMode == '0')
                {
                    val = getNumberAtIndexIfExists((int)val.Value);
                } 
                else if(parameterMode== '2')
                {
                    val = getNumberAtIndexIfExists((int)((val.Value) + relativebase));
                }
            }

            return val;
        }

        private int GetParameterModeIndex(char parameterMode, bool validbyOperands, long? val)
        {
            int ret = (int)val.Value;
            if(validbyOperands
                 && val.HasValue
                 && parameterMode== '2')
            {
                ret = (int)((val.Value) + relativebase);
            }

            return ret;
        }        

        private long? getNumberAtIndexIfExists(int? index)
        {
            if(!index.HasValue)
                return null;
            if(index>= Instructions.Count)
                return 0;
            
            return Instructions[index.Value];
        }

        private void AddOp(long? op1, long? op2, int? outputAddr)
        {
            if(!op1.HasValue || !op2.HasValue || !outputAddr.HasValue)
            {
                Console.WriteLine("Add operation failed");
                return;
            }

            SetValue(outputAddr.Value, op1.Value+op2.Value);
        }


        private void MultOp(long? op1, long? op2, int? outputAddr)
        {
            if(!op1.HasValue || !op2.HasValue || !outputAddr.HasValue)
            {
                Console.WriteLine("Multiplication operation failed");
                return;
            }

            SetValue(outputAddr.Value, op1.Value*op2.Value);
        }

        private void StoreInputOp(int? outputAddr, bool getInputFromUser, long? input = 0)
        {
            if(!outputAddr.HasValue)
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
                    long temp;
                    if(!Int64.TryParse(Console.ReadLine(), out temp))
                    {
                        Console.WriteLine("Not a valid Input");
                        return;
                    }
                    input = temp;
                }
                else
                {
                    input = Inputs[inputIndex];
                    if(debugMessages)
                        Console.WriteLine(String.Format("Using preset Input: {0}",input));
                    inputIndex++;
                }

            }

            SetValue(outputAddr.Value,input.Value);
        }

        public long GetLastOutput()
        {
            return lastOutput;
        }

        private void OutputOp(long? infoAddr)
        {
            if(!infoAddr.HasValue)
            {
                Console.WriteLine("Output op failed");
                return;
            }

            lastOutput = infoAddr.Value;
            if(debugMessages)
                Console.WriteLine(String.Format("Output Command: {0}",infoAddr.Value));
        }

        private void SetValue(int index, long? value)
        {
            if(!value.HasValue)
                return;

            while(index>Instructions.Count-1)
            {
                Instructions.Add(0);
            }

            Instructions[index] = value.Value;
        }  
    }
}