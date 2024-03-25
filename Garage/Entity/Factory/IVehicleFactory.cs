using Garage.Entity.Vehicles;

namespace Garage.Entity.Factory;

public interface IVehicleFactory {
    public IVehicle CreateVehicle();
}