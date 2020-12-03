using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    public class PasswordVerification
    {
        int min = 0;
        int max = 0;

        public PasswordVerification(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        public void PrintValidPasswordCount(int section)
        {
            var count = 0;
            for(int x = min; x<=max; x++)
            {
                if(section == 1 && VerifyPassword1(x))
                    count++;
                else if(section == 2 && VerifyPassword2(x))
                {
                    count++;
                }
            }
        
            Console.WriteLine(String.Format("Valid Password Count : {0}", count));
        }

        private bool VerifyPassword1(int pass)
        {
            string stringPass = pass.ToString();

            if(stringPass.Length != 6)
                return false;

            bool consecutiveDigit = false;

            for(int index =  1; index < stringPass.Length; index++)
            {
                if(!consecutiveDigit && stringPass[index] == stringPass[index - 1])
                {
                    consecutiveDigit = true;
                }
                if(Int16.Parse(stringPass.Substring(index,1)) < Int16.Parse(stringPass.Substring(index-1,1)))
                    return false;
                
            }

            return consecutiveDigit;
        }

        private bool VerifyPassword2(int pass)
        {
            string stringPass = pass.ToString();

            if(stringPass.Length != 6)
                return false;

            bool consecutiveDigit = false;
            string currentDigit = "";
            bool currentDigitValid = false;

            for(int index =  1; index < stringPass.Length; index++)
            {
                if(stringPass.Substring(index,1) != currentDigit)
                {
                    currentDigit = stringPass.Substring(index,1);
                    currentDigitValid = true;
                }

                if(!consecutiveDigit && currentDigitValid && stringPass[index] == stringPass[index - 1])
                {
                    if(index != stringPass.Length-1 && stringPass[index] == stringPass[index+1])
                    {
                        currentDigitValid = false;
                    }
                    else
                    {
                        consecutiveDigit = true;
                    }
                }
                
                if(Int16.Parse(stringPass.Substring(index,1)) < Int16.Parse(stringPass.Substring(index-1,1)))
                    return false;
                
            }

            return consecutiveDigit;
        }        

    }
}