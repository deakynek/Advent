using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace Advent
{
    class Adapters
    {
        List<long> volts = new List<long>();
        long deviceAdapterRating = 0;

        Dictionary<long,Tuple<List<long>,long>> ComboDictionary = new Dictionary<long, Tuple<List<long>, long>>();

        public Adapters(List<string> lines)
        {
            volts = lines.Select(x => Int64.Parse(x)).ToList();
            deviceAdapterRating = volts.Max() +3;
            volts.Add(0);
            volts.Add(deviceAdapterRating);
            UseEveryAdapter();
            GetNumberOfValidCombos();
        }
        
        private void UseEveryAdapter()
        {
            long input = 0;
            var diff1 = 0;
            var diff3 = 0;
            foreach(var adapter in volts.OrderBy(x=>x))
            {
                if(adapter-input > 4)
                    continue;

                if(adapter-input == 1)
                    diff1++;

                if(adapter-input == 3)
                    diff3++;

                input = adapter;
            }

            if(deviceAdapterRating-input == 1)
                diff1++;

            if(deviceAdapterRating- input == 3)
                diff3++;

            Console.WriteLine(String.Format("1 diff : {0}, 3 diff : {1}, mult : {2}",diff1,diff3,diff1*diff3));
        }

        private void GetNumberOfValidCombos()
        {
            Dictionary<long,long> OtherCombos = new Dictionary<long, long>();
            foreach(var adapter in volts.OrderBy(x=>x))
            {
                if(adapter == 0)
                {
                    OtherCombos[adapter] = 1; 
                    continue;
                }

                long Combos = 0;
                for(int i = 1; i<4; i++)
                {
                    if(volts.Contains(adapter-i))
                        Combos += OtherCombos[adapter-i];
                }

                OtherCombos.Add(adapter, Combos);
            }

            Console.WriteLine("Part 2 - combos: " + OtherCombos[deviceAdapterRating]);
        }

            

    }

}