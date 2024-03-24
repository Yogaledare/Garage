using Garage.Entity;

namespace Garage.Services;

public interface IGarageHandler<T> where T : IVehicle {
    List<Garage<T>> Garages { get; }
    bool AddVehicle(T vehicle, Garage<T> garage);
    bool DoesLicencePlateExist(string? licencePlate);
    void CreateGarage(int capacity);
    public string ListContents();
}