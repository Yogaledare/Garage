using Garage.Entity.Vehicles;
using Garage.Services.GarageHandler;
using Garage.Services.Input;

namespace Garage.Entity.Factory;

/// <summary>
/// A factory for creating <see cref="Boat"/> instances, allowing specification of the boat's length.
/// </summary>
public class BoatFactory : VehicleFactory {
    /// <summary>
    /// Initializes a new instance of the <see cref="BoatFactory"/> class.
    /// </summary>
    /// <param name="garageHandler">The garage handler used to validate unique properties of vehicles, such as license plates.</param>
    public BoatFactory(IGarageHandler<IVehicle> garageHandler) : base(garageHandler) {
    }

    /// <summary>
    /// Specifies the type of vehicle produced by this factory.
    /// </summary>
    public override Type ProducedVehicleType => typeof(Boat);

    /// <summary>
    /// Creates a new instance of a <see cref="Boat"/>, prompting the user to specify the length.
    /// </summary>
    /// <returns>A new <see cref="Boat"/> instance with specified properties.</returns>
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