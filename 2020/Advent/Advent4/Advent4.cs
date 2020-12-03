using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    class Advent4
    {
        List<List<string>> map = new List<List<string>>();

        public Advent4(List<string> lines)
        {
            
            foreach(var line in lines)
            {
                map.Add(line.Select(x => x.ToString()).ToList<string>());
            }

        }
    }    
}