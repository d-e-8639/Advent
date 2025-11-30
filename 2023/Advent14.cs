using System;
using System.IO;
using System.IO.Hashing;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Collections;
using System.Security;
using System.Runtime.Serialization;
using System.Runtime.Intrinsics.Arm;

namespace Advent
{
    public class Advent14
    {
        public static void Do(){
            string file;
            using (StreamReader sr = new StreamReader("C:/src/HelloWorld/Advent14.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.None);

            Platform p = new Platform(lines);

            //task1(p);

            task2(p);
        }

        private static void task1(Platform p) {
            Console.WriteLine(p.ToString());
            p.TiltNorth();
            Console.WriteLine();
            Console.WriteLine(p);
            List<Rock> rs = p.Rocks.SelectMany(rl => rl).ToList();
            Console.WriteLine("Weight north: " + p.Rocks.SelectMany(rl => rl).Where(r => r != null).Select(r => r.WeightNorth()).Sum());
        }

        private static void task2(Platform p) {

            // Console.WriteLine(p.ToString());
            // Console.WriteLine();
            // byte[] b = p.SerializeBinary().ToArray();

            // Platform deser = Platform.DeserializeBinary(b);

            // Console.WriteLine(deser.ToString());
            // Console.WriteLine();

            Dictionary<ulong, long> index = new Dictionary<ulong, long>();
            Dictionary<long, byte[]> platforms = new Dictionary<long, byte[]>();

            List<byte[]> platSerial = new List<byte[]>();
            long start =0, totalRotateCount =0;
            for (long i=0; i < 1000000000; i++) {
                //byte[] indexHash = System.Security.Cryptography.MD5.HashData(p.SerializeBinary().ToArray());
                byte[] serialPlatform = p.SerializeBinary().ToArray();

                for (int x=0; x < platSerial.Count; x++) {
                    if (platSerial[x].SequenceEqual(serialPlatform)) {
                        start = x;
                        totalRotateCount = i - start;
                        break;
                    }
                }
                if (totalRotateCount != 0) {
                    break;
                }

                platSerial.Add(serialPlatform);


                //byte[] indexHash = Crc64.Hash(serialPlatform);
                // ulong key = makeKey(indexHash);

                // if (index.ContainsKey(key)) {
                //     if (serialPlatform.SequenceEqual(platforms[4])) {
                //         // Console.WriteLine(p.ToString());
                //         // Console.WriteLine("----------------");
                //         // Console.WriteLine(Platform.DeserializeBinary(platforms[index[key]]).ToString());
                //         start = index[key];
                //         totalRotateCount = i - start;
                //         break;
                //     }
                // }
                // index[key] = i;
                // platforms[i] = serialPlatform;

                p.TiltNorth();
                p.TiltWest();
                p.TiltSouth();
                p.TiltEast();
            }

            //long ticks = ((20 - start) % totalRotateCount);
            long ticks = ((1000000000 - start) % totalRotateCount);

            for (int i=0; i < ticks; i++) {
                p.TiltNorth();
                p.TiltWest();
                p.TiltSouth();
                p.TiltEast();
                Console.WriteLine("temp Weight north: " + p.Rocks.SelectMany(rl => rl).Where(r => r != null).Select(r => r.WeightNorth()).Sum());
            }

            Console.WriteLine("Weight north: " + p.Rocks.SelectMany(rl => rl).Where(r => r != null).Select(r => r.WeightNorth()).Sum());

        }

        private static ulong makeKey(byte[] hash) {
            return
                ((ulong)hash[0]) << 56 |
                ((ulong)hash[0]) << 48 |
                ((ulong)hash[0]) << 40 |
                ((ulong)hash[0]) << 32 |
                ((ulong)hash[0]) << 24 |
                ((ulong)hash[0]) << 16 |
                ((ulong)hash[0]) << 8 |
                ((ulong)hash[0]);
        }

        public class Platform {
            public List<RockList> Rocks = new List<RockList>();

            public Platform(IEnumerable<string> lines) {
                int rowNum=0;
                foreach (string line in lines) {
                    RockList r = new RockList(this, rowNum++, line.Length);
                    int colNum=0;
                    foreach (char c in line) {
                        if (c == 'O') {
                            r[colNum] = new RoundRock(this);
                        }
                        else if (c == '#') {
                            r[colNum] = new CubeRock(this);
                        }
                        colNum++;
                    }
                    Rocks.Add(r);
                }
            }

            public Platform(int height, int width)
            {
                for (int y=0; y < height; y++) {
                    Rocks.Add(new RockList(this, y, width));
                }
            }

            public void TiltNorth() {
                for (int y = 0; y < Rocks.Count; y++) {
                    for (int x=0; x < Rocks[y].Length; x++) {
                        if (Rocks[y][x] is RoundRock daRock) {
                            daRock.RollNorth();
                        }
                    }
                }
            }

            public void TiltEast() {
                for (int y = Rocks.Count - 1; y >= 0; y--) {
                    for (int x=Rocks[y].Length - 1; x >= 0; x--) {
                        if (Rocks[y][x] is RoundRock daRock) {
                            daRock.RollEast();
                        }
                    }
                }
            }

            public void TiltSouth() {
                for (int y = Rocks.Count - 1; y >= 0; y--) {
                    for (int x=0; x < Rocks[y].Length; x++) {
                        if (Rocks[y][x] is RoundRock daRock) {
                            daRock.RollSouth();
                        }
                    }
                }
            }

            public void TiltWest() {
                for (int y = 0; y < Rocks.Count; y++) {
                    for (int x=0; x < Rocks[y].Length; x++) {
                        if (Rocks[y][x] is RoundRock daRock) {
                            daRock.RollWest();
                        }
                    }
                }
            }


            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                foreach (RockList rl in Rocks) {
                    sb.AppendLine(rl.ToString());
                }
                return sb.ToString();
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }


            public List<byte> SerializeBinary() {
                BitList data = new BitList();
                data.Add((uint) ((ushort) this.Rocks.Count), 16);
                data.Add((uint) ((ushort) this.Rocks[0].Length), 16);
                foreach (RockList row in this.Rocks) {
                    foreach (Rock r in row) {
                        if (r == null) {
                            data.Add(false);
                        }
                        else if (r is RoundRock) {
                            data.Add(0x1, 2);
                        }
                        else if (r is CubeRock) {
                            data.Add(0x3, 2);
                        }
                    }
                }
                // Console.WriteLine(data.ToString());
                return data.ToBytes();
            }

            public static Platform DeserializeBinary(byte[] data) {
                int height = BitConverter.ToInt16(new ReadOnlySpan<byte>(data, 0, 2));
                int width = BitConverter.ToInt16(new ReadOnlySpan<byte>(data, 2, 4));

                BitArray d = new BitArray(data);
                int i=32;
                Platform p = new Platform(height, width);

                for (int y=0; y < height; y++) {
                    List<Rock> rl = new List<Rock>();
                    for (int x=0; x < width; x++) {
                        if (d[i]) {
                            i++;
                            if (d[i]) {
                                p.Rocks[y][x] = new CubeRock(p);
                            }
                            else {
                                p.Rocks[y][x] = new RoundRock(p);
                            }
                        }
                        else {
                            // Do nothing, no rock to add
                        }
                        i++;
                    }
                }
                return p;
            }

        }

        public class RockList : IEnumerable<Rock>, IEnumerator<Rock> {
            private Platform platform;
            private readonly int RowIndex;
            private Rock[] rocks;

            public int Length {
                get {
                    return rocks.Length;
                }
            }

            public RockList(Platform p, int rowIndex, int size) {
                platform = p;
                RowIndex = rowIndex;
                rocks = new Rock[size];
                for (int i=0; i < size; i++) {
                    rocks[i] = null;
                }
            }

            public Rock this[int index] {
                get { return rocks[index]; }
                set {
                    // Remove rock from previous location
                    if (value.Y != null && value.X != null) {
                        platform.Rocks[value.Y.Value].rocks[value.X.Value] = null;
                    }
                    // Set new location
                    rocks[index] = value;
                    value.X = index;
                    value.Y = RowIndex;
                }
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                foreach (Rock r in rocks) {
                    sb.Append(r != null ? r.Character : '.');
                }
                return sb.ToString();
            }


            private int? currentIndex;
            public Rock Current
            {
                get
                {
                    return rocks[currentIndex.Value];
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public IEnumerator<Rock> GetEnumerator()
            {
                this.Reset();
                return this;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public bool MoveNext()
            {
                if (currentIndex == null) {
                    currentIndex = 0;
                }
                else {
                    currentIndex++;
                }
                return currentIndex < rocks.Length;
            }

            public void Reset()
            {
                currentIndex=null;
            }

            public void Dispose()
            {
            }


        }

        public abstract class Rock {
            protected Platform platform;
            public int? Y = null;
            public int? X = null;

            public Rock(Platform p) {
                platform = p;
            }

            public void Move(int y, int x) {
                platform.Rocks[y][x] = this;
            }

            public abstract char Character { get; }

            public abstract int WeightNorth ();
        }

        public class RoundRock : Rock
        {
            public RoundRock(Platform p) : base(p)
            {
            }

            public override char Character => 'O';

            public override int WeightNorth()
            {
                return platform.Rocks.Count - this.Y.Value;
            }

            public void RollNorth (){
                int rollDist=0;
                for (; Y.Value - rollDist > 0; rollDist++) {
                    if (platform.Rocks[Y.Value - rollDist - 1][X.Value] != null) {
                        break;
                    }
                }
                if (rollDist != 0) {
                    Move(Y.Value - rollDist, X.Value);
                }
            }

            public void RollEast() {
                int rollDist=0;
                for (; X.Value + rollDist < platform.Rocks[Y.Value].Length - 1; rollDist++) {
                    if (platform.Rocks[Y.Value][X.Value + rollDist + 1] != null) {
                        break;
                    }
                }
                if (rollDist != 0) {
                    Move(Y.Value, X.Value + rollDist);
                }
            }

            public void RollSouth() {
                int rollDist=0;
                for (; Y.Value + rollDist < platform.Rocks.Count - 1; rollDist++) {
                    if (platform.Rocks[Y.Value + rollDist + 1][X.Value] != null) {
                        break;
                    }
                }
                if (rollDist != 0) {
                    Move(Y.Value + rollDist, X.Value);
                }
            }

            public void RollWest() {
                int rollDist=0;
                for (; X.Value - rollDist > 0 ; rollDist++) {
                    if (platform.Rocks[Y.Value][X.Value - rollDist - 1] != null) {
                        break;
                    }
                }
                if (rollDist != 0) {
                    Move(Y.Value, X.Value - rollDist);
                }
            }


        }

        public class CubeRock : Rock
        {
            public CubeRock(Platform p) : base(p)
            {
            }

            public override char Character => '#';

            public override int WeightNorth()
            {
                return 0;
            }
        }
    }
}
