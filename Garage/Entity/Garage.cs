using System.Collections;
using System.Text;

namespace Garage.Entity;

public class Garage<T> : IEnumerable<T> where T : IVehicle {
    private T?[] _items;

    public int Capacity { get; }
    public int NumItems { get; private set; } = 0;

    public bool IsFull => NumItems >= _items.Length;




    public Garage(int capacity) {
        _items = new T?[capacity];
        Capacity = capacity; 
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
            output.AppendLine(item.ToString());
        }

        if (output.Length > 0 && output.ToString().EndsWith(Environment.NewLine)) {
            output.Remove(output.Length - Environment.NewLine.Length, Environment.NewLine.Length);
        }

        return output.ToString();
    }


    public IEnumerator<T> GetEnumerator() {
        return _items.OfType<T>().GetEnumerator();
    }


    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}