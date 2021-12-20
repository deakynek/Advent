using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    class BinaryOperators
    {
        long totOps = 0;

        public BinaryOperators(List<string> lines)
        {
            var operators = "";
            foreach(var num in lines[0])
            {
                var thisBinary =  Convert.ToString(Convert.ToInt64(num.ToString(), 16), 2);
                thisBinary = new string('0', 4 - thisBinary.Length) + thisBinary;
                operators+= thisBinary;
            }

            var startingIndex = 0;
            var tot = 0;

            long result = 0;
            ParsePacket(operators, 0, out result);
            Console.WriteLine(totOps);
            Console.WriteLine(result);
        }

        private int ParsePacket(string operators, int startingIndex, out long result)
        {
            
            var originalIndex = startingIndex;
            var packetVer = Convert.ToInt32(operators.Substring(startingIndex,3),2);
            startingIndex+=3;
            var packetId= Convert.ToInt64(operators.Substring(startingIndex,3),2);
            startingIndex+=3;


            totOps+=packetVer;

            if(packetId == 4)
            {
                var totalNumber = "";
                while(operators[startingIndex] !='0')
                {
                    totalNumber+=operators.Substring(startingIndex+1, 4);
                    startingIndex += 5;
                }
                totalNumber += operators.Substring(startingIndex+1, 4);
                var decNumber = Convert.ToInt64(totalNumber, 2);
                startingIndex+=5;

                result = decNumber;
            }
            else
            {

                var lenType = int.Parse(operators.Substring(startingIndex,1));
                startingIndex+=1;
                
                List<long> lowerResults = new List<long>();
                var length = 0;
                if(lenType == 0)
                {
                    length = Convert.ToInt32(operators.Substring(startingIndex,15), 2);
                    startingIndex+=15;

                    var totBitsUsed = 0;

                    while(totBitsUsed < length)
                    {
                        long thisResult =0;
                        totBitsUsed += ParsePacket(operators.Substring(startingIndex+totBitsUsed, length-totBitsUsed),0, out thisResult);
                        lowerResults.Add(thisResult);
                    }

                    startingIndex+=length;
                }
                else
                {
                    length = Convert.ToInt32(operators.Substring(startingIndex,11), 2);
                    startingIndex+=11;

                    while(length>0)
                    {
                        long thisResult =0;
                        startingIndex += ParsePacket(operators, startingIndex, out thisResult);
                        lowerResults.Add(thisResult);

                        length -=1;
                    }

                }

                switch(packetId)
                {
                    case 0:
                        result = lowerResults.Sum();
                        break;
                    case 1:
                        result = 1;
                        foreach(var r in lowerResults)
                        {
                            result *=r;
                        }
                        break;
                    case 2:
                        result = lowerResults.Min();
                        break;
                    case 3:
                        result = lowerResults.Max();
                        break;
                    case 5:
                        if(lowerResults[0]>lowerResults[1])
                            result = 1;
                        else
                            result = 0;
                        break;
                    case 6:                        
                        if(lowerResults[0]<lowerResults[1])
                            result = 1;
                        else
                            result = 0;
                        break;
                    case 7:                        
                        if(lowerResults[0]==lowerResults[1])
                            result = 1;
                        else
                            result = 0;
                        break;

                    default:
                        result = 0;
                        break;
                }
            }

            var packetLength = startingIndex-originalIndex;
            return packetLength;

            
        
        }
    }    
}