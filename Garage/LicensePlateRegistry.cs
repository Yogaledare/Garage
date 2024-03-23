namespace Garage;

public class LicensePlateRegistry : ILicensePlateRegistry {
    private readonly HashSet<string> _plates = new HashSet<string>();

    public bool IsUnique(string licensePlate) {
        return !_plates.Contains(licensePlate);
    }

    public void RegisterPlate(string licensePlate) {
        _plates.Add(licensePlate);
    }

    public void UnregisterPlate(string licensePlate) {
        _plates.Remove(licensePlate);
    }
}