using FluentValidation;
using Garage.Entity.Vehicles;
using Garage.Services.GarageHandler;
using Garage.Services.Input;
using Garage.Services.UI;

namespace Garage.Entity.Factory;

public abstract class VehicleFactory<TProperties> : IVehicleFactory<TProperties> where TProperties : class {
    
    private readonly IGarageHandler<IVehicle> _garageHandler;

    protected VehicleFactory(IGarageHandler<IVehicle> garageHandler) {
        _garageHandler = garageHandler;
    }

    public IVehicle CreateVehicle(TProperties properties) {
        var vehicle = CreateSpecificVehicle(properties);
        SetCommonProperties(vehicle);
        return vehicle;
    }

    public abstract Type ProducedVehicleType { get; }
    
    public abstract TProperties GetExpectedProperties(); 

    protected abstract IVehicle CreateSpecificVehicle();

    private void SetCommonProperties(IVehicle vehicle) {
        
        vehicle.LicencePlate = InputRetriever.RetrieveInput(
            "LicensePlate: ",
            s => InputValidator.ValidateLicensePlate(s, _garageHandler)
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