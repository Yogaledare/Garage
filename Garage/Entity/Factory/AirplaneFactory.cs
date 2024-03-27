using Garage.Entity.Vehicles;
using Garage.Services.GarageHandler;
using Garage.Services.Input;

namespace Garage.Entity.Factory;

/// <summary>
/// A factory for creating <see cref="Airplane"/> instances, allowing specification of the number of engines.
/// </summary>
public class AirplaneFactory : VehicleFactory {
    /// <summary>
    /// Initializes a new instance of the <see cref="AirplaneFactory"/> class.
    /// </summary>
    /// <param name="garageHandler">The garage handler used to validate unique properties of vehicles, such as license plates.</param>
    public AirplaneFactory(IGarageHandler<IVehicle> garageHandler) : base(garageHandler) {
    }

    /// <summary>
    /// Specifies the type of vehicle produced by this factory.
    /// </summary>
    public override Type ProducedVehicleType => typeof(Airplane);

    /// <summary>
    /// Creates a new instance of an <see cref="Airplane"/>, prompting the user to specify the number of engines.
    /// </summary>
    /// <returns>A new <see cref="Airplane"/> instance with specified properties.</returns>
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