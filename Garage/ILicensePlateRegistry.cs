namespace Garage;

public interface ILicensePlateRegistry {
    bool IsUnique(string licensePlate);
    void RegisterPlate(string licensePlate);
    void UnregisterPlate(string licensePlate);
}