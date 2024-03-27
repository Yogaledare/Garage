using Garage.Entity.Vehicles;
using Garage.Services.GarageHandler;
using Garage.Services.Input;

namespace Garage.Entity.Factory;

/// <summary>
/// A factory for creating <see cref="Bus"/> instances, allowing specification of the number of seats.
/// </summary>
public class BusFactory : VehicleFactory {
    /// <summary>
    /// Initializes a new instance of the <see cref="BusFactory"/> class.
    /// </summary>
    /// <param name="garageHandler">The garage handler used to validate unique properties of vehicles, such as license plates.</param>
    public BusFactory(IGarageHandler<IVehicle> garageHandler) : base(garageHandler) {
    }

    /// <summary>
    /// Specifies the type of vehicle produced by this factory.
    /// </summary>
    public override Type ProducedVehicleType => typeof(Bus);

    /// <summary>
    /// Creates a new instance of a <see cref="Bus"/>, prompting the user to specify the number of seats.
    /// </summary>
    /// <returns>A new <see cref="Bus"/> instance with specified properties.</returns>
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