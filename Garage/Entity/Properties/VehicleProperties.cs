using Garage.Entity.Vehicles;
using Garage.Services.Input;
using LanguageExt.Common;

namespace Garage.Entity.Properties;

public class VehicleProperties {


    public Property<string> LicencePlate { get; private set; }
    public Property<int> NumWheels { get; private set;  }
    public Property<VehicleColor> Color { get; private set;  }
    public Property<double> TopSpeed { get; private set;  }


    public VehicleProperties() {
        LicencePlate = new Property<string>(
            "LicensePlate: ",
            s => InputValidator.ValidateLicensePlate(s)
            );
        NumWheels = new Property<int>(
            "Number of Wheels: ",
            s => InputValidator.ValidateNumberBounded(s, 1, 8)
        ); 
        Color = new Property<VehicleColor>(
            "Color: ", 
            s => InputValidator.
            
            )

    }







}


//
// vehicle.LicencePlate = InputRetriever.RetrieveInput(
//     "LicensePlate: ",
//     s => InputValidator.ValidateLicensePlate(s, _garageHandler)
// );
// vehicle.NumWheels = InputRetriever.RetrieveInput(
//     "numWheels: ",
//     s => InputValidator.ValidateNumberBounded(s, 0, 4)
// );
// vehicle.Color = InputRetriever.SelectFromEnum<VehicleColor>();
// vehicle.TopSpeed = InputRetriever.RetrieveInput(
//     "TopSpeed: ",
//     s => InputValidator.ValidateDoubleBounded(s, 0, 450)
// );
//
