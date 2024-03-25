using Garage.Entity.Vehicles;
using Garage.Services.GarageHandler;
using Garage.Services.Input;
using Garage.Services.UI;

namespace Garage.Entity.Factory;

public class CarFactory(IGarageHandler<IVehicle> garageHandler) : VehicleFactory(garageHandler) {
    protected override IVehicle CreateSpecificVehicle() {
        var car = new Car {
            NumDoors = InputRetriever.RetrieveInput(
                "NumDoors: ",
                s => InputValidator.ValidateNumberBounded(s, 0, 5)
            )
        };
        return car;
    }
}