using System;
using System.Collections.Generic;

public class Repository<T> where T : Animal
{
    private readonly List<T> _items = new List<T>();

    public void Add(T item)
    {
        _items.Add(item);
    }

    public List<T> GetAll()
    {
        return _items;
    }

    public List<T> Find(Predicate<T> match)
    {
        return _items.FindAll(match);
    }
}