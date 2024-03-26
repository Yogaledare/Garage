using Garage.Entity.Vehicles;
using Garage.Services.GarageHandler;
using Garage.Services.Input;

namespace Garage.Entity.Factory;

public class BoatFactory : VehicleFactory {
    public BoatFactory(IGarageHandler<IVehicle> garageHandler) : base(garageHandler) { }

    public override Type ProducedVehicleType => typeof(Boat);

    protected override IVehicle CreateSpecificVehicle() {
        var boat = new Boat {
            Length = InputRetriever.RetrieveInput(
                "Length: ",
                s => InputValidator.ValidateDoubleBounded(s, 1.0, 100.0) // Assuming meters
            )
        };
        return boat;
    }
}