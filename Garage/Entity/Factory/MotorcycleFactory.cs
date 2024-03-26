using Garage.Entity.Vehicles;
using Garage.Services.GarageHandler;
using Garage.Services.Input;

namespace Garage.Entity.Factory;

public class MotorcycleFactory : VehicleFactory {
    public MotorcycleFactory(IGarageHandler<IVehicle> garageHandler) : base(garageHandler) { }

    public override Type ProducedVehicleType => typeof(Motorcycle);

    protected override IVehicle CreateSpecificVehicle() {
        var motorcycle = new Motorcycle {
            CylinderVolume = InputRetriever.RetrieveInput(
                "CylinderVolume: ",
                s => InputValidator.ValidateNumberBounded(s, 50, 1500) // Assuming CC
            )
        };
        return motorcycle;
    }
}