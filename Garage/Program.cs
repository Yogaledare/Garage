using System.Drawing;

namespace Garage;

class Program {
    static void Main(string[] args) {
        var garage = new Garage<Car>(4);

        var car = new Car(4, VehicleColor.Black, "ABC123", 160, 5); 
        garage.Add(car);

        var r1 = garage.GetAll();

        foreach (var c in r1) {
            Console.WriteLine(c);
        }

    }


    public abstract class Vehicle : IVehicle {
        protected Vehicle(int numWheels, VehicleColor color, string licencePlate, double topSpeed) {
            NumWheels = numWheels;
            Color = color;
            LicencePlate = licencePlate;
            TopSpeed = topSpeed;
        }

        public int NumWheels { get; }
        public VehicleColor Color { get; }
        public string LicencePlate { get; }
        public double TopSpeed { get; }
    }


    public class Car : Vehicle {
        public Car(int numWheels, VehicleColor color, string licencePlate, double topSpeed, int numDoors) : base(numWheels,
            color,
            licencePlate, topSpeed) {
            NumDoors = numDoors;
        }

        public int NumDoors { get; }
    }


    public enum VehicleColor {
        Black,
        White,
    }


    public interface IVehicle {
        int NumWheels { get; }
        VehicleColor Color { get; }
        public string LicencePlate { get; }
        double TopSpeed { get; }
    }


    public class Garage<T> where T : IVehicle {
        private T[] _items;

        private int _size;

        private bool IsFull => _size >= _items.Length;


        public Garage(int size, int preFilled = 0) {
            _items = new T[size];
        }


        public IEnumerable<T> GetAll() {
            return _items.Take(_size);
        }


        public void Add(T item) {
            if (IsFull) {
                return; 
            }

            _items[_size++] = item; 
        }


        public bool Remove(T item) {
            var index = IndexOf(item);

            if (index is -1) return false;
            
            RemoveAt(index);
            return true;
        }


        private int IndexOf(T item) => Array.IndexOf(_items, item, 0);


        private void RemoveAt(int index) {
            _size--;
            if (index < _size) {
                Array.Copy(_items, index + 1, _items, index, _size - index);
            }
        }
    }
}


// {
// get {
// var size = _items.Length;
// var filled = _nextIndex;
// return filled < size; 
// }
// }