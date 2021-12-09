using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    class SevenSegDisplay
    {
        List<int> entries;
        Dictionary<List<string>, List<string>> inputLines = new Dictionary<List<string>, List<string>>();

        public SevenSegDisplay(List<string> lines)
        {
            foreach(var line in lines)
            {
                var part1 = line.Trim().Split('|')[0].Trim().Split(' ').ToList();
                var part2 = line.Trim().Split('|')[1].Trim().Split(' ').ToList();
                inputLines.Add(part1, part2);
            }
            //Part 1
            Console.WriteLine(String.Format("Total number of 1s, 8s, 7s, and 4s in output: {0}",Part1()));
            double tot = 0;
            foreach(var line in inputLines)
            {
                tot+= DecodeMethod2(line.Key, line.Value);
            }

            Console.WriteLine(String.Format("Total of all outputs: {0}",tot));
        }

        private int Part1()
        {
            var tot = 0;
            foreach(var output in inputLines.Values)
            {
                foreach(var dec in output)
                {
                    var len = dec.Length;
                    if (len== 2 || len ==3 || len==4 || len==7)
                        tot++;
                }
            }
            return tot;
        }

        private double Decode(List<string> input, List<string> output)
        {
            var rep1 = "";
            var rep7 = "";
            var rep4 = "";

            Dictionary<string, string> map = new Dictionary<string, string>();

            foreach(var entry in input)
            {
                if (entry.Length ==2)
                    rep1 = entry;
                if (entry.Length ==3)
                    rep7 = entry;
                if (entry.Length ==4)
                    rep4 = entry;
            }

            var mapa = rep7.First(x=>!rep1.Contains(x)).ToString();
            map[mapa] = "a";

            var rep9 = input.First(x=> x.Length == 6 && rep4.All(y=>x.Contains(y)));
            var mapsAG = string.Join("",rep9.Where(x=> !rep4.Contains(x)));
            map[mapsAG.Replace(mapa, "")] = "g";

            Dictionary<int,string>  segMap = new Dictionary<int, string>();
            segMap.Add(6,"b");
            segMap.Add(8,"c");
            segMap.Add(7,"d");
            segMap.Add(4,"e");
            segMap.Add(9,"f");

            var all = string.Join("",input);
            var charFreqs = all.Distinct().ToDictionary(x=>x, x=> all.Count(u=> u==x));
            foreach(var charFreq in charFreqs)
            {
                if(map.Keys.ToList().Contains(charFreq.Key.ToString()))
                    continue;
                
                map.Add(charFreq.Key.ToString(), segMap[charFreq.Value]);
            }

            var stringLists = new List<string>();
            stringLists.Add("abcefg");
            stringLists.Add("cf");
            stringLists.Add("acdeg");
            stringLists.Add("acdfg");
            stringLists.Add("bcdf");
            stringLists.Add("abdfg");
            stringLists.Add("abdefg");
            stringLists.Add("acf");
            stringLists.Add("abcdefg");
            stringLists.Add("abcdfg");

            double tot = 0;
            output.Reverse();
            for(int i = 0; i<output.Count(); i++)
            {
                var translated = string.Join("",output[i].Select(x=> map[x.ToString()]).OrderBy(x=>x));
                var num = stringLists.IndexOf(translated);

                tot+= Math.Pow(10,i)*num;
            }

            return tot;
        }

        private double DecodeMethod2(List<string> input, List<string> output)
        {
            var reps = input.Select(x=> "").ToList();



            foreach(var entry in input)
            {
                if (entry.Length ==2)
                    reps[1] = entry;
                if (entry.Length ==3)
                    reps[7] = entry;
                if (entry.Length ==4)
                    reps[4] = entry;
                if (entry.Length ==7)
                    reps[8] = entry;
            }

            var remaining = input.Where(x => !reps.Contains(x)).ToList();
            foreach(var entry in remaining)
            {
                if (entry.Length ==6 && reps[4].All(x => entry.Contains(x)))
                    reps[9] = entry;
                else if (entry.Length ==5 && reps[7].All(x => entry.Contains(x)))
                    reps[3] = entry;
                else if (entry.Length ==6 && !remaining.Any(x => x.Length==5 && x.All(y=> entry.Contains(y))))
                    reps[0] = entry;
            }

            remaining = input.Where(x => !reps.Contains(x)).ToList(); 
            reps[6] = remaining.First(x=> x.Length==6);
            reps[5] = remaining.First(x=> x.Length == 5 && x.All(y => reps[6].Contains(y)));
            remaining = input.Where(x => !reps.Contains(x)).ToList(); 
            reps[2] = remaining.First();


            Dictionary<string, int> map = new Dictionary<string, int>();
            for(int i = 0; i<reps.Count(); i++)
                map.Add(string.Join("",reps[i].OrderBy(x=>x)), i);

            double tot = 0;
            output.Reverse();
            for(int i = 0; i<output.Count(); i++)
            {
                var ordered = string.Join("",output[i].OrderBy(x=>x));
                var num = map[ordered];

                tot+= Math.Pow(10,i)*num;
            }

            return tot;
        }
    }    
}