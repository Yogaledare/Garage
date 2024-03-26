using Garage.Entity.Vehicles;
using Garage.Services.GarageHandler;
using LanguageExt.Common;
using static Garage.Services.Input.InputValidator;

namespace Garage.Services.Input;

public static class InputRetriever {
    public static T RetrieveInput<T>(string prompt, Func<string?, Result<T>> validator) {
        T? output = default;

        while (true) {
            Console.Write(prompt);
            var input = Console.ReadLine();
            var result = validator(input);

            bool shouldBreak = result.Match(
                Succ: validatedSentence => {
                    output = validatedSentence;
                    return true;
                },
                Fail: ex => {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            );

            if (shouldBreak) {
                break;
            }
        }

        if (output == null) throw new InvalidOperationException("Parsing failed");
        return output;
    }


    public static T SelectFromMenu<T>(IEnumerable<(string, T)> options) {
        Console.WriteLine($"Select {typeof(T).Name}");

        var index = 0;
        var enumerable = options as (string Description, T Value)[] ?? options.ToArray();
        foreach (var option in enumerable) {
            Console.WriteLine($"{index++}. {option.Description}");
        }

        var input = RetrieveInput("Choice: ", s => ValidateNumberBounded(s, 0, index - 1));

        return enumerable.ElementAt(input).Value;
    }


    public static T SelectFromMenu<T>(IEnumerable<T> options) {
        Console.WriteLine($"Select {typeof(T).Name}");
        var describedObjects = options.Select(o => (o?.ToString() ?? "N/A", o));
        return SelectFromMenu<T>(describedObjects);
    }


    public static T SelectFromEnum<T>() where T : struct, Enum {
        // Generate the menu options from the enum values
        var optionsDescription = Enum.GetValues(typeof(T))
            .Cast<T>()
            .Select(e => (Description: e.ToString(), Value: e))
            .ToList();

        // Use the existing SelectFromMenu method to let the user choose
        return SelectFromMenu(optionsDescription);
    }

    public static void EnterLicensePlateSearch<T>(T vehicle) where T : IVehicle {
        vehicle.LicencePlate = RetrieveInput("License plate: ", ValidateLicensePlateSearch);
    }

    public static void EnterLicensePlate<T>(T vehicle, IGarageHandler<IVehicle> garageHandler) where T : IVehicle {
        vehicle.LicencePlate = RetrieveInput("License plate: ", s => ValidateLicensePlate(s, garageHandler));
    }

    public static void EnterNumWheels<T>(T vehicle) where T : IVehicle {
        vehicle.NumWheels = RetrieveInput("NumWheels: ", s => ValidateNumberBounded(s, 0, 8));
    }

    public static void EnterVehicleColor<T>(T vehicle) where T : IVehicle {
        vehicle.Color = SelectFromEnum<VehicleColor>();
    }

    public static void EnterTopSpeed<T>(T vehicle) where T : IVehicle {
        vehicle.TopSpeed = RetrieveInput("TopSpeed: ", s => ValidateDoubleBounded(s, 0, 450));
    }
}


//
// public static T SelectFromMenu<T>(IEnumerable<T> options) {
//     Console.WriteLine($"Select {typeof(T).Name}");
//
//     var index = 0;
//     var enumerable = options as (string Description, T Value)[] ?? options.ToArray();
//     foreach (var option in enumerable) {
//         Console.WriteLine($"{index++}. {option.Description}");
//     }
//
//     var input = RetrieveInput("Choice: ", s => ValidateNumberBounded(s, 0, index - 1));
//
//     return enumerable.ElementAt(input).Value;
// }
//