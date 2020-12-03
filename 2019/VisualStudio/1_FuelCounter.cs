using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;


namespace Advent
{
    public class FuelCounter
    {
        List<int> FuelReq1 = new List<int>();
        List<int> FuelReq2 = new List<int>();

        public FuelCounter(List<string> input)
        {
            foreach(var entry in input)
            {
                FuelReq1.Add(FuelCalc(Int32.Parse(entry)));
            }

            foreach(var entry in input)
            {
                var fuel = 0;
                int mass = Int32.Parse(entry);

                while(mass > 0)
                {
                    mass =  FuelCalc(mass);
                    fuel += mass;
                }
                FuelReq2.Add(fuel);
            }

        }

        private int FuelCalc(int mass)
        {
            var calc = mass/3 - 2;
            if(calc <0)
            {
                calc = 0;
            }
            return calc;
        }

        public void PrintTotalFuel1()
        {
            var Fuel= 0;
            FuelReq1.ForEach(x => Fuel+=x);

            Console.WriteLine(String.Format("Total Fuel Calc 1: {0}",Fuel));
        }

        public void PrintTotalFuel2()
        {
            var Fuel= 0;
            FuelReq2.ForEach(x => Fuel+=x);

            Console.WriteLine(String.Format("Total Fuel Calc 2: {0}",Fuel));
        }
    }

}