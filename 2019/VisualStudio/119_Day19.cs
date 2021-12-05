using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

namespace Advent
{
    public class Day19
    {

        Dictionary<int, string> rules = new Dictionary<int, string>();
        Dictionary<int, string> compositeReqExes  = new Dictionary<int, string>();

        List<string> passwords = new List<string>();

        public Day19(List<string> inputs)
        {
            var rulesDone = false;

            foreach(var input in inputs)
            {
                if(input == "")
                {
                    rulesDone = true;
                    continue;
                }

                if(!rulesDone)
                {
                    var split = input.Split(':');
                    var rule = Int32.Parse(split[0]);
                    var text = split[1];

                    rules.Add(rule, text);
                    continue;
                }

                passwords.Add(input);
            }

            FillRegExes();

            FindPasswordsThatMatch0();
        }

        private void FillRegExes()
        {
            while(compositeReqExes.Keys.Count() < rules.Keys.Count())
            {
                foreach(var rule in rules)
                {
                    if(compositeReqExes.ContainsKey(rule.Key))
                        continue;

                    if(rule.Value.Contains("\""))
                    {
                        var firstIndex = rule.Value.IndexOf("\"");
                        var lastIndex = rule.Value.LastIndexOf("\"");

                        compositeReqExes.Add(rule.Key, String.Format(@"{0}",rule.Value.Substring(firstIndex+1, lastIndex-firstIndex-1)));
                        continue;
                    }

                    var nums = rule.Value.Split(' ');
                    if(!nums.Where(x => Regex.IsMatch(x, @"\d")).Select(x => Int32.Parse(x)).All(x => compositeReqExes.ContainsKey(x)))
                        continue;

                    if(nums.Contains("|"))
                    {
                        var reg = @"(";
                        foreach(var op in nums)
                        {
                            if(String.IsNullOrEmpty(op))
                                continue;

                            if(op == "|")
                                reg += @"|";
                            else
                                reg += String.Format(@"{0}", compositeReqExes[Int32.Parse(op)]);
                        }
                        reg += @")";

                        if(rule.Key != 0)
                            compositeReqExes.Add(rule.Key, reg);

                        if(rule.Key == 8 )
                            compositeReqExes.Add(1008, String.Format("({0}|({0})+)", compositeReqExes[42]));
                    }
                    else
                    {
                        var reg = @"";
                        foreach(var op in nums)
                        {
                            if(String.IsNullOrEmpty(op))
                                continue;

                            reg += String.Format(@"{0}", compositeReqExes[Int32.Parse(op)]);
                        }

                        if(rule.Key != 0)
                            compositeReqExes.Add(rule.Key, reg);

                        if(rule.Key == 8 )
                            compositeReqExes.Add(1008, String.Format("({0}|({0})+)", compositeReqExes[42]));                      
                    }
                }
            }
        }

        private void FindPasswordsThatMatch0()
        {
            int matching = 0;

            foreach(var password in passwords)
            {
                if(Regex.IsMatch(password,"^"+compositeReqExes[42]))
                {
                    var found = false;
                    var after8s = GetAllBackendAfter8(password);

                    foreach(var backend in after8s)
                    {
                        if(Remove11Regex(backend))
                        {
                            found = true;
                            break;
                        }
                    }


                    if(found)
                        matching++;
                }
            }

            Console.WriteLine("Part 2 : correct passwords "+ matching);

        }

        public List<string> GetAllBackendAfter8(string input)
        {
            if(!Regex.IsMatch(input,"^"+compositeReqExes[42]))
                return new List<string>();

            List<string> returns = new List<string>();
            MatchCollection matches = Regex.Matches(input, "^"+compositeReqExes[42]);
            foreach(Match match in matches)
            {
                returns.Add(input.Substring(match.Length));
                returns.AddRange(GetAllBackendAfter8(input.Substring(match.Length)));
            }

            return returns;

        }

        public bool Remove11Regex(string remaining)
        {
            //42 11 31
            var match = @"^" + compositeReqExes[42] + @"(\w*)" + compositeReqExes[31] + "$"; 

            if(!Regex.IsMatch(remaining, match))
                return false;

            MatchCollection frontMatch = Regex.Matches(remaining, "^"+compositeReqExes[42]);
            foreach(Match thisMatch in frontMatch)
            {
                var backend = remaining.Substring(thisMatch.Length);
                if(Regex.IsMatch(backend, "^"+compositeReqExes[31]+"$"))
                    return true;

                MatchCollection backMatch = Regex.Matches(backend,compositeReqExes[31]+"$");
                foreach(Match thisBackMatch in backMatch)
                {
                    if(thisBackMatch.Length == backend.Length)
                        return true;

                    var middle = backend.Substring(0, backend.Length-thisBackMatch.Length);
                    if(Remove11Regex(middle))
                        return true;
                }
            }


            return false;
        }

        public string GetRecursiveRegEx(int key, List<string> ops)
        {
            List<List<string>> parts = new List<List<string>>();

            List<string> part = new List<string>();
            foreach(var op in ops)
            {
                if(op =="|")
                {
                    parts.Add(part.ToList());
                    part.Clear();
                    continue;
                }

                part.Add(op);

                if(op == ops.Last())
                    parts.Add(part.ToList());
            }

            var baseReg = GetBaseRegEx(parts.Where(x => !x.Contains(key.ToString())).ToList());
            var others = parts.Where(x => x.Contains(key.ToString())).ToList();
            if(others.Count() == 0)
                return baseReg;

            var allReg = @"("+baseReg;
            foreach(var other in others)
            {
                allReg += "|";
                foreach(var prev in other)
                {
                    if(String.IsNullOrEmpty(prev))
                        continue;

                    if(prev == key.ToString())
                        allReg+= "("+baseReg+")+";
                    else
                        allReg+= compositeReqExes[Int32.Parse(prev)];
                }
            }
            allReg += ")";
            return allReg;
        }

        private string GetBaseRegEx(List<List<string>> higher)
        {
            var reg = @"(";

            foreach(var option in higher)
            {
                if(option != higher.First())
                    reg+=")|(";
                foreach(var prev in option)
                {
                    if(String.IsNullOrEmpty(prev))
                        continue;

                    reg += compositeReqExes[Int32.Parse(prev)];
                }
            }

            reg += ")";

            return reg;

        }
    }
}