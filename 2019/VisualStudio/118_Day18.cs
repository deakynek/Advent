using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

namespace Advent
{
    public class Day18
    {

        List<string> equations = new List<string>();
        public Day18(List<string> inputs)
        {

            equations = inputs;
            long totalAns = 0;
            foreach(var eq in equations)
            {
                totalAns += RecurseEq(eq.Split(' ').ToList());
            }
            Console.WriteLine("Part 1: sum of all answers - " + totalAns);
            
            
            totalAns = 0;
            foreach(var eq in equations)
            {
                var newEq = FixEq(eq.Split(' ').ToList());
                totalAns += RecurseEq(newEq);
            }
            Console.WriteLine("Part 2: sum of all answers - " + totalAns);
       

        }

        public long RecurseEq(List<string> ops)
        {
            long? val = null;
            int i = 0;
            bool? sumOp = null;
            while(i < ops.Count)
            {
                var iadder = 1;
                long thisVal = 0;
                if(ops[i] == "+" || ops[i] == "*")
                    continue;
                if(ops[i].Contains("("))
                {
                    var matchingEndingIndex = ops.Count()-1;
                    int parenCount = 0;
                    for(int j = i; j<ops.Count(); j++)
                    {
                        if(ops[j].Contains("("))
                        {
                            parenCount+=ops[j].Where(x => x =='(').Count();
                        }
                        if(ops[j].Contains(")"))
                        {
                            parenCount -= ops[j].Where(x => x ==')').Count();
                            if(parenCount == 0)
                            {
                                matchingEndingIndex = j;
                                break;
                            }
                        }
                    }

                    var inner = ops.GetRange(i,matchingEndingIndex-i+1).ToList();
                    inner[0] = inner[0].Substring(1);
                    inner[inner.Count()-1] = inner[inner.Count()-1].Substring(0,inner[inner.Count()-1].Count()-1);
                    thisVal = RecurseEq(inner);
                    iadder = inner.Count();
                }
                else
                {
                    thisVal = Int32.Parse(ops[i]);
                    iadder = 1;
                }


                if(!val.HasValue)
                    val = thisVal;
                else if(sumOp.HasValue)
                {
                    if(sumOp.Value)
                        val += thisVal;
                    else
                        val *= thisVal;

                }
                
                if(i+iadder>= ops.Count)
                {
                    sumOp = null;
                }
                else if(ops[i+iadder]== "+")
                {
                    sumOp = true;
                }
                else if(ops[i+iadder]=="*")
                {
                    sumOp = false;
                }
                else
                {
                    sumOp = null;
                }

                i += iadder +1;

            }

            return val.HasValue? val.Value : 0;
        }


        private List<string> FixEq(List<string> eq)
        {
            var copy = eq.ToList();
            for(int i = 0; i<eq.Count(); i++)
            {
                if(copy[i] != "+")
                    continue;

                var startInd = i-1;
                var endId = i+1;

                if(copy[i-1].Contains(")"))
                {
                    var parenCount = 0;
                    for(int j = i-1; j>=0; j--)
                    {
                        parenCount += eq[j].Where(x => x==')').Count();
                        parenCount -= eq[j].Where(x => x=='(').Count();

                        if(parenCount <= 0)
                        {
                            startInd = j;
                            break;
                        }                            
                    }
                }

                if(copy[i+1].Contains("("))
                {
                    var parenCount = 0;
                    for(int j = i+1; j<copy.Count(); j++)
                    {
                        parenCount += eq[j].Where(x => x=='(').Count();
                        parenCount -= eq[j].Where(x => x==')').Count();

                        if(parenCount <= 0)
                        {
                            endId = j;
                            break;
                        }                            
                    }
                }    

                eq[startInd] = "("+eq[startInd];            
                eq[endId] = eq[endId]+")"; 
            }

            return eq;
        }
    }
}