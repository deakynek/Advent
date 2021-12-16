using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    class Brackets
    {
        List<int> entries;

        public Brackets(List<string> lines)
        {
            Dictionary<char,char> brackets = new Dictionary<char, char>();
            brackets.Add('[',']');
            brackets.Add('{','}');
            brackets.Add('(',')');
            brackets.Add('<','>');

            Dictionary<char, int> cost = new Dictionary<char, int>();
            cost.Add(']',57);
            cost.Add('>',25137);
            cost.Add(')',3);
            cost.Add('}',1197);

            Dictionary<char, int> score = new Dictionary<char, int>();
            score.Add('[',2);
            score.Add('<',4);
            score.Add('(',1);
            score.Add('{',3);

            var totCost = 0;
            var scores = new List<long>();
            foreach(var line in lines.Select(x=>x.Trim()))
            {
                var openingCharacters = new List<char>();
                var illegalChar = ' ';
                for(int i = 0; i< line.Length;i++)
                {
                    if(brackets.Keys.Contains(line[i]))
                    {
                        openingCharacters.Add(line[i]);
                    }
                    else if(line[i]==brackets[openingCharacters.Last()])
                    {
                        openingCharacters = openingCharacters.Take(openingCharacters.Count() -1).ToList();
                    }
                    else
                    {
                        illegalChar = line[i];
                        break;
                    }
                }
                if(illegalChar == ' ')
                {
                    openingCharacters.Reverse();
                    long thisScore = 0;
                    foreach(var open in openingCharacters)
                    {
                        thisScore = thisScore*5 + score[open];
                    }
                    scores.Add(thisScore);
                }
                else
                    totCost += cost[illegalChar];

            }

            Console.WriteLine(totCost);
            Console.WriteLine(scores.OrderBy(x=>x).Skip(scores.Count()/2).First());
        }
    }    
}