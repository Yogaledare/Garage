using Garage.Entity.Vehicles;
using Garage.Services.GarageHandler;
using static Garage.Services.Input.InputRetriever; 

namespace Garage.Entity.Factory;

public abstract class VehicleFactory(IGarageHandler<IVehicle> garageHandler) : IVehicleFactory {
    public IVehicle CreateVehicle() {
        var vehicle = CreateSpecificVehicle();
        SetCommonProperties(vehicle);
        return vehicle;
    }

    public abstract Type ProducedVehicleType { get; }


    protected abstract IVehicle CreateSpecificVehicle();

    private void SetCommonProperties(IVehicle vehicle) {
        EnterLicensePlate(vehicle, garageHandler);
        EnterNumWheels(vehicle);
        EnterVehicleColor(vehicle);
        EnterTopSpeed(vehicle);
    }
}


      
//
//
// public static void <T>(T vehicle) where T : IVehicle {
//     vehicle.LicencePlate = RetrieveInput("License plate: ", ValidateLicensePlateSearch);
// }
//
//
// public static void EnterNumWheels<T>(T vehicle) where T : IVehicle {
//     vehicle.NumWheels = RetrieveInput("NumWheels: ", s => ValidateNumberBounded(s, 0, 8));
// }
//
//
// public static void EnterVehicleColor<T>(T vehicle) where T : IVehicle {
//     vehicle.Color = SelectFromEnum<VehicleColor>();
// }
//
// public static void EnterTopSpeed<T>(T vehicle) where T : IVehicle {
//     vehicle.TopSpeed = RetrieveInput("TopSpeed: ", s => ValidateNumberBounded(s, 0, 450)); 
// }
        
// vehicle.LicencePlate = InputRetriever.RetrieveInput(
//     "LicensePlate: ",
//     s => InputValidator.ValidateLicensePlate(s, garageHandler)
// );
// vehicle.NumWheels = InputRetriever.RetrieveInput(
//     "numWheels: ",
//     s => InputValidator.ValidateNumberBounded(s, 0, 4)
// );
// vehicle.Color = InputRetriever.SelectFromEnum<VehicleColor>();
// vehicle.TopSpeed = InputRetriever.RetrieveInput(
//     "TopSpeed: ",
//     s => InputValidator.ValidateDoubleBounded(s, 0, 450)
// );

