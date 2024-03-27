using Garage.Entity;
using Garage.Entity.Vehicles;
using LanguageExt.Common;

namespace Garage.Services.GarageHandler;

/// <summary>
/// Defines a contract for managing garages and vehicles within the Garage application.
/// This interface provides methods for adding, removing, querying vehicles and  managing garages. 
/// </summary>
/// <typeparam name="T">The specific type of IVehicle the garage handler manages.</typeparam>
public interface IGarageHandler<T> where T : IVehicle {
    /// <summary>
    /// Gets a list of all managed garages.
    /// </summary>
    List<Garage<T>> Garages { get; }
    
    /// <summary>
    /// Adds a vehicle to a specified garage. Performs validation checks to ensure the license plate is unique and the garage has space.
    /// </summary>
    /// <param name="vehicle">The vehicle to be added.</param>
    /// <param name="garage">The garage where the vehicle should be parked.</param>
    /// <returns>A Result object. Success contains the added vehicle; Failure contains an Exception indicating the reason for failure.</returns>
    Result<T> AddVehicle(T vehicle, Garage<T> garage);
    /// <summary>
    /// Removes a vehicle from the garages based on its license plate.
    /// </summary>
    /// <param name="licensePlate">The license plate of the vehicle to remove.</param>
    /// <returns>True if the vehicle was found and removed; otherwise, false.</returns>
    
    bool RemoveVehicle(string licensePlate);
    
    /// <summary>
    /// Checks if a vehicle with the given license plate exists in any of the garages.
    /// </summary>
    /// <param name="licencePlate">The license plate to search for.</param>
    /// <returns>True if a vehicle with the license plate exists; otherwise, false.</returns>
    bool DoesLicencePlateExist(string? licencePlate);
  
    /// <summary>
    /// Creates a new garage with a specified capacity. 
    /// </summary>
    /// <param name="capacity">The maximum capacity of the new garage.</param>
    void CreateGarage(int capacity);
 
    /// <summary>
    /// Generates a listing of all garages and their contents.
    /// </summary>
    /// <returns>A formatted string representing the garage contents.</returns>
    public string ListContents();
  
    /// <summary>
    /// Finds a vehicle based on its license plate.
    /// </summary>
    /// <param name="licencePlate">The license plate of the vehicle to find.</param>
    /// <returns>A Result object. Success contains a tuple of the garage where the vehicle was found and the vehicle itself; Failure contains an Exception.</returns>
    Result<(Garage<T>, T)> FindVehicle(string licencePlate);
 
    /// <summary>
    /// Counts the number of vehicles of each specified type across all garages.
    /// </summary>
    /// <param name="types">A collection of vehicle types to count.</param>
    /// <returns>A dictionary mapping vehicle types to their respective counts.</returns>
    Dictionary<Type, int> CountVehicleTypes(HashSet<Type> types);
 
    /// <summary>
    /// Queries and filters vehicles based on the provided search criteria, such as license plate, number of wheels, color, or top speed. 
    /// </summary>
    /// <param name="searchCriteria">An IVehicle object containing search parameters.</param>
    /// <returns>A list of vehicles matching the search criteria.</returns>  
    List<T> QueryVehicles(T searchCriteria);
}