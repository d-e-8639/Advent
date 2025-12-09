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
            using (StreamReader sr = new StreamReader(wd + "Advent7sample.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);


            Grid<ManifoldEntry> g = new Grid<ManifoldEntry>(lines.Select(l => l.Select(HardCodedConverter.Factory(
                ('^', ManifoldEntry.Splitter),
                ('S', ManifoldEntry.Start),
                ('.', ManifoldEntry.Empty)
            ))));


            Stopwatch st1 = new Stopwatch();
            st1.Start();
            task1();
            st1.Stop();


            Stopwatch st2 = new Stopwatch();
            st2.Start();
            task2();
            st2.Stop();

            Console.WriteLine($"Time 1: {st1.ElapsedMilliseconds}, Time 2: {st2.ElapsedMilliseconds}" );

        }

        public enum ManifoldEntry
        {
            Start,
            Splitter,
            Empty
        }

        private static void task1() {
        }

        private static void task2() {
        }

        // public class CharConverter<T>
        // {
        //     Func<char, T> getDefault;
        //     Dictionary<char, Func<char, T>> mapping;

        //     private CharConverter(Func<char, T> getDefault, Dictionary<char, Func<char, T>> mapping)
        //     {
        //         this.getDefault = getDefault;
        //         this.mapping = mapping;
        //     }

        //     public static CharConverter<T> Factory(params (char, T)[] map)
        //     {
        //         return Factory(default(T), map);
        //     }

        //     public static CharConverter<T> Factory(T defaultValue, params (char, T)[] conversions)
        //     {
        //         // if (defaultValue.GetType() != typeof(T))
        //         // {
        //         //     throw new Exception("Default value type must match declared return type " + typeof(T).ToString());
        //         // }

        //         // if (!conversions.All(c => c.Item2.GetType() == typeof(T)))
        //         // {
        //         //     throw new Exception("Type of all values in map must match declared return type " + typeof(T).ToString());
        //         // }

        //         Func<char, T> getDefault = delegate (char c){return (T)defaultValue;};
        //         Dictionary<char, Func<char, T>> getMapping = 
        //             conversions.ToDictionary<(char, T), char, Func<char, T>>(item => item.Item1, item => delegate(char c) { return (T)item.Item2; });
        //             //conversions.ToDictionary<(char, T), char, Func<char, T>>(item => item.Item1, item => c => item.Item2 );

        //         return new CharConverter<T>(getDefault, getMapping);
        //     }

        //     public static CharConverter<T> Factory(Func<char, T> defaultValue, params (char, Func<char, T>)[] conversions)
        //     {
        //         return new CharConverter<T>(defaultValue, conversions.ToDictionary(item => item.Item1, item => item.Item2));
        //     }

        //     public static CharConverter<T> Factory(T defaultValue, params (char, Func<char, T>)[] conversions)
        //     {
        //         return Factory((c) => defaultValue, conversions);
        //     }

        //     // public CharConverter(T defaultValue, params (char, object)[] conversions)
        //     // {
        //     //     this.defaultValue = defaultValue;
        //     //     this.conversions = conversions.ToDictionary(t => t.Item1, t => t.Item2);
        //     // }

        //     public T Convert(char c)
        //     {
        //         Func<char, T> val;
        //         if (mapping.TryGetValue(c, out val))
        //         {
        //             return val(c);
        //         }
        //         return this.getDefault(c);
        //     }
        // }

        public class HardCodedConverter
        {

            public static Func<TItem, TConverter> Factory<TItem, TConverter>(Func<TItem, TConverter> defaultValue, Dictionary<TItem, Func<TItem, TConverter>> conversions)
            {
                return delegate (TItem c)
                {
                    Func<TItem, TConverter> getDefault = defaultValue;
                    Dictionary<TItem, Func<TItem, TConverter>> mapping = conversions;

                    Func<TItem, TConverter> val;
                    if (mapping.TryGetValue(c, out val))
                    {
                        return val(c);
                    }
                    return getDefault(c);
                };

            }

            public static Func<TItem, TConverter> Factory<TItem, TConverter>(params (TItem, TConverter)[] map)
            {
                return Factory(default, map);
            }

            public static Func<TItem, TConverter> Factory<TItem, TConverter>(TConverter defaultValue, params (TItem, TConverter)[] conversions)
            {
                Func<TItem, TConverter> getDefault = delegate (TItem c){return (TConverter)defaultValue;};
                Dictionary<TItem, Func<TItem, TConverter>> getMapping = 
                    conversions.ToDictionary<(TItem, TConverter), TItem, Func<TItem, TConverter>>(item => item.Item1, item => delegate(TItem c) { return (TConverter)item.Item2; });

                return Factory(getDefault, getMapping);
            }

            public static Func<TItem, TConverter> Factory<TItem, TConverter>(Func<TItem, TConverter> defaultValue, params (TItem, Func<TItem, TConverter>)[] conversions)
            {
                return Factory(defaultValue, conversions.ToDictionary(item => item.Item1, item => item.Item2));
            }

            public static Func<TItem, TConverter> Factory<TItem, TConverter>(TConverter defaultValue, params (TItem, Func<TItem, TConverter>)[] conversions)
            {
                return Factory((c) => defaultValue, conversions);
            }

            public static Func<TItem, TConverter> Factory<TItem, TConverter>(params (TItem, Func<TItem, TConverter>)[] conversions)
            {
                return Factory((c) => default, conversions);
            }
        }

    }
}
