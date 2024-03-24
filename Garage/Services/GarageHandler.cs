using System.Text;
using Garage.Entity;

namespace Garage.Services;

public class GarageHandler<T>() : IGarageHandler<T> where T : IVehicle {
    public List<Garage<T>> Garages { get; } = [];

    public bool AddVehicle(T vehicle, Garage<T> garage) {
        if (DoesLicencePlateExist(vehicle.LicencePlate)) return false;

        if (garage.IsFull) return false;

        garage.Add(vehicle);
        return true;
    }

    public bool DoesLicencePlateExist(string? licencePlate) {
        return Garages.SelectMany(garage => garage)
            .Any(vehicle => string.Equals(vehicle.LicencePlate, licencePlate, StringComparison.OrdinalIgnoreCase));
    }

    public void CreateGarage(int capacity) {
        Garages.Add(new Garage<T>(capacity));
    }

    
    
    
    
    

    public string ListContents() {
        var output = new StringBuilder();

        output.AppendLine("Garages: ");

        foreach (var garage in Garages) {
            output.AppendLine($"Capacity = {garage.Capacity}, #stored = {garage.NumItems}");

            foreach (var item in garage) {
                output.AppendLine(item.ToString());
            }
        }

        return output.ToString(); 
    }
}