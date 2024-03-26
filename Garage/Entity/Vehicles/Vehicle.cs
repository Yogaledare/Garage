using System.Text;

namespace Garage.Entity.Vehicles;

public abstract class Vehicle : IVehicle {
    public string? LicencePlate { get; set;  }

    public int? NumWheels { get; set; }

    public VehicleColor? Color { get; set; }

    public int? TopSpeed { get; set; }
        
    public override string ToString() {
        StringBuilder stringBuilder = new StringBuilder();

        stringBuilder.Append($"{GetType().Name} ");
        stringBuilder.Append(LicencePlate is not null ? $"LicencePlate={LicencePlate} " : "");
        stringBuilder.Append(NumWheels is not null ? $"NumWheels={NumWheels} " : "");
        stringBuilder.Append(Color is not null ? $"Color={Color} " : "");
        stringBuilder.Append(TopSpeed is not null ? $"TopSpeed={TopSpeed} " : "");

        return stringBuilder.ToString();
    }
}