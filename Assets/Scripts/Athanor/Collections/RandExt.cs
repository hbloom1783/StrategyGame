using System.Linq;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Athanor.Collections
{
    public static class RandExt
    {
        public static T RandomPick<T>(this ICollection<T> input)
        {
            return input.ElementAt(Random.Range(0, input.Count - 1));
        }

        public static T RandomTake<T>(this ICollection<T> input)
        {
            T item = input.RandomPick();
            input.Remove(item);
            return item;
        }
    }
}