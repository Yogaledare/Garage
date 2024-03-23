namespace Garage;

public interface IVehicle {
    public string LicencePlate { get; }
    int NumWheels { get; }
    VehicleColor Color { get; }
    double TopSpeed { get; }
}