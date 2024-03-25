using System.Reflection;
using System.Text;
using Garage.Entity;
using Garage.Services.Conversion;
using Garage.Services.GarageHandler;
using Garage.Services.Input;
using static Garage.Services.Input.InputValidator;
using static Garage.Services.Input.InputRetriever;

namespace Garage.Services.UI;

public class UI : IUI {
    private readonly IGarageHandler<IVehicle> _garageHandler;

    // private readonly ITypeConversionService _converter;
    private List<(string Description, Action Method)> _menuOptions;
    private List<(string, IVehicleFactory)> _vehicleFactoryOptions;
    public event Action OnExitRequested;


    public UI(IGarageHandler<IVehicle> garageHandler) {
        _garageHandler = garageHandler;
        // _converter = converter;
        InitializeMenuOptions();
        InitializeTypeOptions();
    }


    private void InitializeTypeOptions() {
        _vehicleFactoryOptions = [
            ("Car", new CarFactory(_garageHandler)),
        ];
    }


    private void InitializeMenuOptions() {
        _menuOptions = [
            ("Add a vehicle", AddVehicle),
            ("Add a garage", AddGarage),
            ("Search for vehicle (by license plate)", SearchForVehicle),
            ("List garages (and all parked vehicles)", ListGarages),
            ("Exit", ExitProgram)
        ];
    }


    public void MainMenu() {
        var continueRunning = true;
        OnExitRequested += () => continueRunning = false;

        while (continueRunning) {
            Console.WriteLine();
            var chosenMethod = SelectFromMenu(_menuOptions);
            chosenMethod.Invoke();
        }
    }


    private void ExitProgram() {
        OnExitRequested?.Invoke();
    }


    private void ListGarages() {
        var output = _garageHandler.ListContents();
        Console.WriteLine(output);
    }

    private void SearchForVehicle() {
        Console.WriteLine("Enter licence plate to search for (format 'ABC123', non-case sensitive)");
        var input = RetrieveInput("Plate number: ", ValidateLicensePlateSearch);
        var result = _garageHandler.FindVehicle(input);

        var output = result.Match(
            Succ: (tuple) => {
                var (garage, vehicle) = tuple;
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Found vehicle!");
                stringBuilder.AppendLine($"Vehicle: {vehicle}");
                stringBuilder.Append($"Found in a garage with capacity = {garage.Capacity} and #stored = {garage.NumItems} ");
                return stringBuilder.ToString();
            },
            Fail: exception => exception.Message
        );

        Console.WriteLine(output);
    }


    private void AddGarage() {
        var capacity = InputRetriever.RetrieveInput("Capacity: ", ValidateNumber);

        _garageHandler.CreateGarage(capacity);
        Console.WriteLine($"New garage with {capacity} created. Garages: ");

        foreach (var garage in _garageHandler.Garages) {
            Console.WriteLine(garage);
        }
    }


    public void AddVehicle() {
        var garages = _garageHandler.Garages;

        if (garages.Count <= 0) {
            Console.WriteLine("Create a garage first!");
            return;
        }

        var vehicleFactory = SelectFromMenu(_vehicleFactoryOptions);
        var vehicleInstance = vehicleFactory.CreateVehicle();
        Console.WriteLine("Created Vehicle Details:");
        Console.WriteLine(vehicleInstance.ToString());

        var garagesDescribed = garages.Select(g => (g.ShortDescription(), g));
        var garageSelected = SelectFromMenu(garagesDescribed);

        _garageHandler.AddVehicle(vehicleInstance, garageSelected);
        Console.WriteLine($"Added {vehicleInstance.GetType().Name} to {garageSelected.ShortDescription()}");
    }
}

public interface IVehicleFactory {
    public IVehicle CreateVehicle();
}

public abstract class VehicleFactory(IGarageHandler<IVehicle> garageHandler) : IVehicleFactory {
    public IVehicle CreateVehicle() {
        var vehicle = CreateSpecificVehicle();
        SetCommonProperties(vehicle);
        return vehicle;
    }

    protected abstract IVehicle CreateSpecificVehicle();

    private void SetCommonProperties(IVehicle vehicle) {
        vehicle.LicencePlate = RetrieveInput("LicensePlate: ", s => ValidateLicensePlate(s, garageHandler));
        vehicle.NumWheels = RetrieveInput("numWheels: ", s => ValidateNumberBounded(s, 0, 4));
        vehicle.Color = SelectFromEnum<VehicleColor>();
        vehicle.TopSpeed = RetrieveInput("TopSpeed: ", s => ValidateDoubleBounded(s, 0, 450));
    }
}

public class CarFactory(IGarageHandler<IVehicle> garageHandler) : VehicleFactory(garageHandler) {
    protected override IVehicle CreateSpecificVehicle() {
        var car = new Car {
            NumDoors = RetrieveInput("NumDoors: ", s => ValidateNumberBounded(s, 0, 5))
        };
        return car;
    }
}


//
// public IVehicle CreateVehicle() {
//     var vehicleFactory = SelectFromMenu(_vehicleFactoryOptions);
//     return vehicleFactory.CreateVehicle();
// }


//
// private static Type ChooseVehicleType() {
//     var vehicleTypes = Assembly.GetExecutingAssembly()
//         .GetTypes()
//         .Where(t => typeof(IVehicle).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
//         .ToList();
//
//     if (vehicleTypes.Count == 0) {
//         throw new InvalidOperationException("No vehicle classes available!");
//     }
//
//     var vehicleTypesDescribed = vehicleTypes.Select(t => (t.Name, t));
//     var vehicleType = SelectFromMenu(vehicleTypesDescribed);
//
//     return vehicleType;
// }
//
//
// private IVehicle BuildVehicleInstance(Type vehicleType) {
//     var vehicleObject = Activator.CreateInstance(vehicleType);
//
//     if (vehicleObject is not IVehicle vehicleInstance) {
//         throw new InvalidOperationException($"The created instance is not an IVehicle: {vehicleType.Name}");
//     }
//
//     foreach (var propertyInfo in vehicleType.GetProperties()) {
//         var entry = RetrieveInput(
//             $"{propertyInfo.Name}: ",
//             s => ValidateProperty(s, propertyInfo, _converter, _garageHandler));
//         propertyInfo.SetValue(vehicleInstance, entry);
//     }
//
//     return vehicleInstance;
// }
//


// Console.WriteLine("Choose a vehicle");

// for (var i = 0; i < vehicleTypes.Count; i++) {
//     Console.WriteLine($"{i}: {vehicleTypes[i].Name}");
// }
//
// var numTypes = vehicleTypes.Count;
// var index = InputRetriever.RetrieveInput(
//     "Choice: ",
//     i => ValidateNumberBounded(i, 0, numTypes - 1));

// var vehicleType = vehicleTypes[index];


// {
// var description = g.ShortDescription();
// return (description, g);
// }); 

// Console.WriteLine("Choose a garage: ");
// for (var i = 0; i < garages.Count; i++) {
// Console.WriteLine($"{i}: {garages[i]}");
// }


// var garageSelected = SelectFromMenu()

// var garageIndex = InputRetriever.RetrieveInput("Choice: ", s => ValidateNumberBounded(s, 0, garages.Count));
// var garageSelected = garages[garageIndex];


//
// private T SelectFromMenu<T>(IEnumerable<(string Description, T Value)> options) {
//     Console.WriteLine($"Select {typeof(T).Name}");
//
//     var index = 0;
//     var enumerable = options as (string Description, T Value)[] ?? options.ToArray();
//     foreach (var option in enumerable) {
//         Console.WriteLine($"{index++}. {option.Description}");
//     }
//
//     var input = InputRetriever.RetrieveInput("Choice: ", s => ValidateNumberBounded(s, 0, index - 1));
//
//     return enumerable.ElementAt(input).Value;
// }


// public static void ShowMenu() {
//     Console.WriteLine("Main menu: ");
//     Console.WriteLine("1. Add a vehicle");
//     Console.WriteLine("2. Add a garage");
//     Console.WriteLine("3. Search for vehicle (by license plate)");
//     Console.WriteLine("4. List garages (and all parked vehicles))");
//     Console.WriteLine("0. Exit");
// }


//
//
//
//
// public bool Navigate() {
//     var input = RetrieveInput("Choice: ", s => ValidateNumberBounded(s, 0, 4));
//     Console.WriteLine();
//
//     switch (input) {
//         case 1:
//             AddVehicle();
//             break;
//         case 2:
//             AddGarage();
//             break;
//         case 3:
//             SearchForVehicle();
//             break;
//         case 4:
//             ListGarages();
//             break;
//         case 0:
//             return false;
//     }
//
//     return true;
// }


//
//
// private T SelectFromCollection<T>(IEnumerable<T> options) {
//     Console.WriteLine($"Choose a {typeof(T).Name}");
//
//     var index = 0;
//     var enumerable = options as T[] ?? options.ToArray();
//     foreach (var option in enumerable) {
//         Console.WriteLine($"{index++}. {option}");
//     }
//
//     var input = RetrieveInput("Choice: ", s => ValidateNumberBounded(s, 0, index - 1));
//
//     return enumerable.ElementAt(input);
// }
//


//     = [
//     ("Add a vehicle", AddVehicle), 
//     
//
//
// ]; 


// private const SortedDictionary<int, (string Description, Action Method)>[] _menuOptions = new SortedDictionary<int, (string, Action)>();

// private SortedDictionary<int, (string Description, Action Method)> _menuOptions = new SortedDictionary<int, (string, Action)>() {
// {1, }
// };


// public UI() {
//     _menuOptions.Add(1, ("Add a vehicle", AddVehicle));
//     _menuOptions.Add(2, ("Add a garage", AddGarage));
//     _menuOptions.Add(3, ("Search for vehicle (by license plate)", SearchForVehicle));
//     _menuOptions.Add(4, ("List garages (and all parked vehicles)", ListGarages));
//     _menuOptions.Add(0, ("Exit", () => { }));
// }


// var contents = garageHandler.ListContents();

// Console.WriteLine(contents);


// foreach (var garage in garageHandler.Garages) {
// Console.WriteLine(garage);
// }


// garageHandler.AddVehicle(vehicleInstance); 

// Print the created object's details
// if (vehicleInstance != null) {

// }
// else {
// Console.WriteLine("Failed to create vehicle instance.");
// }