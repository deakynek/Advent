using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    public class EngineThrusters
    {
        IntComputer engineProgram;
        List<IntComputer> engineList = new List<IntComputer>();

        bool UseFeedback = false;
        public EngineThrusters(List<string> inputs)
        {
            engineProgram = new IntComputer(inputs);

            while(engineList.Count<5)
            {
                engineList.Add(new IntComputer(inputs));

            }
        }

        private void ResetAllEngines()
        {
            foreach(var eng in engineList)
            {
                eng.ResetProgram();
            }
        }

        public void RunThroughPossibleCombos()
        {
            var Phases= new List<int>(){0,1,2,3,4};
            UseFeedback = false;

            var maxThrust = RecurseCombos(new List<int>(), Phases);
            Console.WriteLine("Maximum Thrust : {0}", maxThrust);
        }

        public void RunThroughPossibleCombosWithFeedback()
        {
            var Phases= new List<int>(){5,6,7,8,9};
            UseFeedback = true;

            var maxThrust = RecurseCombos(new List<int>(), Phases);
            Console.WriteLine("Maximum Thrust : {0}", maxThrust);
        }

        private long RecurseCombos(List<int> currentSet, List<int> remainingOptions)
        {
            if(remainingOptions.Count == 0)
            {
                long output = 0;
                string combo = "";
                currentSet.ForEach(x => combo+=x.ToString());
                Console.WriteLine(combo);

                if(!UseFeedback)
                {
                    foreach(var phase in currentSet)
                    {
                        engineProgram.ResetProgram();
                        engineProgram.SetInputs(new List<long>(){phase, output});
                        engineProgram.RunProgram(0,false);
                        output = engineProgram.GetLastOutput();
                    }
                }
                else
                {
                    ResetAllEngines();
                    for(int i = 0; i<currentSet.Count; i++)
                    {
                        engineList[i].SetInputs(new List<long>(){currentSet[i]});
                    }

                    long nextInput = 0;
                    int currentEngine = 0;
                    while(!engineList.Last().programComplete)
                    {
                        engineList[currentEngine].AddInput(nextInput);
                        engineList[currentEngine].RunProgram(engineList[currentEngine].nextStepIndex, false, true);
                        
                        
                        var temp = engineList[currentEngine].lastOutput;

                        //Console.WriteLine(String.Format("Engine {0}:  input={1} output={2}",currentEngine,nextInput,temp));

                        nextInput = temp;
                        currentEngine++;
                        if(currentEngine >= engineList.Count)
                            currentEngine= currentEngine-engineList.Count;

                    }
                    Console.WriteLine("\t"+engineList.Last().lastOutput.ToString());
                    output = engineList.Last().lastOutput;
                }

                return output;
            }

            long max = 0;
            foreach(var option in remainingOptions)
            {
                var copyCurrentSet = currentSet.Select(x=>x).ToList<int>();
                copyCurrentSet.Add(option);
                var copyRemainingSet = remainingOptions.Where(x=> x!= option).ToList<int>();

                
                var thrusterOutput = RecurseCombos(copyCurrentSet, copyRemainingSet);
                if(thrusterOutput> max)
                    max = thrusterOutput;
            }

            return max;
        }

    }
}