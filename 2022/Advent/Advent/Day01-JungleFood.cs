using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent
{
    internal class JungleFood
    {
        List<int> quatities = new List<int>();

        public JungleFood(List<string> inputs)
        {
            bool skip = false;
            var tot = 0;

            foreach(var line in inputs)
            {
                if(String.IsNullOrEmpty(line))
                {
                    skip = true;
                    quatities.Add(tot);
                    continue;
                }

                if(skip)
                {
                    skip = false;
                    tot = 0;
                }
                tot += Int32.Parse(line);

                if(line == inputs.Last())
                    quatities.Add(tot);
            }


            Console.WriteLine(string.Format("Maximum Food = {0}", quatities.Max()));

            quatities.Sort();
            quatities.Reverse();
            var top3 = quatities.ElementAt(0) + quatities.ElementAt(1) + quatities.ElementAt(2);

            Console.WriteLine(string.Format("Top 3 Total = {0}", top3));


        }

    }
}
