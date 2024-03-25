using Garage.Entity.Vehicles;
using Garage.Services.GarageHandler;
using Garage.Services.Input;
using Garage.Services.UI;

namespace Garage.Entity.Factory;

public abstract class VehicleFactory(IGarageHandler<IVehicle> garageHandler) : IVehicleFactory {
    public IVehicle CreateVehicle() {
        var vehicle = CreateSpecificVehicle();
        SetCommonProperties(vehicle);
        return vehicle;
    }

    protected abstract IVehicle CreateSpecificVehicle();

    private void SetCommonProperties(IVehicle vehicle) {
        vehicle.LicencePlate = InputRetriever.RetrieveInput(
            "LicensePlate: ",
            s => InputValidator.ValidateLicensePlate(s, garageHandler)
        );
        vehicle.NumWheels = InputRetriever.RetrieveInput(
            "numWheels: ",
            s => InputValidator.ValidateNumberBounded(s, 0, 4)
        );
        vehicle.Color = InputRetriever.SelectFromEnum<VehicleColor>();
        vehicle.TopSpeed = InputRetriever.RetrieveInput(
            "TopSpeed: ",
            s => InputValidator.ValidateDoubleBounded(s, 0, 450)
        );
    }
}