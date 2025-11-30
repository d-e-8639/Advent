using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Advent {
    public class CircularEnumerator<T> : IEnumerator<T>{

        private T[] data;
        private int index=0;
        public long Loops { get; private set; } = -1;

        public bool AtOrigin {
            get {return index == 0;}
        }

        public T CurrentOffset(int i) {
            // We must use a modulo function here that will handle negative numbers
            return data[modulo((index + i), data.Length)];
        }

        // % operator is the remainder, and doesn't calculate modulo correctly for negative numbers.
        private static int modulo(int x, int N) {
            return (x % N + N) % N;
        }

        public T Next {
            get {
                return data[(index + 1) % data.Length];
            }
        }

        public T Prev {
            get {
                return index == 0 ? data.Last() : data[index - 1];
            }
        }


        public int Index {get { return index; } }

        public T Current
        {
            get
            {
                if (index < 0) {
                    throw new Exception("Enumerator not initialized. Call MoveNext()");
                }
                return data[index];
            }
        }

        object IEnumerator.Current => this.Current;

        public void Dispose()
        {
            // nothing to dispose;
        }

        public bool MoveNext()
        {
            index = (index + 1) % data.Length;
            if (AtOrigin) {
                Loops ++;
            }
            return true;
        }

        public void Reset()
        {
            index=-1;
            Loops=-1;
        }

        public CircularEnumerator(IEnumerable<T> data) {
            this.data = data.ToArray();
            Reset();
        }
    }
}