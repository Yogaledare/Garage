namespace Garage.Entity.Vehicles;

public interface IVehicle {
    public string? LicencePlate { get; set; }
    int? NumWheels { get; set; }
    VehicleColor? Color { get; set; }
    int? TopSpeed { get; set; }
    string ToString();
}