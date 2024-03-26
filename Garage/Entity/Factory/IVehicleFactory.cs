using Garage.Entity.Vehicles;

namespace Garage.Entity.Factory;

public interface IVehicleFactory<TProperties> where TProperties : class {
    public IVehicle CreateVehicle(TProperties properties);
    Type ProducedVehicleType { get; }
    TProperties GetExpectedProperties();
}