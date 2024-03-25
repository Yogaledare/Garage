using Garage.Entity.Vehicles;
using Garage.Services.GarageHandler;

namespace Garage.Entity.Factory;

public class TruckFactory(IGarageHandler<IVehicle> garageHandler) : VehicleFactory(garageHandler) {
    
    public override Type ProducedVehicleType { get; }
    
    protected override IVehicle CreateSpecificVehicle() {
        throw new NotImplementedException();
    }
    
}