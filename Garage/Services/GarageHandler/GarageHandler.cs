using System.Text;
using FluentValidation;
using Garage.Entity;
using Garage.Entity.Vehicles;
using Garage.Services.UI;
using LanguageExt.Common;

namespace Garage.Services.GarageHandler;

public class GarageHandler<T> : IGarageHandler<T> where T : IVehicle {
    // // private readonly IUI _ui; 
    // private readonly IVehicleFactoryProvider _factoryProvider;
    //
    // public GarageHandler(IVehicleFactoryProvider factoryProvider) {
    //     _factoryProvider = factoryProvider;
    // }

    public List<Garage<T>> Garages { get; } = [];

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

    public bool DoesLicencePlateExist(string? licencePlate) {
        return Garages
            .SelectMany(garage => garage)
            .Any(vehicle => string.Equals(vehicle.LicencePlate, licencePlate, StringComparison.OrdinalIgnoreCase));
    }

    public void CreateGarage(int capacity) {
        Garages.Add(new Garage<T>(capacity));
    }

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

    public Dictionary<Type, int> CountVehicleTypes(HashSet<Type> types) {
        var output = new Dictionary<Type, int>();

        foreach (var type in types) {
            // var type = factory.ProducedVehicleType;
            var count = Garages.SelectMany(g => g)
                .Count(v => v.GetType() == type);
            output[type] = count;
        }

        return output;
    }


    public string ListContents() {
        var output = new StringBuilder();

        output.AppendLine("Garages: ");

        foreach (var garage in Garages) {
            output.AppendLine(garage.ToString());
            // output.AppendLine($"Capacity = {garage.Capacity}, #stored = {garage.NumItems}");

            // foreach (var item in garage) {
            // output.AppendLine(item.ToString());
            // }
        }

        // if (output.Length > 0 && output.ToString().EndsWith(Environment.NewLine)) {
        // output.Remove(output.Length - Environment.NewLine.Length, Environment.NewLine.Length);
        // }

        return output.ToString().TrimEnd();
    }
}


//
//
// foreach (var garage in Garages) {
//     foreach (var vehicle in garage) {
//         if (vehicle.LicencePlate != licensePlate) continue;
//                 
//         garage.Remove(vehicle);
//         return true;
//     }
// }
//
// return false; 
//