using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    class AdventX
    {
        List<List<string>> map = new List<List<string>>();

        public AdventX(List<string> lines)
        {
            
            foreach(var line in lines)
            {
                map.Add(line.Select(x => x.ToString()).ToList<string>());
            }

        }
    }    
}