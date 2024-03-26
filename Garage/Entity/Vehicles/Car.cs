namespace Garage.Entity.Vehicles;

public class Car : Vehicle {
    public int NumDoors { get; set; }
        
    public override string ToString()
    {
        return base.ToString() +  $"NumDoors={NumDoors}";
    }
}