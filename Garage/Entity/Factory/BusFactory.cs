using Garage.Entity.Vehicles;
using Garage.Services.GarageHandler;
using Garage.Services.Input;

namespace Garage.Entity.Factory;

public class BusFactory : VehicleFactory {
    public BusFactory(IGarageHandler<IVehicle> garageHandler) : base(garageHandler) { }

    public override Type ProducedVehicleType => typeof(Bus);

    protected override IVehicle CreateSpecificVehicle() {
        var bus = new Bus {
            NumberOfSeats = InputRetriever.RetrieveInput(
                "NumberOfSeats: ",
                s => InputValidator.ValidateNumberBounded(s, 10, 50)
            )
        };
        return bus;
    }
}