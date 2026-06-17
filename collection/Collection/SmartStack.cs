using System.Collections;

namespace Collection;

public sealed class SmartStack<T> : IEnumerable<T>
{
    private T[] _items;
    private int _count;

    public SmartStack()
    {
        _items = new T[4];
        _count = 0;
    }

    public SmartStack(int capacity)
    {
        if (capacity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "Ёмкость не может быть отрицательной.");
        }
        _items = new T[capacity];
        _count = 0;
    }

    public SmartStack(IEnumerable<T> collection)
    {
        if (collection == null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        if (collection is ICollection<T> col)
        {
            _items = new T[col.Count];
            _count = 0;
            foreach (var item in col)
            {
                _items[_count++] = item;
            }
        }
        else if (collection is IReadOnlyCollection<T> readOnlyCol)
        {
            _items = new T[readOnlyCol.Count];
            _count = 0;
            foreach (var item in readOnlyCol)
            {
                _items[_count++] = item;
            }
        }
        else
        {
            _items = new T[4];
            _count = 0;
            foreach (var item in collection)
            {
                Push(item);
            }
        }
    }

    public int Count => _count;

    public int Capacity => _items.Length;

    public void Push(T item)
    {
        EnsureCapacity(_count + 1);
        _items[_count++] = item;
    }

    public void PushRange(IEnumerable<T> collection)
    {
        if (collection == null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        if (collection is ICollection<T> col)
        {
            EnsureCapacity(_count + col.Count);
            foreach (var item in col)
            {
                _items[_count++] = item;
            }
        }
        else if (collection is IReadOnlyCollection<T> readOnlyCol)
        {
            EnsureCapacity(_count + readOnlyCol.Count);
            foreach (var item in readOnlyCol)
            {
                _items[_count++] = item;
            }
        }
        else
        {
            foreach (var item in collection)
            {
                Push(item);
            }
        }
    }

    public T Pop()
    {
        if (_count == 0)
        {
            throw new InvalidOperationException("Стек пуст.");
        }

        _count--;
        T item = _items[_count];
        _items[_count] = default; 
        return item;
    }

    public T Peek()
    {
        if (_count == 0)
        {
            throw new InvalidOperationException("Стек пуст.");
        }

        return _items[_count - 1];
    }

    public bool Contains(T item)
    {
        var comparer = EqualityComparer<T>.Default;
        for (int i = 0; i < _count; i++)
        {
            if (comparer.Equals(_items[i], item))
            {
                return true;
            }
        }
        return false;
    }

    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= _count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Индекс находится вне диапазона элементов стека.");
            }
            return _items[_count - 1 - index];
        }
        set
        {
            if (index < 0 || index >= _count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Индекс находится вне диапазона элементов стека.");
            }
            _items[_count - 1 - index] = value;
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = _count - 1; i >= 0; i--)
        {
            yield return _items[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private void EnsureCapacity(int min)
    {
        if (_items.Length < min)
        {
            int newCapacity = _items.Length == 0 ? 4 : _items.Length * 2;
            if (newCapacity < min)
            {
                newCapacity = min;
            }

            T[] newItems = new T[newCapacity];
            for (int i = 0; i < _count; i++)
            {
                newItems[i] = _items[i];
            }
            _items = newItems;
        }
    }
}