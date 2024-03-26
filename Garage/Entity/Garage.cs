using System.Collections;
using System.Text;
using Garage.Entity.Vehicles;

namespace Garage.Entity;

public class Garage<T> : IEnumerable<T> where T : IVehicle {
    private T?[] _items;

    public int Capacity { get; }

    public int NumItems { get; private set; }

    public bool IsFull => NumItems >= _items.Length;


    public Garage(int capacity) {
        _items = new T?[capacity];
        Capacity = capacity;
    }


    public bool Add(T input) {
        if (IsFull) {
            return false;
        }

        var indexSameItem = Array.IndexOf(_items, input);

        if (indexSameItem != -1) {
            return false;
        }

        var indexFirstEmpty = Array.IndexOf(_items, null);

        if (indexFirstEmpty is -1) {
            return false;
        }

        _items[indexFirstEmpty] = input;
        NumItems++;
        return true;
    }


    public bool Remove(T item) {
        var index = Array.IndexOf(_items, item);

        if (index is -1) {
            return false;
        }

        _items[index] = default;
        NumItems--;
        return true;
    }


    public string ShortDescription() {
        return $"Garage with capacity = {Capacity}, #stored = {NumItems}";
    }


    public override string ToString() {
        var output = new StringBuilder();

        output.AppendLine(ShortDescription());

        var itemsNotNull = _items.Where(item => item is not null);

        foreach (var item in itemsNotNull) {
            output.Append("    ");
            output.AppendLine(item?.ToString());
        }

        return output.ToString().TrimEnd();
    }


    public IEnumerator<T> GetEnumerator() {
        return _items.OfType<T>().GetEnumerator();
    }


    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}