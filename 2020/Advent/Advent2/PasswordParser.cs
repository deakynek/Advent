using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    class PasswordParser
    {
        List<Tuple<string, int,int, string, bool, bool>> entries;

        public PasswordParser(List<string> lines)
        {
            entries = new List<Tuple<string, int, int, string, bool, bool>>();

            foreach(string entry in lines)
            {
                var values = entry.Split(("-: ").ToArray<char>());
                int min = Int32.Parse(values[0]);
                int max = Int32.Parse(values[1]);
                string requirement = values[2];

                string password = values[4];

                var count1 = password.Where(ch => ch == requirement[0]).Count();
                var valid1 = count1>=min && count1 <=max;
                
                var valid2 = password[min-1] == requirement[0] ^ password[max-1] == requirement[0];

                entries.Add(new Tuple<string, int,int, string, bool,bool>(requirement,min,max,password,valid1, valid2));
            }
        }

        public int FindValidPasswordCountByRange()
        {
            return entries.Where(ent => ent.Item5).Count();
        }
        public int FindValidPasswordCountByPosition()
        {
            return entries.Where(ent => ent.Item6).Count();
        }
    }    
}