using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace Garage.Entity.Vehicles;

public abstract class Vehicle : IVehicle {
    [Required]
    [RegularExpression("^[A-Z]{3}[0-9]{3}$", ErrorMessage = "Licence plate must have the format ABC123")]
    public string? LicencePlate { get; set;  }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Number of wheels must be at least 1.")]
    public int? NumWheels { get; set; }

    [Required]
    public VehicleColor? Color { get; set; }

    [Required]
    public int? TopSpeed { get; set; }
        
    public override string ToString() {
        StringBuilder stringBuilder = new StringBuilder();

        stringBuilder.Append($"{GetType().Name} ");
        stringBuilder.Append(LicencePlate is not null ? $"LicencePlate={LicencePlate} " : "");
        stringBuilder.Append(NumWheels is not null ? $"NumWheels={NumWheels} " : "");
        stringBuilder.Append(Color is not null ? $"NumWheels={Color} " : "");
        stringBuilder.Append(TopSpeed is not null ? $"NumWheels={TopSpeed} " : "");

        return stringBuilder.ToString();
    }

    // public IVehicle CreateVehicle() {
        
    // }
}