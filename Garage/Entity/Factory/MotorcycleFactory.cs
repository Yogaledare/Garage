using Garage.Entity.Vehicles;
using Garage.Services.GarageHandler;
using Garage.Services.Input;

namespace Garage.Entity.Factory;

/// <summary>
/// A factory for creating <see cref="Motorcycle"/> instances, allowing specification of the cylinder volume.
/// </summary>
public class MotorcycleFactory : VehicleFactory {
    /// <summary>
    /// Initializes a new instance of the <see cref="MotorcycleFactory"/> class.
    /// </summary>
    /// <param name="garageHandler">The garage handler used to validate unique properties of vehicles, such as license plates.</param>
    public MotorcycleFactory(IGarageHandler<IVehicle> garageHandler) : base(garageHandler) { }

    /// <summary>
    /// Specifies the type of vehicle produced by this factory.
    /// </summary>
    public override Type ProducedVehicleType => typeof(Motorcycle);

    /// <summary>
    /// Creates a new instance of a <see cref="Motorcycle"/>, prompting the user to specify the cylinder volume.
    /// </summary>
    /// <returns>A new <see cref="Motorcycle"/> instance with specified properties.</returns>
    protected override IVehicle CreateSpecificVehicle() {
        var motorcycle = new Motorcycle {
            CylinderVolume = InputRetriever.RetrieveInput(
                "CylinderVolume: ",
                s => InputValidator.ValidateNumberBounded(s, 50, 1500) // Assuming CC
            )
        };
        return motorcycle;
    }
}