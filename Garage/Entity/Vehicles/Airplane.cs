namespace Garage.Entity.Vehicles;

public class Airplane : Vehicle {
    public int NumberOfEngines { get; set; }

    public override string ToString() {
        return base.ToString() + $", NumberOfEngines={NumberOfEngines}";
    }
}