using System.ComponentModel.DataAnnotations;

namespace Garage.Entity.Vehicles;

public abstract class Vehicle : IVehicle {
    [Required]
    [RegularExpression("^[A-Z]{3}[0-9]{3}$", ErrorMessage = "Licence plate must have the format ABC123")]
    public string? LicencePlate { get; set;  }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Number of wheels must be at least 1.")]
    public int NumWheels { get; set; }

    [Required]
    public VehicleColor Color { get; set; }

    [Required]
    public double TopSpeed { get; set; }
        
    public override string ToString()
    {
        return $"LicencePlate={LicencePlate}, NumWheels={NumWheels}, Color={Color}, TopSpeed={TopSpeed}";
    }

    // public IVehicle CreateVehicle() {
        
    // }
}