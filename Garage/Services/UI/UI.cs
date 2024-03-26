using System.ComponentModel;
using System.Reflection;
using System.Text;
using Garage.Entity;
using Garage.Entity.Factory;
using Garage.Entity.Vehicles;
using Garage.Services.Conversion;
using Garage.Services.FactoryProvider;
using Garage.Services.GarageHandler;
using Garage.Services.Input;
using LanguageExt.ClassInstances.Const;
using static Garage.Services.Input.InputValidator;
using static Garage.Services.Input.InputRetriever;

namespace Garage.Services.UI;

public class UI : IUI {
    private readonly IGarageHandler<IVehicle> _garageHandler;
    private readonly IVehicleFactoryProvider _factoryProvider;
    private readonly List<(string Description, Action Method)> _menuOptions;

    // public List<(string, IVehicleFactory)> VehicleFactoryOptions { get; set; }
    public event Action OnExitRequested;


    public UI(IGarageHandler<IVehicle> garageHandler, IVehicleFactoryProvider factoryProvider) {
        _garageHandler = garageHandler;
        _factoryProvider = factoryProvider;
        _menuOptions = [
            ("Exit", ExitProgram),
            ("Add garage", AddGarage),
            ("Add vehicle", AddVehicle),
            ("Remove vehicle", RemoveVehicle),
            ("Search for vehicle", SearchForVehicle),
            // ("Query on properties", QueryOnProperties),
            ("List parked vehicles (& garages)", ListParkedVehicles),
            ("List vehicle types", ListVehicleTypes),
            ("Pre-populate garages", PrePopulate),
        ];
    }


    private void QueryOnProperties() {

        IVehicle searchObject = new Car();

        IEnumerable<(string, Action<IVehicle>)> searchOptions = [
            ("LicencePlate", EnterLicensePlateSearch),
            ("NumWheels", EnterNumWheels),
            ("VehicleColor", EnterVehicleColor),
            ("TopSpeed", EnterTopSpeed),
        ]; 

        
        




    }
    
    
    

    private void PrePopulate() {
        _garageHandler.CreateGarage(4);
        _garageHandler.CreateGarage(10);
        _garageHandler.CreateGarage(3);
        _garageHandler.CreateGarage(8);

        List<(IVehicle vehicle, int Destination)> vehicles = [
            (new Car {
                LicencePlate = "ABC123",
                NumWheels = 4,
                Color = VehicleColor.Blue,
                TopSpeed = 150,
                NumDoors = 4
            }, 0),
            (new Car {
                LicencePlate = "DEF456",
                NumWheels = 4,
                Color = VehicleColor.Black,
                TopSpeed = 160,
                NumDoors = 5
            }, 2),
            (new Car {
                LicencePlate = "GHI789",
                NumWheels = 4,
                Color = VehicleColor.White,
                TopSpeed = 170,
                NumDoors = 3
            }, 0),
        ];

        foreach (var (vehicle, destination) in vehicles) {
            var garage = _garageHandler.Garages[destination]; 
            var result = _garageHandler.AddVehicle(vehicle, garage);
            var output = result.Match(
                Succ: v => $"Added {vehicle.GetType().Name} to {garage.ShortDescription()}",
                Fail: ex => ex.Message
            );
            Console.WriteLine(output);
        }
    }

    private void RemoveVehicle() {
        var licensePlate = RetrieveInput("Licence plate: ", ValidateLicensePlateSearch);
        var result = _garageHandler.RemoveVehicle(licensePlate);
        var message = result ? $"Removed vehicle with licence plate {licensePlate}" : "Could not find vehicle";
        Console.WriteLine(message);
    }

    private void ListVehicleTypes() {
        HashSet<Type> types = new HashSet<Type>();

        foreach (var (description, factory) in _factoryProvider.GetAvailableFactories()) {
            types.Add(factory.ProducedVehicleType);
        }

        var response = _garageHandler.CountVehicleTypes(types);

        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("Vehicle types (#):");

        bool isFirst = true;
        foreach (var (type, count) in response) {
            if (!isFirst) {
                stringBuilder.Append(',');
            }

            stringBuilder.Append($" {type.Name} ({count}),");
            isFirst = false;
        }

        Console.WriteLine(stringBuilder.ToString());
    }


    public void MainMenu() {
        var continueRunning = true;
        OnExitRequested += () => continueRunning = false;

        while (continueRunning) {
            Console.WriteLine();
            var chosenMethod = SelectFromMenu(_menuOptions);
            Console.WriteLine();
            chosenMethod.Invoke();
        }
    }


    private void ExitProgram() {
        OnExitRequested?.Invoke();
    }


    private void ListParkedVehicles() {
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
                stringBuilder.Append($"Found in {garage.ShortDescription()}");
                return stringBuilder.ToString();
            },
            Fail: exception => exception.Message
        );

        Console.WriteLine(output);
    }


    private void AddGarage() {
        var capacity = RetrieveInput("Capacity: ", ValidateNumber);

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

        var vehicleFactory = SelectFromMenu(_factoryProvider.GetAvailableFactories());
        var vehicleInstance = vehicleFactory.CreateVehicle();
        Console.WriteLine("Vehicle Details:");
        Console.WriteLine(vehicleInstance.ToString());

        var garagesDescribed = garages.Select(g => (g.ShortDescription(), g));
        var garageSelected = SelectFromMenu(garagesDescribed);
        var result = _garageHandler.AddVehicle(vehicleInstance, garageSelected);

        var output = result.Match(
            Succ: v => $"Added {vehicleInstance.GetType().Name} to {garageSelected.ShortDescription()}",
            Fail: ex => ex.Message
        );

        Console.WriteLine(output);
    }
}




    
// public static void EnterNumWheels<T>(T vehicle) where T : IVehicle {
//     vehicle.NumWheels = RetrieveInput("NumWheels: ", s => ValidateNumberBounded(s, 0, 8));
// }
//
//
// public static void EnterVehicleColor<T>(T vehicle) where T : IVehicle {
//     vehicle.Color = SelectFromEnum<VehicleColor>();
// }
//
// public static void EnterTopSpeed<T>(T vehicle) where T : IVehicle {
//     vehicle.TopSpeed = RetrieveInput("TopSpeed: ", s => ValidateNumberBounded(s, 0, 450)); 
// }


// Action<IVehicle> getLicensePlate = vehicle => vehicle.LicencePlate = RetrieveInput("LicensePlate: ", ValidateLicensePlateSearch);
// Action<IVehicle> getNumWheels = Vehicle => Vehicle.NumWheels = RetrieveInput("NumWheels: ", s => ValidateNumberBounded(s, 0, 8));
// var getColor = () => SelectFromEnum<VehicleColor>();
// var getTopSpeed = () => RetrieveInput("TopSpeed: ", s => ValidateNumberBounded(s, 0, 450)); 
//
//
// List<Action> queryMethods = [
//     InputRetriever.RetrieveInput(
//         "LicensePlate: ",
//         s => InputValidator.ValidateLicensePlateSearch(s)
//     ),
// ]; 


// var query = RetrieveInput("Query: ", s => ValidateQuery(s)); 

    
// InputRetriever.RetrieveInput(
//     "LicensePlate: ",
//     s => InputValidator.ValidateLicensePlate(s, _garageHandler)
// )
    
    
    
    
// vehicle.LicencePlate = InputRetriever.RetrieveInput(
// "LicensePlate: ",
// s => InputValidator.ValidateLicensePlate(s, garageHandler)
// );
// vehicle.NumWheels = InputRetriever.RetrieveInput(
// "numWheels: ",
// s => InputValidator.ValidateNumberBounded(s, 0, 4)
// );
// vehicle.Color = InputRetriever.SelectFromEnum<VehicleColor>();
// vehicle.TopSpeed = InputRetriever.RetrieveInput(
// "TopSpeed: ",
// s => InputValidator.ValidateDoubleBounded(s, 0, 450)
// );





//
//
//
// private void InitializeMenuOptions() {
//
// }
//
//

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