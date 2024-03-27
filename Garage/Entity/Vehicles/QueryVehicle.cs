using System.Text;

namespace Garage.Entity.Vehicles;

/// <summary>
/// Represents a vehicle used specifically for querying/filtering other vehicles.
/// </summary>
public class QueryVehicle : Vehicle {
    /// <summary>
    /// Overrides the base ToString to provide a search-criteria-focused representation.
    /// </summary>
    /// <returns>A string containing non-null search parameters or "(Empty)" if all are null.</returns>
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