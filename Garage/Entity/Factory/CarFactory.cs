using Garage.Entity.Vehicles;
using Garage.Services.GarageHandler;
using Garage.Services.Input;

namespace Garage.Entity.Factory;

/// <summary>
/// A factory for creating <see cref="Car"/> instances, allowing specification of the number of doors.
/// </summary>
public class CarFactory : VehicleFactory {
    /// <summary>
    /// Initializes a new instance of the <see cref="CarFactory"/> class.
    /// </summary>
    /// <param name="garageHandler">The garage handler used to validate unique properties of vehicles, such as license plates.</param>
    public CarFactory(IGarageHandler<IVehicle> garageHandler) : base(garageHandler) { }

    /// <summary>
    /// Specifies the type of vehicle produced by this factory.
    /// </summary>
    public override Type ProducedVehicleType => typeof(Car);

    /// <summary>
    /// Creates a new instance of a <see cref="Car"/>, prompting the user to specify the number of doors.
    /// </summary>
    /// <returns>A new <see cref="Car"/> instance with specified properties.</returns>
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