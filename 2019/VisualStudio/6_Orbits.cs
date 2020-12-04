using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    public class Orbits
    {
        Dictionary<string,string> orbitsList = new Dictionary<string, string>();

        public Orbits(List<string> inputs)
        {
            foreach(var input in inputs)
            {
                var parts = input.Split(')');
                orbitsList.Add(parts[1],parts[0]);
            }
        }

        private int CountOrbits(string planet)
        {
            int count = 0;
            string key = planet;
            while(orbitsList.Keys.Contains(key))
            {
                //Console.WriteLine(String.Format("{0} orbits {1}", key, orbitsList[key]));
                count++;
                key = orbitsList[key];
            }

            //Console.WriteLine(String.Format("Planet {0} has {1} direct and inderect orbits", planet, count));
            return count;
        }

        public void PrintTotalDirectAndIndirectOrbits()
        {
            var total = 0;
            foreach(var orbiter in orbitsList.Keys)
            {
                total += CountOrbits(orbiter);
            }
            
            Console.WriteLine(String.Format("Total Direct and Indirect Orbits: {0}", total));
        }


        public void PrintOrbitalTransfersToSanta()
        {
            var yourOrbitList = GetOrbitList("YOU");
            var santaOrbitList = GetOrbitList("SAN");

            var firstCommon = yourOrbitList.First(x => santaOrbitList.Contains(x));
            var transfers = CountOrbits("YOU")+CountOrbits("SAN")-2*(CountOrbits(firstCommon)+1);

            Console.WriteLine(String.Format("Orbit Transfer Count: {0}", transfers));
        }

        private List<string> GetOrbitList(string startingPlanet)
        {
            var ret = new List<string>();
            var key = startingPlanet;
            while(orbitsList.ContainsKey(key))
            {
                ret.Add(key);
                key = orbitsList[key];
            }

            return ret;
        } 

    }
}