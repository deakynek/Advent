using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    class OctoFlash
    {
        List<int> entries = new List<int>();

        public OctoFlash(List<string> lines)
        {
            foreach(var line in lines)
            {
                entries.AddRange(line.Trim().Select(x=>int.Parse(x.ToString())));
            }

            bool All = false;
            int tot = 0;
            int j = 0;
            for(j = 0;j<100; j++)
            {
                tot += DoStep(out All);
            }

            Console.WriteLine(tot);


            while(!All)
            {
                DoStep(out All);
                j++;
            }

            Console.WriteLine(j);
        }

        private int DoStep(out bool All)
        { 
            All = false;
            entries = entries.Select(x=>x+1).ToList();

            if (!entries.Any(x=>x>9))
                return 0;

            var currentIndex = 0;
            List<int> flashPos = new List<int>();
            for(int i= 0; i<100; i++)
            {
                if(entries[i]>9)
                    flashPos.Add(i);
            }

            while(currentIndex < flashPos.Count())
            {
                var currPos = flashPos[currentIndex];
                List<int> posAdders = new List<int>(){-10,10};
                if(currPos%10!= 0)
                    posAdders.AddRange(new List<int>(){-1,-11,9});
                if(currPos%10 != 9)
                    posAdders.AddRange(new List<int>(){1,-9,11});

                foreach(var pos in posAdders)
                {
                    var addPos = pos+flashPos[currentIndex];
                    if(addPos < 0 || addPos>99 || flashPos.Contains(addPos))
                        continue;

                    entries[addPos]++;
                    if(entries[addPos]>9)
                        flashPos.Add(addPos);
                }
                
                currentIndex++;
            }

            foreach(var pos in flashPos)
            {
                entries[pos] = 0;
            }

            if(flashPos.Count() == 100)
                All = true;

            return flashPos.Count();
        }
    }    
}