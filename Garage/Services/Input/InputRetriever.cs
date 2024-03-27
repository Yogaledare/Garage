using Garage.Entity.Vehicles;
using Garage.Services.GarageHandler;
using LanguageExt.Common;
using static Garage.Services.Input.InputValidator;

namespace Garage.Services.Input;

/// <summary>
/// Provides methods for retrieving and validating user input from the console.
/// </summary>
public static class InputRetriever {
    
    /// <summary>
    /// Retrieves input from the user, validates it using the provided validator function, and repeats the prompt until valid input is obtained.
    /// </summary>
    /// <typeparam name="T">The type of input expected.</typeparam>
    /// <param name="prompt">The message to display to the user.</param>
    /// <param name="validator">A function to validate the user input.</param>
    /// <returns>The validated input converted to the specified type.</returns>
    /// <exception cref="InvalidOperationException">Thrown when input parsing fails.</exception>
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

    /// <summary>
    /// Presents a menu of options to the user and retrieves their selection.
    /// </summary>
    /// <typeparam name="T">The type of the value associated with each menu option.</typeparam>
    /// <param name="options">A collection of options, each with a description and associated value.</param>
    /// <param name="groupName">The name of the group of options being selected from, for display purposes.</param>
    /// <returns>The value associated with the user's menu selection.</returns>
    public static T SelectFromMenu<T>(IEnumerable<(string, T)> options, string groupName) {
        Console.WriteLine($"Select {groupName}");

        var index = 0;
        var enumerable = options as (string Description, T Value)[] ?? options.ToArray();
        foreach (var option in enumerable) {
            Console.WriteLine($"{index++}. {option.Description}");
        }

        var input = RetrieveInput("Choice: ", s => ValidateNumberBounded(s, 0, index - 1));

        return enumerable.ElementAt(input).Value;
    }

    /// <summary>
    /// Presents a menu of options to the user based on the provided collection and retrieves their selection.
    /// </summary>
    /// <typeparam name="T">The type of the options in the collection.</typeparam>
    /// <param name="options">The collection of options from which the user can choose.</param>
    /// <param name="groupName">The name of the group of options being selected from, for display purposes.</param>
    /// <returns>The user's selection from the options.</returns>
    public static T SelectFromMenu<T>(IEnumerable<T> options, string groupName) {
        Console.WriteLine($"Select {typeof(T).Name}");
        var describedObjects = options.Select(o => (o?.ToString() ?? "N/A", o));
        return SelectFromMenu<T>(describedObjects, groupName);
    }

    /// <summary>
    /// Presents a menu of enum values to the user and retrieves their selection.
    /// </summary>
    /// <typeparam name="T">The Enum type from which the user can select a value.</typeparam>
    /// <param name="groupName">The name of the enum being selected from, for display purposes.</param>
    /// <returns>The enum value selected by the user.</returns>
    public static T SelectFromEnum<T>(string groupName) where T : struct, Enum {
        // Generate the menu options from the enum values
        var optionsDescription = Enum.GetValues(typeof(T))
            .Cast<T>()
            .Select(e => (Description: e.ToString(), Value: e))
            .ToList();

        // Use the existing SelectFromMenu method to let the user choose
        return SelectFromMenu(optionsDescription, groupName);
    }

    /// <summary>
    /// Prompts the user to enter a license plate for search purposes and validates the input against the expected format.
    /// </summary>
    /// <typeparam name="T">The type of the vehicle. Must implement the IVehicle interface.</typeparam>
    /// <param name="vehicle">The vehicle instance to assign the license plate to.</param>
    public static void EnterLicensePlateSearch<T>(T vehicle) where T : IVehicle {
        vehicle.LicencePlate = RetrieveInput("License plate: ", ValidateLicensePlateSearch);
    }

    /// <summary>
    /// Prompts the user to enter a license plate for a new or existing vehicle and validates the input for uniqueness and format.
    /// </summary>
    /// <typeparam name="T">The type of the vehicle. Must implement the IVehicle interface.</typeparam>
    /// <param name="vehicle">The vehicle instance to assign the license plate to.</param>
    /// <param name="garageHandler">The garage handler used to check the uniqueness of the license plate.</param>
    public static void EnterLicensePlate<T>(T vehicle, IGarageHandler<IVehicle> garageHandler) where T : IVehicle {
        vehicle.LicencePlate = RetrieveInput("License plate: ", s => ValidateLicensePlate(s, garageHandler));
    }

    /// <summary>
    /// Prompts the user to enter the number of wheels for a vehicle and validates the input against predefined constraints.
    /// </summary>
    /// <typeparam name="T">The type of the vehicle. Must implement the IVehicle interface.</typeparam>
    /// <param name="vehicle">The vehicle instance to assign the number of wheels to.</param>
    public static void EnterNumWheels<T>(T vehicle) where T : IVehicle {
        vehicle.NumWheels = RetrieveInput("NumWheels: ", s => ValidateNumberBounded(s, 0, 8));
    }

    /// <summary>
    /// Prompts the user to select a color for a vehicle from a predefined list of enum values.
    /// </summary>
    /// <typeparam name="T">The type of the vehicle. Must implement the IVehicle interface.</typeparam>
    /// <param name="vehicle">The vehicle instance to assign the color to.</param>
    public static void EnterVehicleColor<T>(T vehicle) where T : IVehicle {
        vehicle.Color = SelectFromEnum<VehicleColor>("Color");
    }

    /// <summary>
    /// Prompts the user to enter the top speed for a vehicle and validates the input against predefined constraints.
    /// </summary>
    /// <typeparam name="T">The type of the vehicle. Must implement the IVehicle interface.</typeparam>
    /// <param name="vehicle">The vehicle instance to assign the top speed to.</param>
    public static void EnterTopSpeed<T>(T vehicle) where T : IVehicle {
        vehicle.TopSpeed = RetrieveInput("TopSpeed: ", s => ValidateNumberBounded(s, 0, 450));
    }
}