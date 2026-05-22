using System;
using System.Collections.Generic;

public class Cache<TKey, TValue> where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> _storage = new Dictionary<TKey, TValue>();

    public void Add(TKey key, TValue value)
    {
        _storage[key] = value;
    }

    public TValue? Get(TKey key)
    {
        if (_storage.TryGetValue(key, out TValue? value))
        {
            return value;
        }
        return default;
    }

    public bool Contains(TKey key)
    {
        return _storage.ContainsKey(key);
    }
}