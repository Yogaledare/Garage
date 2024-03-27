using System.Text;

namespace Garage.Entity.Vehicles;

public class QueryVehicle : Vehicle {
    public override string ToString() {
        StringBuilder stringBuilder = new StringBuilder();

        stringBuilder.Append(LicencePlate is not null ? $"LicencePlate={LicencePlate} " : "");
        stringBuilder.Append(NumWheels is not null ? $"NumWheels={NumWheels} " : "");
        stringBuilder.Append(Color is not null ? $"Color={Color} " : "");
        stringBuilder.Append(TopSpeed is not null ? $"TopSpeed={TopSpeed} " : "");
        var output = stringBuilder.ToString().Trim();
        return output != "" ? output : "(Empty)";
    }
}