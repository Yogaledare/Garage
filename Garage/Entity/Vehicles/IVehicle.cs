namespace Garage.Entity.Vehicles;

/// <summary>
/// Defines the core properties and behaviors of a vehicle within the Garage application.
/// </summary>
public interface IVehicle 
{
    /// <summary>
    /// Gets or sets the vehicle's license plate.
    /// </summary>
    public string? LicencePlate { get; set; } 

    /// <summary>
    /// Gets or sets the number of wheels on the vehicle.
    /// </summary>
    int? NumWheels { get; set; } 

    /// <summary>
    /// Gets or sets the color of the vehicle.
    /// </summary>
    VehicleColor? Color { get; set; } 

    /// <summary>
    /// Gets or sets the top speed of the vehicle (presumably in a standard unit like km/h or mph).
    /// </summary>
    int? TopSpeed { get; set; } 

    /// <summary>
    /// Provides a string representation of the vehicle.  This will likely be overridden by concrete vehicle types.
    /// </summary>
    /// <returns>A string representing the vehicle's essential information.</returns>
    string ToString(); 
}
