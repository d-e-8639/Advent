using System;
using System.Collections.Generic;
using System.Text;

namespace Advent.lib
{
    public static class AdventMath
    {
        // % operator is the remainder, and doesn't calculate modulo correctly for negative numbers.
        public static int Modulo(int x, int N) {
            return (x % N + N) % N;
        }

    }
}
