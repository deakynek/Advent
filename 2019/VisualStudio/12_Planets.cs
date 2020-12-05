using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    public class Planets
    {
        List<List<int>> planetPositions = new List<List<int>>();
        List<List<int>> planetVelocities = new List<List<int>>();
        List<bool> posBackToInitial = new List<bool>(){false, false, false}; 
        
        List<List<int>> planetInitialPositions = new List<List<int>>();


        public Planets(List<string> inputs)
        {
            foreach(var input in inputs)
            {
                var split = input.Split((new List<char>(){'=',',','>'}).ToArray());

                var pos = new List<int>(){Int32.Parse(split[1]),
                                            Int32.Parse(split[3]),
                                            Int32.Parse(split[5])};

                var vel = new List<int>(){0,0,0};
                planetVelocities.Add(vel);
                planetInitialPositions.Add(pos);
            }
            planetPositions = planetInitialPositions.Select(x=>x.Select(y=>y).ToList()).ToList();

        }

        private void AdvancePlanets()
        {
            var prevPos = planetPositions.ToList();
            var prevVel = planetVelocities.ToList();

            //Calc Velocities
            for(int p = 0; p < prevPos.Count; p++)
            {
                for(int op = 0; op< prevPos.Count; op++)
                {
                    if(p == op)
                        continue;

                    for(int i = 0; i < prevPos[p].Count; i++)
                    {

                        if(prevPos[p][i] > prevPos[op][i])
                            planetVelocities[p][i]--;
                        else if (prevPos[p][i] < prevPos[op][i])
                            planetVelocities[p][i]++;

                    }
                }
            }

            //Advance Position
            for(int i = 0; i< planetPositions.Count; i++)
            {
                for(int v = 0; v < planetPositions[i].Count; v++)
                {
                    planetPositions[i][v] += planetVelocities[i][v];
                }
            }
        }

        public void MovePlanets()
        {
            int movement = 0;

            while(movement < 1000)
            {
                AdvancePlanets();
                movement++;
            }

            int TotalEnergy = 0;
            for(int i = 0; i< planetPositions.Count; i++)
            {
                int KineticEnergy = 0;
                int PotentialEnergy = 0;
                for(int v = 0; v < planetPositions[i].Count; v++)
                {
                    PotentialEnergy += Math.Abs(planetPositions[i][v]);
                    KineticEnergy += Math.Abs(planetVelocities[i][v]);

                }

                Console.WriteLine(String.Format("Planet {0}: Potential- {1}  Kinetic- {2}",i,PotentialEnergy,KineticEnergy));
                TotalEnergy += KineticEnergy*PotentialEnergy;
            }

            Console.WriteLine(String.Format("Total Energy - {0}",TotalEnergy));

            Reset();

            List<long> cyclesToRepeat = new List<long>();
            long step = 0;
            while(posBackToInitial.Any(repeat => !repeat))
            {
                step++;

                if(step% 1000000 ==0)
                    Console.WriteLine((step/100000).ToString() + " Million Steps");

                AdvancePlanets();

                
                for(int i = 0; i < planetPositions[0].Count; i++)
                {
                    if(!posBackToInitial[i] && CheckInitialPosAndVel(i))
                    {
                        posBackToInitial[i] = true;
                        cyclesToRepeat.Add(step);
                        Console.WriteLine(String.Format("Planets Position {0} repeats after {1} steps",i,step));
                    }                  
                }
            }

            long minSteps = 1;
            for(int i =0; i < cyclesToRepeat.Count; i++)
            {
                minSteps= MoreMath.getLCM(minSteps, cyclesToRepeat[i]);
            }

            Console.WriteLine("Part 2 - Min steps to repeat " + minSteps.ToString());
        }

        private bool CheckInitialPosAndVel(int pos)
        {
            for(int i = 0; i < planetPositions.Count; i++)
            {
                if(planetPositions[i][pos] != planetInitialPositions[i][pos])
                    return false;

                if(planetVelocities[i][pos] !=0)
                    return false;
            }

            return true;
        }

        private bool CheckInitialVel(int planet)
        {
            return  planetVelocities[planet][2] == 0 && 
                    planetVelocities[planet][1] == 0 && 
                    planetVelocities[planet][0] == 0;            
        }
        public void Reset()
        {
            planetPositions = planetInitialPositions.Select(x=>x.Select(y=>y).ToList()).ToList();
            
            planetVelocities.Clear();
            foreach(var p in planetPositions)
            {
                planetVelocities.Add(new List<int>(){0,0,0});
            }
        }    

    }

}