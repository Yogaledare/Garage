using System.Text;
using Garage.Entity;
using Garage.Entity.Vehicles;
using LanguageExt.Common;

namespace Garage.Services.GarageHandler;

/// <summary>
/// Defines a contract for managing garages and vehicles within the Garage application.
/// </summary>
/// <typeparam name="T">The specific type of IVehicle the garage handler manages.</typeparam>
public class GarageHandler<T> : IGarageHandler<T> where T : IVehicle {

    /// <summary>
    /// Gets a list of all managed garages.
    /// </summary>
    public List<Garage<T>> Garages { get; } = [];

    /// <summary>
    /// Adds a vehicle to a specified garage. Performs validation checks.
    /// </summary>
    /// <param name="vehicle">The vehicle to be added.</param>
    /// <param name="garage">The garage where the vehicle should be parked.</param>
    /// <returns>A Result object. Success contains the added vehicle; Failure contains an Exception.</returns>
    public Result<T> AddVehicle(T vehicle, Garage<T> garage) {
        if (DoesLicencePlateExist(vehicle.LicencePlate)) {
            var error = new InvalidOperationException("Licence number already in use");
            return new Result<T>(error);
        }

        if (garage.IsFull) {
            var error = new InvalidOperationException("Garage full!");
            return new Result<T>(error);
        }

        garage.Add(vehicle);
        return vehicle;
    }

    /// <summary>
    /// Removes a vehicle from the garages based on its license plate.
    /// </summary>
    /// <param name="licensePlate">The license plate of the vehicle to remove.</param>
    /// <returns>True if the vehicle was found and removed; otherwise, false.</returns>
    public bool RemoveVehicle(string licensePlate) {
        var searchResult = FindVehicle(licensePlate);

        return searchResult.Match(
            Succ: tuple => {
                var (g, v) = tuple;
                g.Remove(v);
                return true;
            },
            Fail: ex => false
        );
    }

    /// <summary>
    /// Checks if a vehicle with the given license plate exists in any of the garages.
    /// </summary>
    /// <param name="licencePlate">The license plate to search for.</param>
    /// <returns>True if a vehicle with the license plate exists; otherwise, false.</returns>
    public bool DoesLicencePlateExist(string? licencePlate) {
        return Garages
            .SelectMany(garage => garage)
            .Any(vehicle => string.Equals(vehicle.LicencePlate, licencePlate, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Creates a new garage with a specified capacity. 
    /// </summary>
    /// <param name="capacity">The maximum capacity of the new garage.</param>
    public void CreateGarage(int capacity) {
        Garages.Add(new Garage<T>(capacity));
    }

    /// <summary>
    /// Finds a vehicle based on its license plate.
    /// </summary>
    /// <param name="licencePlate">The license plate of the vehicle to find.</param>
    /// <returns>A Result object. Success contains a tuple of the garage and vehicle; Failure contains an Exception.</returns>
    public Result<(Garage<T>, T)> FindVehicle(string licencePlate) {
        foreach (var garage in Garages) {
            foreach (var vehicle in garage) {
                if (string.Equals(vehicle.LicencePlate, licencePlate, StringComparison.OrdinalIgnoreCase)) {
                    return new Result<(Garage<T>, T)>((garage, vehicle));
                }
            }
        }

        var error = new InvalidOperationException("Cannot find vehicle");
        return new Result<(Garage<T>, T)>(error);
    }
    
    /// <summary>
    /// Counts the number of vehicles of each specified type across all garages.
    /// </summary>
    /// <param name="types">A collection of vehicle types to count.</param>
    /// <returns>A dictionary mapping vehicle types to their respective counts.</returns>
    public Dictionary<Type, int> CountVehicleTypes(HashSet<Type> types) {
        var output = new Dictionary<Type, int>();

        foreach (var type in types) {
            var count = Garages.SelectMany(g => g)
                .Count(v => v.GetType() == type);
            output[type] = count;
        }

        return output;
    }

    /// <summary>
    /// Generates a listing of all garages and their contents.
    /// </summary>
    /// <returns>A formatted string representing the garage contents.</returns>
    public string ListContents() {
        var output = new StringBuilder();

        output.AppendLine("Garages: ");

        foreach (var garage in Garages) {
            output.AppendLine(garage.ToString());
        }

        return output.ToString().TrimEnd();
    }
    
    /// <summary>
    /// Queries and filters vehicles based on the provided search criteria
    /// </summary>
    /// <param name="searchCriteria">An IVehicle object containing search parameters.</param>
    /// <returns>A list of vehicles matching the search criteria.</returns>  
    public List<T> QueryVehicles(T searchCriteria) {
        var query = Garages.SelectMany(g => g).AsQueryable();

        if (searchCriteria.LicencePlate != null)
            query = query.Where(vehicle => vehicle.LicencePlate == searchCriteria.LicencePlate);
        if (searchCriteria.NumWheels != null)
            query = query.Where(vehicle => vehicle.NumWheels == searchCriteria.NumWheels);
        if (searchCriteria.Color != null)
            query = query.Where(vehicle => vehicle.Color == searchCriteria.Color);
        if (searchCriteria.TopSpeed != null)
            query = query.Where(vehicle => vehicle.TopSpeed == searchCriteria.TopSpeed);

        return query.ToList();
    }

    
}
