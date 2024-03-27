namespace Garage.Entity.Vehicles;

/// <summary>
/// Represents an airplane vehicle within the Garage application.
/// </summary>
public class Airplane : Vehicle {
    /// <summary>
    /// Gets or sets the number of engines on the airplane.
    /// </summary>
    public int NumberOfEngines { get; set; }

    /// <summary>
    /// Overrides the base ToString method to provide a custom representation of an Airplane.
    /// </summary>
    /// <returns>A string containing basic vehicle information and the number of engines.</returns>
    public override string ToString() {
        return base.ToString() + $"NumberOfEngines={NumberOfEngines}";
    }
}