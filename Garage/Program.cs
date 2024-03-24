using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Garage.Validation;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Garage;

public partial class Program {
    static void Main(string[] args) {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services => {
                // services.AddSingleton<ILicensePlateRegistry, LicensePlateRegistry>();
                services.AddSingleton<IGarageHandler<IVehicle>, GarageHandler<IVehicle>>();
                services.AddSingleton<ITypeConversionService, TypeConversionService>();
            })
            .UseConsoleLifetime()
            .Build();


        AddVehicle(host.Services.GetService<ITypeConversionService>());

        // RetrieveInteger(0, 5); 
    }


    private static void AddVehicle(ITypeConversionService converter) {
        // var i = RetrieveInput("hello", i => ValidateNumberBounded(i, 0, 5));
        // Console.WriteLine(i);

        var vehicleTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(IVehicle).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .ToList();

        for (var i = 0; i < vehicleTypes.Count; i++) {
            Console.WriteLine($"{i}: {vehicleTypes[i].Name}");
        }


        var numTypes = vehicleTypes.Count;
        var index = RetrieveInput("", i => ValidateNumberBounded(i, 0, numTypes - 1));

        var vehicle = vehicleTypes[index];
        Console.WriteLine(vehicle.Name);

        foreach (var propertyInfo in vehicle.GetProperties()) {
            var propertyAttributes = propertyInfo.GetCustomAttributes<ValidationAttribute>();

            foreach (var propertyAttribute in propertyAttributes) {
                Console.WriteLine(propertyAttribute);
                Console.WriteLine(propertyAttribute.ErrorMessage);
                Console.WriteLine();
            }
        }

        foreach (var propertyInfo in vehicle.GetProperties()) {
            // error here: 
            var entry = RetrieveInput($"{propertyInfo.Name}: ", s => ValidateProperty(s, propertyInfo, converter));
        }
    }


    public abstract class Vehicle : IVehicle {
        [Required]
        [RegularExpression("^[A-Z]{3}[0-9]{3}$", ErrorMessage = "Licence plate must have the format ABC123")]
        public string? LicencePlate { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Number of wheels must be at least 1.")]
        public int NumWheels { get; set; }

        [Required]
        public VehicleColor Color { get; set; }

        [Required]
        public double TopSpeed { get; set; }
    }


    public class Car : Vehicle {
        [Range(1, 5, ErrorMessage = "Number of doors must be within [1, 5]")]
        public int NumDoors { get; init; }
    }


    public interface IGarageHandler<T> where T : IVehicle {
        bool AddVehicle(T vehicle, Garage<T> garage);
        bool DoesLicencePlateExist(string? licencePlate);
        void CreateGarage(int capacity);
    }

    public class GarageHandler<T>(ILicensePlateRegistry licensePlateRegistry) : IGarageHandler<T> where T : IVehicle {
        private List<Garage<T>> _garages = [];

        public bool AddVehicle(T vehicle, Garage<T> garage) {
            if (DoesLicencePlateExist(vehicle.LicencePlate)) return false;

            if (garage.IsFull) return false;

            garage.Add(vehicle);
            return true;
        }

        public bool DoesLicencePlateExist(string? licencePlate) {
            return _garages.SelectMany(garage => garage)
                .Any(vehicle => string.Equals(vehicle.LicencePlate, licencePlate, StringComparison.OrdinalIgnoreCase));
        }

        public void CreateGarage(int capacity) {
            _garages.Add(new Garage<T>(capacity));
        }
    }


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

    public static Result<object> ValidateProperty(string? input, PropertyInfo propertyInfo, ITypeConversionService converter) {
        if (string.IsNullOrEmpty(input)) {
            var error = new ValidationException("Error: null or empty input");
            return new Result<object>(error);
        }

        var conversionResult = converter.TryConvert(input, propertyInfo);

        return conversionResult.Match(
            Succ: convertedValue => {
                var validationAttributes = propertyInfo.GetCustomAttributes<ValidationAttribute>(true);
                foreach (var attribute in validationAttributes) {
                    if (!attribute.IsValid(convertedValue)) {
                        var error = new ValidationException(attribute.ErrorMessage);
                        return new Result<object>(error);
                    }
                }

                return new Result<object>(convertedValue);
            },
            Fail: e => new Result<object>(e)
        );
    }


    public static Result<int> ValidateNumberBounded(string? input, int min, int max) {
        var normalNumberValidationResult = ValidateNumber(input);

        return normalNumberValidationResult.Match(
            Succ: number => {
                if (number < min || number > max) {
                    var error = new ValidationException($"Must be within [{min}, {max}] (inclusive)");
                    return new Result<int>(error);
                }

                return number;
            },
            Fail: _ => normalNumberValidationResult);
    }


    public static Result<int> ValidateNumber(string? input) {
        if (string.IsNullOrEmpty(input)) {
            var error = new ValidationException("Error: null or empty input");
            return new Result<int>(error);
        }

        var tokens = input.Split(' ');

        if (tokens.Length > 1) {
            var error = new ValidationException("Error: too many inputs");
            return new Result<int>(error);
        }

        if (!int.TryParse(tokens[0], out int number)) {
            var error = new ValidationException("Error: cannot parse integer");
            return new Result<int>(error);
        }

        return number;
    }
}

public interface ITypeConversionService {
    Result<object> TryConvert(string input, PropertyInfo property);
}

public class TypeConversionService : ITypeConversionService {
    private readonly Dictionary<Type, Func<string, object?>> _converters;


    public TypeConversionService() {
        _converters = new Dictionary<Type, Func<string, object?>> {
            {typeof(int), s => int.TryParse(s, out int i) ? i : null},
            {typeof(double), s => double.TryParse(s, out double d) ? d : null},
            {typeof(VehicleColor), s => Enum.TryParse(s, true, out VehicleColor vc) ? vc : null},
            {typeof(string), s => s},
        };
    }


    public Result<object> TryConvert(string input, PropertyInfo property) {
        if (!_converters.TryGetValue(property.PropertyType, out var converter)) {
            var error = new ValidationException($"No converter available for type: {property.PropertyType.Name}.");
            return new Result<object>(error);
        }

        var convertedValue = converter(input);

        if (convertedValue is null) {
            var error = new ValidationException($"Conversion failed for input: {input}.");
            return new Result<object>(error);
        }

        return new Result<object>(convertedValue);
    }
}



// var parseResult = Enum.TryParse(s, true, out VehicleColor vc);
// Console.WriteLine($"parseresult was {parseResult} for {s} av vc was {vc}");
// if (!parseResult) return null;
// return parseResult;
// return Enum.TryParse(s, true, out VehicleColor vc) ? vc : null;   
// }
// },


// if (!converter.TryConvert(input, propertyInfo, out var convertedValue)) {
//     var error = new ValidationException($"Failed to convert input to {propertyInfo.PropertyType.Name}.");
//     return new Result<object>(error);
// }
//
//
// var validationAttributes = propertyInfo.GetCustomAttributes<ValidationAttribute>(true);
//
//
// foreach (var attribute in validationAttributes) {
//     if (!attribute.IsValid(convertedValue)) {
//         var error = new ValidationException(attribute.ErrorMessage);
//         return new Result<object>(error);
//     }
// }
//
// return new Result<object>(convertedValue);


//
//
//
// public bool TryConvert(string input, PropertyInfo property, out object? result) {
//     result = null;
//
//         
//     if (_converters.TryGetValue(property.PropertyType, out var converter)) {
//         var convertedValue = converter(input);
//         if (convertedValue != null) {
//             result = convertedValue;
//             return true;
//         }
//     }
//
//     return false;
// }
//

//
// public abstract class Vehicle : IVehicle {
//     // protected Vehicle(string licencePlate, int numWheels, VehicleColor color, double topSpeed) {
//     //     LicencePlate = licencePlate;
//     //     NumWheels = numWheels;
//     //     Color = color;
//     //     TopSpeed = topSpeed;
//     // }
//
//     [Required]
//     [RegularExpression("^[A-Z]{3}[0-9]{3}$", ErrorMessage = "Licence plate must have the format ABC123")]
//     public string? LicencePlate { get; set; }
//
//     [Range(1, int.MaxValue, ErrorMessage = "Number of wheels must be at least 1.")]
//     public int NumWheels { get; set; }
//
//     public VehicleColor Color { get; set; }
//
//     public double TopSpeed { get; set; }
// }
//
//
// public class Car : Vehicle {
//     // public Car(string licencePlate, VehicleColor color, double topSpeed, int numDoors) : base(
//     //     licencePlate, 4, color, topSpeed) {
//     //     NumDoors = numDoors;
//     // }
//
//     // [Range(4, int.MaxValue, ErrorMessage = "A car must have at least 4 wheels.")]
//     // public override int NumWheels { get; set; }
//
//     [Range(1, 5, ErrorMessage = "Number of doors must be within [1, 5]")]
//     public int NumDoors { get; init; }
// }
//


// public static void AddVehicle() {
//     var vehicleTypes = Assembly.GetExecutingAssembly()
//         .GetTypes()
//         .Where(t => typeof(IVehicle).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
//         .ToList();
//
//     for (var i = 0; i < vehicleTypes.Count; i++) {
//         Console.WriteLine($"{i}: {vehicleTypes[i].Name}");
//     }
//
//     var selectedIndex = RetrieveInteger(0, vehicleTypes.Count - 1); 
//     var selectedVehicleType = vehicleTypes[selectedIndex];
//
//     var constructor = selectedVehicleType.GetConstructors().First();
//     var parameterValues = RetrieveParameterValues(constructor);
//
//     var vehicle = (IVehicle) constructor.Invoke(parameterValues);
//     Console.WriteLine(vehicle);
// }


// public class NumericStringInput(string value) {
//     public string Value { get; } = value;
// }

//
//
//
// public static void MainTests() {
//     var garage = new Garage<Car>(4);
//
//     // var cars = new Garage<Car>(10) {
//     //     new Car("ABC123", VehicleColor.Black, 100.0, 2),
//     //     new Car("ABC456", VehicleColor.Black, 100.0, 10),
//     //     new Car("ABc123", VehicleColor.Black, 100.0, 10),
//     //     new Car("ABc123", VehicleColor.Black, 100.0, 2),
//     // };
//
//
//     foreach (var car in cars) {
//         var validationResults = new List<ValidationResult>();
//         var validationContext = new ValidationContext(car);
//         bool isValid = Validator.TryValidateObject(car, validationContext, validationResults, true);
//
//         if (!isValid) {
//             foreach (var validationResult in validationResults) {
//                 Console.WriteLine(validationResult.ErrorMessage);
//             }
//         }
//         else {
//             Console.WriteLine("Validation passed.");
//         }
//     }
// }


// public static T HandleEnumSelection<T>() where T : Enum {
//     var enumValues = Enum.GetValues(typeof(T))
//         .Cast<T>()
//         .ToList();
//
//     for (int i = 0; i < enumValues.Count; i++) {
//         Console.WriteLine($"{i}: {enumValues[i]}");
//     }
//
//     var enumIndex = RetrieveInteger(0, enumValues.Count - 1);
//
//     return enumValues[enumIndex];
// }


// public static int RetrieveInteger(int min, int max) {
//     Console.WriteLine($"Make a selection:");
//     while (true) {
//         var input = Console.ReadLine();
//         var validator = new NumericStringValidator(min, max);
//         var validationResult = validator.Validate(input);
//
//         if (validationResult.IsValid) {
//             return int.Parse(input!);
//         }
//
//         foreach (var error in validationResult.Errors) {
//             Console.WriteLine(error);
//         }
//     }
// }


// public static Result<int> ValidateNumberBounded(string? input, int min, int max) {
//     if (string.IsNullOrEmpty(input)) {
//         var error = new ValidationException("Must be non-null");
//         return new Result<int>(error);
//     }
//
//     var tokens = input.Split(' ');
//
//     if (tokens.Length > 1) {
//         var error = new ValidationException("Can only have a single number");
//         return new Result<int>(error);
//     }
//
//     if (!int.TryParse(tokens[0], out int number)) {
//         var error = new ValidationException("Must be parsable to an int");
//         return new Result<int>(error);
//     }
//
//
//     
//     return number;
// }


//
// public static void AddVehicle() {
//     var vehicleTypes = Assembly.GetExecutingAssembly()
//         .GetTypes()
//         .Where(t => typeof(IVehicle).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
//         .ToList();
//
//     for (var i = 0; i < vehicleTypes.Count; i++) {
//         Console.WriteLine($"{i}: {vehicleTypes[i].Name}");
//     }
//
//     var selectedIndex = RetrieveInteger(0, vehicleTypes.Count - 1); 
//     var selectedVehicleType = vehicleTypes[selectedIndex];
//
//     var constructor = selectedVehicleType.GetConstructors().First();
//     var parameterValues = RetrieveParameterValues(constructor);
//
//     var vehicle = (IVehicle) constructor.Invoke(parameterValues);
//     Console.WriteLine(vehicle);
// }
//
//
// public static void CreateVehicleRoutine() {
//         
//         
// }
//
//     
// private static object[] RetrieveParameterValues(ConstructorInfo constructor) {
//     var parameters = constructor.GetParameters();
//     var parameterValues = new object[parameters.Length];
//
//     for (var i = 0; i < parameters.Length; i++) {
//         var parameterType = parameters[i].ParameterType;
//         Console.WriteLine($"Enter {parameters[i].Name} ({parameters[i].ParameterType.Name}):");
//             
//         if (parameterType.IsEnum) {
//             var enumValues = Enum.GetValues(parameterType);
//                 
//             var j = 0;
//             foreach (var value in enumValues) {
//                 Console.WriteLine($"{j++}: {value}");
//             }
//             var enumIndex = RetrieveInteger(0, enumValues.Length - 1); 
//             parameterValues[i] = enumValues.GetValue(enumIndex);
//         }
//
//         else {
//             var input = Console.ReadLine();
//             parameterValues[i] = Convert.ChangeType(input, parameters[i].ParameterType);
//         }
//     }
//
//     return parameterValues;
// }
//
//
//


// if parameters[i].parametertype is enum: 
// get all possible values for this enum
// present as with the vehicle types
// let user input value

// else do the normal thing


// int.Parse(Console.ReadLine());


// if (enumIndex >= 0 && enumIndex < enumValues.Length) {
//     parameterValues[i] = enumValues.GetValue(enumIndex);
// }
// else {
//     Console.WriteLine("Invalid selection. Please select a valid index.");
//     i--; // Decrement the counter to retry this iteration
//     continue;
// }


//
// public static void AddVehicle() {
//     var vehicleTypes = Assembly.GetExecutingAssembly()
//         .GetTypes()
//         .Where(t => typeof(IVehicle).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
//         .ToList();
//
//     for (var i = 0; i < vehicleTypes.Count; i++) {
//         Console.WriteLine($"{i}: {vehicleTypes[i].Name}");
//     }
//
//     // Console.WriteLine("Select a vehicle type by index:");
//     var selectedIndex = RetrieveInteger(0, vehicleTypes.Count - 1); 
//     // var selectedIndex = int.Parse(Console.ReadLine());
//     var selectedVehicleType = vehicleTypes[selectedIndex];
//
//     var constructor = selectedVehicleType.GetConstructors().First();
//     var parameters = constructor.GetParameters();
//     var parameterValues = new object[parameters.Length];
//
//     for (var i = 0; i < parameters.Length; i++) {
//         var parameterType = parameters[i].ParameterType;
//         Console.WriteLine($"Enter {parameters[i].Name} ({parameters[i].ParameterType.Name}):");
//         if (parameterType.IsEnum) {
//             MethodInfo handleEnumSelectionMethod = typeof(Program).GetMethod(nameof(HandleEnumSelection), BindingFlags.Public | BindingFlags.Static);
//
//             // how to call the handleenumselection??
//             HandleEnumSelection<parameterType>(); 
//
//         }
//
//         else {
//             var input = Console.ReadLine();
//             // Convert input to the appropriate type. Additional error handling may be required for type conversion.
//             parameterValues[i] = Convert.ChangeType(input, parameters[i].ParameterType);
//         }
//         // if parameters[i].parametertype is enum: 
//         // get all possible values for this enum
//         // present as with the vehicle types
//         // let user input value
//
//         // else do the normal thing
//     }
//
//     var vehicle = (IVehicle) constructor.Invoke(parameterValues);
//
//     Console.WriteLine(vehicle);
// }
//
//
// public static T HandleEnumSelection<T>() where T : Enum {
//     var enumValues = Enum.GetValues(typeof(T))
//             .Cast<T>()
//             .ToList();
//
//     for (int i = 0; i < enumValues.Count; i++) {
//         Console.WriteLine($"{i}: {enumValues[i]}");
//     }
//     
//     var enumIndex = RetrieveInteger(0, enumValues.Count - 1);
//
//     return enumValues[enumIndex]; 
// }
//


// reflection - get all classes that implements IVehicle

// present a list of these classes and let the user pick one by giving an index


// ------------------------


// reflection - get all constrictor arguments for the class that was picked

// go through all constructor arguments 

//    ask for all inputs

// make an vehicle object calling the constructor of the chosen class

// validate the vehicle object with fluent validation

// the validation process also needs to know if the garage handler has this license plate already-
// so somehow it needs a reference to the garage handler, either as an argument or via DI

// read the results and check what went wrong

// print errors and what parameter they correspond to

// for all parameters that were wrong, ask again for input

// create a new object or update fields of the object that failed validation

// validate again. loop like this until there are no validation errors. 

// add the vehicle with no validation errors to the garagehandler, using a DI reference to this class


// var car = new Car("ABC123", VehicleColor.Black, 100.0, 2);
// var car2 = new Car("ABC123", VehicleColor.Black, 100.0, 10);
//
//
// // Prepare for validation
// var validationResults = new List<ValidationResult>();
// var validationContext = new ValidationContext(car, serviceProvider: null, items: null);
//


// Perform validation

// Check validation result


// var car1 = new Car("ABC123", 4, VehicleColor.Black, 160, 5);
// garage.Add(car1);
// var car2 = new Car("ABC123", 4, VehicleColor.Black, 160, 6);
// garage.Add(car2);
// // garage.Add(car2);
// var car3 = new Car("ABC123", 4, VehicleColor.Black, 160, 7);
// garage.Add(car3);
// garage.Remove(car2);
// var r1 = garage.GetAll();

// foreach (var c in r1) {
// Console.WriteLine(c);
// }
//
// var validator = new LicensePlateValidator();
// var test1 = "ABC123";
// var test2 = "acb123";
// var res1 = validator.Validate(test1);
// var res2 = validator.Validate(test2);
//
//
// if (!res1.IsValid) {
//     foreach (var failure in res1.Errors) {
//         Console.WriteLine($"Validation error: {failure.ErrorMessage}");
//     }
// }
// else {
//     Console.WriteLine("License plate is valid.");
// }
//
//
// if (!res2.IsValid) {
//     foreach (var failure in res2.Errors) {
//         Console.WriteLine($"Validation error: {failure.ErrorMessage}");
//     }
// }
// else {
//     Console.WriteLine("License plate is valid.");
// }