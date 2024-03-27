namespace Garage.Entity.Vehicles;

/// <summary>
/// Represents a car vehicle within the Garage application.
/// </summary>
public class Car : Vehicle {
    /// <summary>
    /// Gets or sets the number of doors on the car.
    /// </summary>
    public int NumDoors { get; set; }
        
    /// <summary>
    /// Overrides the base `ToString` method to provide a custom representation of a car.
    /// </summary>
    /// <returns>A string containing basic vehicle information and the number of doors.</returns>
    public override string ToString()
    {
        return base.ToString() +  $"NumDoors={NumDoors}";
    }
}