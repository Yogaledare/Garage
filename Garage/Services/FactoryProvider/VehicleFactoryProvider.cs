using Garage.Entity.Factory;

namespace Garage.Services.FactoryProvider;

/// <summary>
/// Provides access to a collection of vehicle factories, each capable of creating instances of specific vehicle types.
/// </summary>
public class VehicleFactoryProvider : IVehicleFactoryProvider {
    private readonly IEnumerable<(string, IVehicleFactory)> _factories;

    /// <summary>
    /// Initializes a new instance of the VehicleFactoryProvider class.
    /// </summary>
    /// <param name="factories">A collection of tuples containing factory descriptions and their corresponding IVehicleFactory instances.</param>
    public VehicleFactoryProvider(IEnumerable<(string Description, IVehicleFactory Factory)> factories) {
        _factories = factories;
    }

    /// <summary>
    /// Retrieves the available vehicle factories along with their descriptions.
    /// </summary>
    /// <returns>An enumerable collection of tuples, where each tuple contains a description of the factory and the factory instance itself.</returns>
    public IEnumerable<(string Description, IVehicleFactory Factory)> GetAvailableFactories() {
        return _factories;
    }
}