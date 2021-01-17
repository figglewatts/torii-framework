using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Torii.Util
{
    /// <summary>
    /// Utility functions for generating random numbers.
    /// TODO: Refactor RandUtil to not be static, as will help for determinism of random numbers?
    /// </summary>
    public static class RandUtil
    {
        private static Random _rand;

        private static readonly Color[] _randomColors = new[]
        {
            Color.white,
            Color.red,
            Color.yellow,
            Color.blue,
            Color.cyan,
            Color.green,
            Color.magenta
        };

        static RandUtil() { RefreshSeed(); }

        /// <summary>
        /// Refresh the current random generator seed.
        /// </summary>
        public static void RefreshSeed()
        {
            var seed = (int)DateTime.Now.Ticks & 0x0000FFFF;
            _rand = new Random(seed);
        }

        /// <summary>
        /// Set the random generator to a given seed. Used for determinism.
        /// </summary>
        /// <param name="seed">The seed to set it to.</param>
        public static void SetSeed(int seed) { _rand = new Random(seed); }

        /// <summary>
        /// Get a non-negative random integer.
        /// </summary>
        /// <returns>A non-negative random integer.</returns>
        public static int Int() { return _rand.Next(); }

        /// <summary>
        /// Get a random non-negative integer with exclusive maximum value.
        /// </summary>
        /// <param name="max">The exlusive max value.</param>
        /// <returns>A random non-negative integer.</returns>
        public static int Int(int max) { return _rand.Next(max); }

        /// <summary>
        /// Get a random non-negative integer between a min and max value.
        /// </summary>
        /// <param name="min">The inclusive min bound.</param>
        /// <param name="max">The exlusive max bound.</param>
        /// <returns>The random integer.</returns>
        public static int Int(int min, int max) { return _rand.Next(min, max); }

        /// <summary>
        /// Get a random float x, where 0.0 &lt;= x &lt; 1.0
        /// </summary>
        /// <returns>The random float.</returns>
        public static float Float() { return (float)_rand.NextDouble(); }

        /// <summary>
        /// Get a random float between given bounds.
        /// </summary>
        /// <param name="min">Inclusive min bound.</param>
        /// <param name="max">Exclusive max bound.</param>
        /// <returns>The random float.</returns>
        public static float Float(float min, float max) { return (min + Float() * (max - min)); }

        /// <summary>
        /// Get a random float with a max possible value (exclusive).
        /// </summary>
        /// <param name="max">The exclusive max bound.</param>
        /// <returns>The random float.</returns>
        public static float Float(float max) { return Float(0, max); }

        /// <summary>
        /// Generate a random color from a set of 7 colors.
        ///
        /// White, red, yellow, blue, cyan, green, or magenta.
        ///
        /// If you want to generate a fully random color, use FullyRandColor(). 
        /// </summary>
        /// <returns>A random color.</returns>
        public static Color RandColor() { return _randomColors[Int(_randomColors.Length)]; }

        /// <summary>
        /// Generate a random color from a set of 7 colours.
        ///
        /// White, red, yellow, blue, cyan, green, or magenta.
        ///
        /// If you want to generate a fully random colour, use FullyRandColour(). 
        /// </summary>
        /// <returns>A random colour.</returns>
        public static Color RandColour() { return RandColor(); }

        /// <summary>
        /// Generate a fully random color. This could produce lots of browns or baby-sick type colors, so if
        /// you're looking for an easily recognisable color then use RandColor() instead.
        /// </summary>
        /// <param name="transparency">Should transparency be randomised too?</param>
        /// <returns>A fully random color.</returns>
        public static Color FullyRandColor(bool transparency = false)
        {
            return new Color(Float(0, 1), Float(0, 1), Float(0, 1), transparency ? Float(0, 1) : 1);
        }

        /// <summary>
        /// Generate a fully random colour. This could produce lots of browns or baby-sick type colours, so if
        /// you're looking for an easily recognisable colour then use RandColour() instead.
        /// </summary>
        /// <param name="transparency">Should transparency be randomised too?</param>
        /// <returns>A fully random colour.</returns>
        public static Color FullyRandColour(bool transparency = false) { return FullyRandColor(transparency); }

        /// <summary>
        /// Choose a random element from an array.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <typeparam name="T">The type of the object contained within the array.</typeparam>
        /// <returns>The random choice.</returns>
        public static T RandomArrayElement<T>(T[] array) { return array[Int(array.Length)]; }

        /// <summary>
        /// Choose a random element from a list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <typeparam name="T">The type of the objects contained within the list.</typeparam>
        /// <returns>The random choice.</returns>
        public static T RandomListElement<T>(List<T> list) { return list[Int(list.Count)]; }

        /// <summary>
        /// Choose a random element from an IEnumerable.
        /// </summary>
        /// <param name="list">The enumerable.</param>
        /// <typeparam name="T">The type of the elements in the enumerable.</typeparam>
        /// <returns>The random element.</returns>
        public static T RandomListElement<T>(IEnumerable<T> list)
        {
            IEnumerable<T> enumerable = list.ToList();
            return enumerable.ToList()[Int(enumerable.Count())];
        }

        /// <summary>
        /// Choose a random enum value.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <returns>The random enum value.</returns>
        public static T RandomEnum<T>() where T : Enum
        {
            var v = Enum.GetValues(typeof(T));
            return (T)v.GetValue(_rand.Next(v.Length));
        }

        /// <summary>
        /// Select a random array element from values given in varargs.
        /// </summary>
        /// <param name="list">The varargs params.</param>
        /// <typeparam name="T">The type of the params.</typeparam>
        /// <returns>The randomly chosen element.</returns>
        public static T From<T>(params T[] list) { return RandomArrayElement(list); }

        /// <summary>
        /// If you want something to happen one in every X times, this will calculate whether it happened.
        /// </summary>
        /// <param name="chance">The chance of it happening.</param>
        /// <returns>True if it happened, false otherwise.</returns>
        public static bool OneIn(float chance) { return Float(chance) < 1f / chance; }

        /// <summary>
        /// Shuffle a list with Fisher-Yates.
        /// </summary>
        /// <param name="list">The list to shuffle.</param>
        /// <typeparam name="T">The type of the list.</typeparam>
        /// <returns>The shuffled list.</returns>
        // from: https://stackoverflow.com/a/1262619/13166789
        public static List<T> Shuffle<T>(List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = _rand.Next(n + 1);
                T val = list[k];
                list[k] = list[n];
                list[n] = val;
            }

            return list;
        }

        /// <summary>
        /// Shuffle an array with Fisher-Yates.
        /// </summary>
        /// <param name="array">The array to shuffle.</param>
        /// <typeparam name="T">The type of the elements in the array.</typeparam>
        /// <returns>The shuffled array.</returns>
        /// // from: https://stackoverflow.com/a/1262619/13166789
        public static T[] Shuffle<T>(T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                n--;
                int k = _rand.Next(n + 1);
                T val = array[k];
                array[k] = array[n];
                array[n] = val;
            }

            return array;
        }

        /// <summary>
        /// Pick a random element from a given sequence according to a function that picks weights for each
        /// element. This lets you pick things using weighted randoms, with different chances for each element.
        /// </summary>
        /// <param name="sequence">The sequence to choose from.</param>
        /// <param name="weightFunction">A function that, given an element, returns its weight.</param>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <returns>The chosen sequence element.</returns>
        // adapted from: https://stackoverflow.com/a/11930875/13166789
        public static T WeightedRandomElement<T>(IEnumerable<T> sequence, Func<T, float> weightFunction)
        {
            IEnumerable<T> enumeratedSequence = sequence as T[] ?? sequence.ToArray();
            float totalWeight = enumeratedSequence.Sum(weightFunction);
            float itemWeightIndex = Float() * totalWeight; // the weight of the item to select
            float currentWeightIndex = 0;

            foreach (var item in enumeratedSequence.Select(elt => new KeyValuePair<T, float>(elt, weightFunction(elt))))
            {
                currentWeightIndex += item.Value;

                // if we've hit or passed the weight we're looking for then this is the one to return
                if (currentWeightIndex >= itemWeightIndex) return item.Key;
            }

            return default;
        }
    }
}
