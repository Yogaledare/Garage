﻿using Garage.Entity;
using Garage.Entity.Vehicles;
using LanguageExt.Common;

namespace Garage.Services.GarageHandler;

public interface IGarageHandler<T> where T : IVehicle {
    List<Garage<T>> Garages { get; }
    Result<T> AddVehicle(T vehicle, Garage<T> garage);
    bool RemoveVehicle(string licensePlate);
    bool DoesLicencePlateExist(string? licencePlate);
    void CreateGarage(int capacity);
    public string ListContents();
    Result<(Garage<T>, T)> FindVehicle(string licencePlate);
    Dictionary<Type, int> CountVehicleTypes(HashSet<Type> types);
    List<T> QueryVehicles(T searchCriteria);
}