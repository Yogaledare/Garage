using Garage.Entity;
using Garage.Entity.Vehicles;
using LanguageExt.Common;

namespace Garage.Services.GarageHandler;

public interface IGarageHandler<T> where T : IVehicle {
    List<Garage<T>> Garages { get; }
    bool AddVehicle(T vehicle, Garage<T> garage);
    bool DoesLicencePlateExist(string? licencePlate);
    void CreateGarage(int capacity);
    public string ListContents();
    Result<(Garage<T>, T)> FindVehicle(string licencePlate);
}