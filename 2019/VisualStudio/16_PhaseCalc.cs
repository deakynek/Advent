using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    public class PhaseCalculator
    {
        List<int> initialPhase = new List<int>();

        public PhaseCalculator(List<string> inputs)
        {
            string start = inputs[0];
            SetUpMatrixAndInitial(start);
        }

        private void SetUpMatrixAndInitial(string start)
        {
            initialPhase.Clear();
            List<int> phases = new List<int>(){0,1,0,-1};

            for(int i = 0; i<start.Length; i++)
            {
                initialPhase.Add(Int32.Parse(start.Substring(i,1)));
            }
        }


        public void UpdateToRealSignal(List<string> inputs)
        {
            string start = inputs[0];

            start = string.Concat(Enumerable.Repeat(inputs[0], 10000));

            SetUpMatrixAndInitial(start);

            int offset = Int32.Parse(start.Substring(0,7));

            var currPhase = initialPhase.ToList();
            var iter = 1;
            while(iter <= 100)
            {
                Console.WriteLine("Iter: " + iter.ToString());
                currPhase = ProgressPhase(currPhase, offset);
                iter++;
            }



            var output = "";
            var finalPhase = currPhase.Skip(offset).Take(8);
            foreach(var val in finalPhase)
            {
                output += val.ToString();
            }
            Console.WriteLine("Final Phase : " + output.ToString());

        }

        private List<int> ProgressPhase(List<int> currPhase, int offset)
        {
            var newPhase = currPhase.ToList();
            var sumList = newPhase.ToList();
            var runningTotal=0;

            for(int i = currPhase.Count-1; i >=offset; i--)
            {
                var thistotal = runningTotal+currPhase[i];
                runningTotal =thistotal;
                sumList[i] = thistotal;


                bool add = true;
                int n = 2;
                while(n*(i+1)-1 < sumList.Count())
                {
                    if(n%2 == 0)
                        add = !add;
                    
                    if(add)
                        thistotal += sumList[n*(i+1)-1];
                    else
                        thistotal -= sumList[n*(i+1)-1];
                    
                    n++;
                }

                newPhase[i] = Math.Abs(thistotal%10);

            }
            return newPhase;
        }

        public void ProgressANumberOfTimes(int iter)
        {
            int count = 1;
            var phase = initialPhase.Select(x=> x).ToList();
            while(count <= iter)
            {
                phase = ProgressPhase(phase,0);
                count++;
            }   

            string output = "";
            foreach(var p in phase)
            {
                output+=p.ToString();
            }
            Console.WriteLine("Final phase after {0} iterations: {1}",iter,output.Substring(0,8));
        }

        
    }
}