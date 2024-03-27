namespace Garage.Entity.Vehicles;

/// <summary>
/// Represents a motorcycle vehicle within the Garage application.
/// </summary>
public class Motorcycle : Vehicle {
    /// <summary>
    /// Gets or sets the engine's cylinder volume (likely in cubic centimeters).
    /// </summary>
    public int CylinderVolume { get; set; }

    /// <summary>
    /// Overrides the base `ToString` method to provide a custom representation of a motorcycle.
    /// </summary>
    /// <returns>A string containing basic vehicle information and the cylinder volume.</returns>
    public override string ToString() {
        return base.ToString() + $"CylinderVolume={CylinderVolume}";
    }
}