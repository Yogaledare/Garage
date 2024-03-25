namespace Garage.Entity.Factory.FactoryProvider;

public interface IVehicleFactoryProvider {
    IEnumerable<(string Description, IVehicleFactory Factory)> GetAvailableFactories();
}
