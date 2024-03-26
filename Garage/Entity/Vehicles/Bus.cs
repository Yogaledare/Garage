namespace Garage.Entity.Vehicles;

public class Bus : Vehicle {
    public int NumberOfSeats { get; set; }

    public override string ToString() {
        return base.ToString() + $"NumberOfSeats={NumberOfSeats}";
    }
}