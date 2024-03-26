using Garage.Entity.Factory;

namespace Garage.Services.FactoryProvider;

public class VehicleFactoryProvider : IVehicleFactoryProvider {
    private readonly IEnumerable<(string, IVehicleFactory)> _factories;


    public VehicleFactoryProvider(IEnumerable<(string Description, IVehicleFactory Factory)> factories) {
        _factories = factories;
    }

    public IEnumerable<(string Description, IVehicleFactory Factory)> GetAvailableFactories() {
        return _factories;
    }
}