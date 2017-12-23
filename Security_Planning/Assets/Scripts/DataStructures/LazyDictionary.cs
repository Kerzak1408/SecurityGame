using System.Collections.Generic;

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
