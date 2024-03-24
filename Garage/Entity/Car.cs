using System.ComponentModel.DataAnnotations;

namespace Garage.Entity;

public class Car : Vehicle {
    [Required]
    [Range(1, 5, ErrorMessage = "Number of doors must be within [1, 5]")]
    public int NumDoors { get; set; }
        
    public override string ToString()
    {
        return base.ToString() +  $", NumDoors={NumDoors}";
    }
}