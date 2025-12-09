using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Advent.lib
{
    public class HardCodedConverter
    {

        public static Func<TInput, TOutput> Factory<TInput, TOutput>(Func<TInput, TOutput> defaultValue, Dictionary<TInput, Func<TInput, TOutput>> conversions) {
            return delegate (TInput c)
            {
                Func<TInput, TOutput> getDefault = defaultValue;
                Dictionary<TInput, Func<TInput, TOutput>> mapping = conversions;

                Func<TInput, TOutput> val;
                if (mapping.TryGetValue(c, out val)) {
                    return val(c);
                }
                return getDefault(c);
            };

        }


        private static TOutput noDefaultThrowException<TInput, TOutput>(TInput i) {
            throw new KeyNotFoundException("No conversion defined for input " + i.ToString());
        }

        public static Func<TInput, TOutput> Factory<TInput, TOutput>(params (TInput, TOutput)[] map)
            where TOutput : struct
        {
            return Factory(
                noDefaultThrowException<TInput, TOutput>,
                map.ToDictionary<(TInput, TOutput), TInput, Func<TInput, TOutput>>(
                    item => item.Item1,
                    item => delegate (TInput c) { return (TOutput)item.Item2; }));
        }

        public static Func<TInput, TOutput> Factory<TInput, TOutput>(TOutput defaultValue, params (TInput, TOutput)[] map)
            where TOutput : struct
        {
            Func<TInput, TOutput> getDefault = delegate (TInput c) { return (TOutput)defaultValue; };
            Dictionary<TInput, Func<TInput, TOutput>> getMapping =
                map.ToDictionary<(TInput, TOutput), TInput, Func<TInput, TOutput>>(item => item.Item1, item => delegate (TInput c) { return (TOutput)item.Item2; });

            return Factory(getDefault, getMapping);
        }

        public static Func<TInput, TOutput> Factory<TInput, TOutput>(Func<TInput, TOutput> defaultConverter, params (TInput, Func<TInput, TOutput>)[] conversions) {
            return Factory(defaultConverter, conversions.ToDictionary(item => item.Item1, item => item.Item2));
        }

        public static Func<TInput, TOutput> Factory<TInput, TOutput>(TOutput defaultValue, params (TInput, Func<TInput, TOutput>)[] conversions) {
            return Factory((c) => defaultValue, conversions);
        }

        public static Func<TInput, TOutput> Factory<TInput, TOutput>(params (TInput, Func<TInput, TOutput>)[] conversions) {
            return Factory(noDefaultThrowException<TInput, TOutput>, conversions);
        }

        public static Func<TInput, TOutput> Factory<TInput, TOutput>(params (TInput, Func<TOutput>)[] conversions) {
            return Factory(conversions.Select(converter => (converter.Item1, (Func<TInput, TOutput>)((c) => converter.Item2()))).ToArray());
        }

        public static Func<TInput, TOutput> Factory<TInput, TOutput>(params (TInput, Type)[] map) {
            return Factory(
                noDefaultThrowException<TInput, TOutput>,
                map.ToDictionary<(TInput, Type), TInput, Func<TInput, TOutput>>(
                    item => item.Item1,
                    item => delegate (TInput c) {
                        // Create instance of class using it's constructor that has no parameters
                        ConstructorInfo cInfo = item.Item2.GetConstructor(Type.EmptyTypes);
                        if (cInfo == null) {
                            throw new Exception($"Invalid type given: {item.Item2}. Types given for HardCodedConverter must be a reference type and must have a constructor with no parameters.");
                        }
                        object instance = cInfo.Invoke(new object[] { });
                        return (TOutput) instance;
                    }));
        }


    }
}