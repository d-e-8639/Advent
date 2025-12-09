using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Advent.lib;
using System.Runtime.CompilerServices;

namespace Advent.Y2025
{
    public class Advent7
    {
        public static void Do(string wd){
            string file;
            using (StreamReader sr = new StreamReader(wd + "Advent7.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);



            Grid<ManifoldEntry> g = new Grid<ManifoldEntry>(lines.Select(l => l.Select(HardCodedConverter.Factory<char, ManifoldEntry>(
                ('^', () => new Splitter()),
                ('S', () => new Start()),
                ('.', () => new Empty())
            ))));

            Stopwatch st1 = new Stopwatch();
            st1.Start();
            task1(g);
            st1.Stop();

            g = new Grid<ManifoldEntry>(lines.Select(l => l.Select(HardCodedConverter.Factory<char, ManifoldEntry>(
                ('^', (c) => new Splitter()),
                ('S', (c) => new Start()),
                ('.', (c) => new Empty())
            ))));

            Stopwatch st2 = new Stopwatch();
            st2.Start();
            task2(g);
            st2.Stop();

            Console.WriteLine($"Time 1: {st1.ElapsedMilliseconds}, Time 2: {st2.ElapsedMilliseconds}" );

        }

        public enum ManifoldType
        {
            Start,
            Splitter,
            Empty
        }

        public abstract class ManifoldEntry : IGridRegisterable<ManifoldEntry> {
            public long lit = 0;

            public abstract void light();

            protected GridItem<ManifoldEntry> gi;
            public void Register(GridItem<ManifoldEntry> gridItem) {
                this.gi = gridItem;
            }
        }

        public class Start : ManifoldEntry
        {
            public override void light() {
                this.lit++;
            }

            public override string ToString() {
                return "S";
            }
        }

        public class Splitter : ManifoldEntry
        {
            public bool isSplitting = false;

            public override void light() {
                if (gi.HasUp && (gi.Up.Item.lit > 0)) {
                    isSplitting = true;
                    if (gi.HasLeft) {
                        gi.Left.Item.lit += gi.Up.Item.lit;
                    }
                    if (gi.HasRight) {
                        gi.Right.Item.lit += gi.Up.Item.lit;
                    }
                }
            }

            public override string ToString() {
                return "^";
            }
        }

        public class Empty : ManifoldEntry
        {
            public override void light() {
                if ((gi.HasUp)&&(gi.Up.Item.lit > 0)) {
                    this.lit += gi.Up.Item.lit;
                }
            }

            public override string ToString() {
                if (lit > 0) {
                    return "|";
                }
                return ".";
            }
        }

        private static void task1(Grid<ManifoldEntry> g) {
            foreach (ManifoldEntry me in g.GetItems()) {
                me.light();
            }

            Console.WriteLine(g.ToString());

            Console.WriteLine("Total splits: " + g.GetItems().Count(me => me is Splitter && ((Splitter)me).isSplitting));
        }

        private static void task2(Grid<ManifoldEntry> g) {
            foreach (ManifoldEntry me in g.GetItems()) {
                me.light();
            }

            Console.WriteLine(g.ToString());

            Console.WriteLine("Total splits: " + g.Items.Last().Sum(i => i.Item.lit));
        }



    }
}
