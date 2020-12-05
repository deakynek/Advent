using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    public static class MoreMath
    {
        public static int getGCD(int a, int b)
        {
            if (a == 0)
                return b;
            return getGCD(b % a, a);
        }

        public static int getLCM(int a, int b)
        {
            return (a / getGCD(a, b)) * b;
        }
        public static long getGCD(long a, long b)
        {
            if (a == 0)
                return b;
            return getGCD(b % a, a);
        }

        public static long getLCM(long a, long b)
        {
            return (a / getGCD(a, b)) * b;
        }
    }

}