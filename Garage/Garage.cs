using System.Collections;

namespace Garage;

public class Garage<T> : IEnumerable<T> where T : IVehicle {
    private T?[] _items;

    private int _numItems = 0;

    public bool IsFull => _numItems >= _items.Length;

    
    public Garage(int capacity) {
        _items = new T?[capacity];
    }


    public bool Add(T input) {
        if (IsFull) {
            return false;
        }

        var index = Array.IndexOf(_items, null);

        if (index is -1) {
            return false;
        }

        _items[index] = input;
        return true;
    }


    public bool Remove(T item) {
        var index = Array.IndexOf(_items, item);

        if (index is -1) {
            return false;
        }

        _items[index] = default;

        return true;
    }


    public IEnumerator<T> GetEnumerator() {
        return _items.OfType<T>().GetEnumerator();
    }


    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}