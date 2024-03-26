namespace Garage.Entity.Vehicles;

public class Motorcycle : Vehicle {
    public int CylinderVolume { get; set; }

    public override string ToString() {
        return base.ToString() + $"CylinderVolume={CylinderVolume}";
    }
}