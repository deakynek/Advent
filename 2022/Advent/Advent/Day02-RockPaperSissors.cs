using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent
{
    internal class RockPaperSissors
    {
        List<Tuple<int>> games = new List<Tuple<int>>();

        public RockPaperSissors(List<string> inputs)
        {
            Part1(inputs);
            Part2(inputs);
        }

        private void Part1(List<string> inputs)
        {       
            var tot = 0;

            foreach(var line in inputs)
            {
                var splitLine = line.Replace("A","1")
                                  .Replace("B","2")
                                  .Replace("C","3")
                                  .Replace("X","1")
                                  .Replace("Y","2")
                                  .Replace("Z","3")
                                  .Split(" ");

                var mine = Int16.Parse(splitLine.ElementAt(1));
                var theirs = Int16.Parse(splitLine.ElementAt(0));

                tot+= mine;
                if(mine == theirs)
                    tot += 3;
                else
                {
                    tot += mine == 1 && theirs ==3 ||
                            mine == 2 && theirs ==1 ||
                            mine == 3 && theirs ==2      ? 6 :0;
                }


            }

            Console.WriteLine(string.Format("Round 1 Score = {0}", tot));
        }
        private void Part2(List<string> inputs)
        {  
            var tot = 0;
            foreach(var line in inputs)
            {
                var splitLine = line.Replace("A","1")
                                  .Replace("B","2")
                                  .Replace("C","3")
                                  .Replace("X","1")
                                  .Replace("Y","2")
                                  .Replace("Z","3")
                                  .Split(" ");

                var op = Int16.Parse(splitLine.ElementAt(1));
                var theirs = Int16.Parse(splitLine.ElementAt(0));
                var mine = 0;

                if(op == 1) //lose
                {
                    mine = theirs%3 -1;
                    if(mine<1)
                        mine+=3;
                }
                else if(op ==2) //tie
                {
                    mine = theirs;
                }
                else if(op ==3) //win
                {
                    mine = theirs%3+1;
                }

                tot+= mine;
                if(mine == theirs)
                    tot += 3;
                else
                {
                    tot += mine == 1 && theirs ==3 ||
                            mine == 2 && theirs ==1 ||
                            mine == 3 && theirs ==2      ? 6 :0;
                }
            }

            Console.WriteLine(string.Format("Round 2 Score = {0}", tot));
        }

    }
}