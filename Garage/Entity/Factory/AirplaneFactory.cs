using Garage.Entity.Vehicles;
using Garage.Services.GarageHandler;
using Garage.Services.Input;

namespace Garage.Entity.Factory;

public class AirplaneFactory : VehicleFactory {
    public AirplaneFactory(IGarageHandler<IVehicle> garageHandler) : base(garageHandler) { }

    public override Type ProducedVehicleType => typeof(Airplane);

    protected override IVehicle CreateSpecificVehicle() {
        var airplane = new Airplane {
            NumberOfEngines = InputRetriever.RetrieveInput(
                "NumberOfEngines: ",
                s => InputValidator.ValidateNumberBounded(s, 1, 8)
            )
        };
        return airplane;
    }
}