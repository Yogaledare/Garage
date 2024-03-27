using Garage.Entity.Vehicles;
using Garage.Services.GarageHandler;
using static Garage.Services.Input.InputRetriever;

namespace Garage.Entity.Factory;

/// <summary>
/// Provides a base implementation for vehicle factories, handling common vehicle creation steps.
/// </summary>
public abstract class VehicleFactory : IVehicleFactory {
    private readonly IGarageHandler<IVehicle> _garageHandler;

    /// <summary>
    /// Initializes a new instance of the VehicleFactory class.
    /// </summary>
    /// <param name="garageHandler">An instance of IGarageHandler for interacting with garages.</param>
    protected VehicleFactory(IGarageHandler<IVehicle> garageHandler) {
        _garageHandler = garageHandler;
    }

    /// <summary>
    /// Creates a vehicle, setting its common properties using user input.
    /// </summary>
    /// <returns>A new instance of a vehicle with its properties set.</returns>
    public IVehicle CreateVehicle() {
        var vehicle = CreateSpecificVehicle();
        SetCommonProperties(vehicle);
        return vehicle;
    }

    /// <summary>
    /// Gets the type of vehicle produced by this factory.
    /// </summary>
    public abstract Type ProducedVehicleType { get; }

    /// <summary>
    /// When implemented in a derived class, creates a specific type of vehicle (template method pattern).
    /// </summary>
    /// <returns>A new instance of a specific type of vehicle.</returns>
    protected abstract IVehicle CreateSpecificVehicle();

    /// <summary>
    /// Sets common properties for a vehicle such as license plate, number of wheels, color, and top speed by prompting the user for input.
    /// </summary>
    /// <param name="vehicle">The vehicle instance whose properties are to be set.</param>
    private void SetCommonProperties(IVehicle vehicle) {
        EnterLicensePlate(vehicle, _garageHandler);
        EnterNumWheels(vehicle);
        EnterVehicleColor(vehicle);
        EnterTopSpeed(vehicle);
    }
}