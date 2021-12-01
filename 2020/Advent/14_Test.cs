using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace Advent
{
    class Test
    {
        Dictionary<long, long> mem = new Dictionary<long, long>();
        List<List<Tuple<int,int>>> instructions = new  List<List<Tuple<int,int>>>();
        List<Dictionary<int,string>> masks = new List<Dictionary<int,string>>();
        List<Dictionary<int,string>> masks2 = new List<Dictionary<int,string>>();
        public Test(List<string> lines)
        {
            foreach(var line in lines)
            {
                if(line.Contains("mask"))
                {
                    instructions.Add(new List<Tuple<int,int>>());

                    var thisMask = new Dictionary<int,string>();
                    var thisMask2 = new Dictionary<int,string>();
                    var maskString = line.Skip(7).Reverse().ToList();
                    for(int i = 0; i< maskString.Count(); i++)
                    {
                        if(maskString[i]== 'X')
                        {
                            thisMask2.Add(i, "X");
                            continue;
                        }
                        else if(maskString[i] == '1')
                        {
                            thisMask2.Add(i,"1");
                        }

                        thisMask.Add(i, maskString[i].ToString());
                    }
                    masks.Add(thisMask);   
                    masks2.Add(thisMask2);
                    continue;                 
                }

                var splitList = new List<char>(){'[',']',' '};
                var splitInst = line.Split(splitList.ToArray());

                var addr = Int32.Parse(splitInst[1]);
                var val = Int32.Parse(splitInst[4]);
                instructions.Last().Add(new Tuple<int, int>(addr,val));
            }



            RunInstructions();
            mem.Clear();
            RunInstructions2();
        }

        private void RunInstructions()
        {
            for(int i = 0; i< instructions.Count(); i++)
            {
                var thisMask = masks[i];

                foreach(var inst in instructions[i])
                {
                    var val = Convert.ToString(inst.Item2,2);
                    while(val.Count()<36)
                    {
                        val = val.Insert(0,"0");
                    }

                    val = Reverse(val);
                    
                    foreach(var mask in thisMask)
                    {
                        
                        val = val.Remove(mask.Key,1);
                        val = val.Insert(mask.Key, mask.Value);
                    }

                    var final = Convert.ToInt64(Reverse(val),2);

                    if(!mem.ContainsKey(inst.Item1))
                    {
                        mem.Add(inst.Item1, final);
                    }    
                    else
                    {
                        mem[inst.Item1] = final;
                    }
                }
            }

            Console.WriteLine("Part 1 - sum of all values :" + (mem.Values.Sum()));
        }


        private void RunInstructions2()
        {
            for(int i = 0; i< instructions.Count(); i++)
            {
                var thisMask = masks2[i];

                var powersOfTwo = thisMask.Where(x => x.Value == "X").Select(x => x.Key).ToList();
                var addrAdders = new List<long>(){0};
                foreach(var power in powersOfTwo)
                {
                    var copy = addrAdders.ToList();
                    foreach(var curr in copy)
                    {
                        addrAdders.Add(curr+(long)Math.Pow(2,power));
                    }

                }

                foreach(var inst in instructions[i])
                {
                    var addr = Convert.ToString(inst.Item1,2);
                    while(addr.Count()<36)
                    {
                        addr = addr.Insert(0,"0");
                    }

                    addr = Reverse(addr);
                    
                    
                    foreach(var mask in thisMask)
                    {
                        if(mask.Value == "X")
                        {
                            addr = addr.Remove(mask.Key,1);
                            addr = addr.Insert(mask.Key, "0");
                        }
                        else
                        {
                            addr = addr.Remove(mask.Key,1);
                            addr = addr.Insert(mask.Key, "1");
                        }
                    }

                    var finalAddrBase = Convert.ToInt64(Reverse(addr),2);

                    foreach(var address in addrAdders)
                    {
                        var thisaddress = address+finalAddrBase;
                        if(!mem.ContainsKey(thisaddress))
                        {
                            mem.Add(thisaddress, inst.Item2);
                        }    
                        else
                        {
                            mem[thisaddress] = inst.Item2;
                        }
                    }
                }
            }

            Console.WriteLine("Part 2 - sum of all values :" + (mem.Values.Sum()));
        }

        public static string Reverse( string s )
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse( charArray );
            return new string( charArray );
        }        

    }

}