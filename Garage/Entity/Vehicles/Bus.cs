namespace Garage.Entity.Vehicles;

/// <summary>
/// Represents a bus vehicle within the Garage application. 
/// </summary>
public class Bus : Vehicle {
    /// <summary>
    /// Gets or sets the number of seats on the bus.
    /// </summary>
    public int NumberOfSeats { get; set; }

    /// <summary>
    /// Overrides the base ToString method to provide a custom representation of a Bus.
    /// </summary>
    /// <returns>A string containing basic vehicle information and the number of seats.</returns>
    public override string ToString() {
        return base.ToString() + $"NumberOfSeats={NumberOfSeats}";
    }
}