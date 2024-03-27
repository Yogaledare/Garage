using Garage.Entity.Factory;

namespace Garage.Services.FactoryProvider;

/// <summary>
/// Defines a contract for a service that provides access to various vehicle factories.
/// </summary>
public interface IVehicleFactoryProvider {
    /// <summary>
    /// Retrieves a collection of available vehicle factories along with their descriptions.
    /// </summary>
    /// <returns>An enumerable collection of tuples, where each tuple contains:
    ///     * Description: A human-readable description of the factory.
    ///     * Factory: The corresponding IVehicleFactory instance.
    /// </returns>
    IEnumerable<(string Description, IVehicleFactory Factory)> GetAvailableFactories();
}
