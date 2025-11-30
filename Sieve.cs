using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace HelloWorld
{
    public static class Sieve
    {
        public static List<int> Basic(int limit){

            BitArray b = new BitArray(limit, true);
            for (int i=2,j; (j=i*i) < limit; i++) {
                if (b[i]) {
                    for (; j < limit; j+=i) {
                        b[j] = false;
                    }
                }
            }

            List<int> result = new List<int>();
            for (int i=2; i < limit; i++) {
                if (b[i]){
                    result.Add(i);
                }
            }
            return result;
        }

        
        public static List<int> SkipEven(int limit){

            BitArray b = new BitArray(limit / 2, true);
            for (int i=3,j; (j=i*i) < limit; i+=2) {
                if (b[i >> 1]) {
                    for (; j < limit; j+=i) {
                        if ((j & 0x01) == 1)
                            b[j >> 1] = false;
                    }
                }
            }

            List<int> result = new List<int>();
            result.Add(2);
            for (int i=3; i < limit; i++) {
                if (((i & 0x01) != 0) && b[i >> 1]){
                    result.Add(i);
                }
            }
            return result;
        }

        public static void test1(){
            Console.WriteLine("Basic:    " + string.Join(",", Basic(100)));
            Console.WriteLine("SkipEven: " + string.Join(",", SkipEven(100)));
        }

        public static void test2(){
            for (int i=10; i < 10000; i++){
                bool m = Basic(i).SequenceEqual(SkipEven(i));
                Console.WriteLine($"Gen limit {i}, match: {m}");
                if (m == false) {
                    throw new Exception("Match failed!");
                }
            }
        }

        public static void test3(){
            int limit = 1000000;

            Stopwatch st = new Stopwatch();
            st.Start();
            List<int> l1 = Basic(limit);
            st.Stop();
            Console.WriteLine($"Basic {limit}: {st.ElapsedMilliseconds}ms");

            st.Reset();
            st.Start();
            List<int> l2 = Basic(limit);
            st.Stop();
            Console.WriteLine($"SkipEven {limit}: {st.ElapsedMilliseconds}ms");

        }

    }
}
