using Garage.Entity.Factory;

namespace Garage.Services.FactoryProvider;

public interface IVehicleFactoryProvider {
    IEnumerable<(string Description, IVehicleFactory Factory)> GetAvailableFactories();
}
