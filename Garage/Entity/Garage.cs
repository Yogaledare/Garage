using System.Collections;
using System.Text;
using Garage.Entity.Vehicles;

namespace Garage.Entity;

/// <summary>
/// Represents a generic garage that can store any type of vehicle implementing the IVehicle interface.
/// </summary>
/// <typeparam name="T">The type of vehicle the garage can store. Must implement the IVehicle interface.</typeparam>
public class Garage<T> : IEnumerable<T> where T : IVehicle {
    private T?[] _items;

    /// <summary>
    /// Gets the total capacity of the garage.
    /// </summary>
    public int Capacity { get; }

    /// <summary>
    /// Gets the current number of vehicles stored in the garage.
    /// </summary>
    public int NumItems { get; private set; }

    /// <summary>
    /// Indicates whether the garage is full.
    /// </summary>
    public bool IsFull => NumItems >= _items.Length;

    /// <summary>
    /// Initializes a new instance of the Garage class with the specified capacity.
    /// </summary>
    /// <param name="capacity">The total capacity of the garage.</param>
    public Garage(int capacity) {
        _items = new T?[capacity];
        Capacity = capacity;
    }

    /// <summary>
    /// Attempts to add a vehicle to the garage.
    /// </summary>
    /// <param name="input">The vehicle to add to the garage.</param>
    /// <returns>true if the vehicle was successfully added; otherwise, false.</returns>
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

    /// <summary>
    /// Attempts to remove a vehicle from the garage.
    /// </summary>
    /// <param name="item">The vehicle to remove from the garage.</param>
    /// <returns>true if the vehicle was successfully removed; otherwise, false.</returns>
    public bool Remove(T item) {
        var index = Array.IndexOf(_items, item);

        if (index is -1) {
            return false;
        }

        _items[index] = default;
        NumItems--;
        return true;
    }

    /// <summary>
    /// Provides a short description of the garage, including its capacity and the number of vehicles stored.
    /// </summary>
    /// <returns>A string representation of the garage's capacity and stored vehicle count.</returns>
    public string ShortDescription() {
        return $"Garage with capacity = {Capacity}, #stored = {NumItems}";
    }

    /// <summary>
    /// Returns a string representation of the garage, including a list of all vehicles stored.
    /// </summary>
    /// <returns>A detailed string representation of the garage and its contents.</returns>
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

    /// <summary>
    /// Returns an enumerator that iterates through the collection of stored vehicles.
    /// </summary>
    /// <returns>An enumerator for the collection of vehicles.</returns>
    public IEnumerator<T> GetEnumerator() {
        return _items.OfType<T>().GetEnumerator();
    }

    /// <summary>
    /// Returns a non-generic enumerator that iterates through the collection of stored vehicles.
    /// </summary>
    /// <returns>A non-generic enumerator for the collection of vehicles.</returns>
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}