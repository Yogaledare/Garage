namespace Garage.Entity.Vehicles;

public class Boat : Vehicle {
    public double Length { get; set; }

    public override string ToString() {
        return base.ToString() + $"Length={Length}m";
    }
}