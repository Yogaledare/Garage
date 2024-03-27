using Garage.Entity.Vehicles;

namespace Garage.Entity.Factory;

/// <summary>
/// Defines a contract for a factory that creates vehicles of a specific type.
/// </summary>
public interface IVehicleFactory {
    /// <summary>
    /// Creates a new instance of the vehicle type produced by this factory.
    /// </summary>
    /// <returns>A new IVehicle instance.</returns>
    public IVehicle CreateVehicle();
    
    /// <summary>
    /// Gets the type of vehicle this factory produces.
    /// </summary>
    Type ProducedVehicleType { get; }
}