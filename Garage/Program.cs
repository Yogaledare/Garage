namespace Garage;

class Program {
    static void Main(string[] args) {
        Console.WriteLine("Hello, World!");
    }


    public abstract class Vehicle : IVehicle {
        protected Vehicle(int numWheels, string color, string licencePlate, double topSpeed) {
            NumWheels = numWheels;
            Color = color;
            LicencePlate = licencePlate;
            TopSpeed = topSpeed;
        }

        public int NumWheels { get; }
        public string Color { get; }
        public string LicencePlate { get; }
        public double TopSpeed { get; }
    }


    public class Car : Vehicle {
        public Car(int numWheels, string color, string licencePlate, double topSpeed, int numDoors) : base(numWheels, color,
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
        string Color { get; }
        public string LicencePlate { get; }
        double TopSpeed { get; }
    }


    public class Garage<T> where T : IVehicle {
    }
}