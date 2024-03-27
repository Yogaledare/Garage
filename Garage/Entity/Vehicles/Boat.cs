namespace Garage.Entity.Vehicles;

/// <summary>
/// Represents a boat vehicle within the Garage application.
/// </summary>
public class Boat : Vehicle {
    /// <summary>
    /// Gets or sets the length of the boat in meters.
    /// </summary>
    public double Length { get; set; }

    /// <summary>
    /// Overrides the base `ToString` method to provide a custom representation of a boat.
    /// </summary>
    /// <returns>A string containing basic vehicle information and the boat's length.</returns>
    public override string ToString() {
        return base.ToString() + $"Length={Length}m";
    }
}