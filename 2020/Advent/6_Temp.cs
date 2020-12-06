using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace Advent
{
    class Customs
    {
        
        List<List<string>> GroupForms = new List<List<string>>();
        public Customs(List<string> lines)
        {
            List<string> group = new List<string>();

            foreach(var line in lines)
            {
                if(line == "")
                {
                    GroupForms.Add(group);
                    group = new List<string>();
                    continue;
                }

                group.Add(line);

                if(line == lines.Last())
                {
                    GroupForms.Add(group);
                }
            }
        }

        public void PrintGroupsTotal()
        {
            int totals = 0;

            foreach(var group in GroupForms)
            {
                List<char> thisGroupForm = new List<char>();

                foreach(var person in group)
                {
                    thisGroupForm.AddRange(person.ToList());
                }

                totals += thisGroupForm.Distinct().Count();
            }

            Console.WriteLine("Part 1 Total : " + totals);
            totals = 0;

            foreach(var group in GroupForms)
            {
                List<char> thisGroupForm = new List<char>();

                foreach(var person in group)
                {
                    thisGroupForm.AddRange(person.ToList());
                }

                int allAnswered = 0;
                foreach (var question in thisGroupForm.Distinct())
                {
                    if(thisGroupForm.Count(x=> x==question) ==group.Count())
                        allAnswered++;
                }
                totals += allAnswered;
            }

            Console.WriteLine("Part 2 Total : " + totals);

        }


    }
}
