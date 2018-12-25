using System.Collections.Generic;

namespace Athanor.Collections
{
    public static class DictionaryExt
    {
        public static Dictionary<TKey, TValue> FormDictionary<TKey, TValue>(
            this IEnumerable<KeyValuePair<TKey, TValue>> iekvp)
        {
            Dictionary<TKey, TValue> newDict = new Dictionary<TKey, TValue>();

            foreach (KeyValuePair<TKey, TValue> kv in iekvp)
                newDict[kv.Key] = kv.Value;

            return newDict;
        }
    }
}
