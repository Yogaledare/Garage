namespace Garage.Entity.Vehicles;

public interface IVehicle {
    public string? LicencePlate { get; set; }
    int NumWheels { get; set; }
    VehicleColor Color { get; set; }
    double TopSpeed { get; set; }
    string ToString();
    // public IVehicle CreateVehicle();
}