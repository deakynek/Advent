using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace Advent
{
    class PassPortVerification
    {
        List<Dictionary<string,string>> passports = new List<Dictionary<string, string>>();
        List<string> RequiredFeilds = new List<string>(){"byr","iyr","eyr","hgt","hcl","ecl","pid"};

        public PassPortVerification(List<string> lines)
        {
            Dictionary<string,string> pass= new Dictionary<string, string>();

            foreach(var line in lines)
            {
                if(line == "")
                {
                    passports.Add(pass);
                    pass = new Dictionary<string, string>();
                    continue;
                }

                foreach(var entry in line.Split(' '))
                {
                    var keyValPair = entry.Split(':');
                    pass.Add(keyValPair[0], keyValPair[1]);
                }

                if(line == lines.Last())
                {
                    passports.Add(pass);
                }
            }
        }

        public void ValidatePassPortsPrintInvalid()
        {
            var validCountPart1 = 0;
            var validCountPart2 = 0;

            foreach(var pass in passports)
            {
                if(ValidatePassPort(pass, false))
                    validCountPart1++;

                if(ValidatePassPort(pass, true))
                    validCountPart2++;
            }

            Console.WriteLine("Part 1: Valid Passport Count - " + validCountPart1.ToString());
            Console.WriteLine("Part 2: Valid Passport Count - " + validCountPart2.ToString());
        }

        public bool ValidatePassPort(Dictionary<string,string> pass, bool checkValues)
        {
            var missingVals = RequiredFeilds.Where(x => !pass.Keys.Contains(x)).ToList<string>();

            if(missingVals.Any())
                return false;

            if(!checkValues)
                return true;

            foreach(var key in pass.Keys)
            {
                var value = pass[key];
                switch(key)
                {
                    case "byr":
                        if(!Regex.Match(value, @"^\d{4}$").Success)
                            return false;
                        int year = Int32.Parse(value);
                        if(year<1920 || year>2002 )
                            return false;
                        break;

                    case "iyr":
                        if(!Regex.Match(value, @"^\d{4}$").Success)
                            return false;
                        year = Int32.Parse(value);
                        if(year<2010 || year>2020 )
                            return false;
                        break;

                    case "eyr":
                        if(!Regex.Match(value, @"^\d{4}$").Success)
                            return false;
                        year = Int32.Parse(value);
                        if(year<2020 || year>2030 )
                            return false;
                        break;

                    case "hgt":
                        if(!Regex.Match(value, @"^\d+(cm|in)$").Success)
                            return false;
                        
                        if(value.Substring(value.Length-2,2) == "cm")
                        {
                            var cm = Int32.Parse(value.Substring(0,value.Length-2));
                            if(cm < 150 || cm > 193)
                                return false;
                        }
                        if(value.Substring(value.Length-2,2) == "in")
                        {
                            var inches = Int32.Parse(value.Substring(0,value.Length-2));
                            if(inches < 59 || inches > 76)
                                return false;
                        }
                        break;
                    
                    case "hcl":
                        if(!Regex.Match(value, @"^#([0-9]|[a-f]){6}$").Success)
                            return false;
                        break;
                    
                    case "ecl":
                        if(!Regex.Match(value, @"^(amb|blu|brn|gry|grn|hzl|oth)$").Success)
                            return false;
                        break;
                    
                    case "pid":
                        if(!Regex.Match(value, @"^\d{9}$").Success)
                            return false;
                        break;
                    default:
                        break;
                }
            }

            return true;
        }


    }    
}