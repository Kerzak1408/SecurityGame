using System.Collections.Generic;

namespace Assets.Scripts.DataStructures
{
    /// <summary>
    /// Dictionary that assign defaultValue to the key that is accessed but does not exist in the dictionary.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class LazyDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        private TValue defaultValue;

        public LazyDictionary(TValue defaultValue)
        {
            this.defaultValue = defaultValue;
        }

        public new TValue this[TKey key]
        {
            get
            {
                if (!ContainsKey(key))
                {
                    this[key] = defaultValue;
                }
                return base[key];
            }
            set
            {
                base[key] = value;
            }
        }
    }
}
