using System.Text;

namespace Garage.Entity.Vehicles;

/// <summary>
/// Represents the abstract base class for all vehicle types within the Garage application.
/// </summary>
public abstract class Vehicle : IVehicle {
    /// <summary>
    /// Gets or sets the vehicle's license plate.
    /// </summary>
    public string? LicencePlate { get; set;  }

    /// <summary>
    /// Gets or sets the number of wheels on the vehicle.
    /// </summary>
    public int? NumWheels { get; set; }

    /// <summary>
    /// Gets or sets the color of the vehicle.
    /// </summary>
    public VehicleColor? Color { get; set; }

    /// <summary>
    /// Gets or sets the top speed of the vehicle (presumably in a standard unit like km/h or mph).
    /// </summary>
    public int? TopSpeed { get; set; }
        
    /// <summary>
    /// Provides a default string representation of a vehicle.  Concrete vehicle classes will typically override this for more specific output.
    /// </summary>
    /// <returns>A string containing basic vehicle information.</returns>
    public override string ToString() {
        StringBuilder stringBuilder = new StringBuilder();

        stringBuilder.Append($"{GetType().Name} ");
        stringBuilder.Append(LicencePlate is not null ? $"LicencePlate={LicencePlate} " : "");
        stringBuilder.Append(NumWheels is not null ? $"NumWheels={NumWheels} " : "");
        stringBuilder.Append(Color is not null ? $"Color={Color} " : "");
        stringBuilder.Append(TopSpeed is not null ? $"TopSpeed={TopSpeed} " : "");

        return stringBuilder.ToString();
    }
}