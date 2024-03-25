using System.Text;
using Garage.Entity;
using Garage.Entity.Vehicles;
using Garage.Services.UI;
using LanguageExt.Common;

namespace Garage.Services.GarageHandler;

public class GarageHandler<T>(IUI ui) : IGarageHandler<T> where T : IVehicle {
    public List<Garage<T>> Garages { get; } = [];

    public bool AddVehicle(T vehicle, Garage<T> garage) {
        if (DoesLicencePlateExist(vehicle.LicencePlate)) return false;

        if (garage.IsFull) return false;

        garage.Add(vehicle);
        return true;
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