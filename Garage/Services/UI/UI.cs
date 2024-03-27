using System.Text;
using Garage.Entity.Vehicles;
using Garage.Services.FactoryProvider;
using Garage.Services.GarageHandler;
using static Garage.Services.Input.InputValidator;
using static Garage.Services.Input.InputRetriever;

namespace Garage.Services.UI;


/// <summary>
/// Handles user interaction and provides a console-based user interface for managing garages and vehicles.
/// </summary>
public class UI : IUI {
    private readonly IGarageHandler<IVehicle> _garageHandler;
    private readonly IVehicleFactoryProvider _factoryProvider;
    private readonly List<(string Description, Action Method)> _menuOptions;
    
    /// <summary>
    /// Occurs when the user requests to exit the program.
    /// </summary>
    public event Action OnExitRequested;

    /// <summary>
    /// Initializes a new instance of the UI class.
    /// </summary>
    /// <param name="garageHandler">The garage handler to manage vehicle storage.</param>
    /// <param name="factoryProvider">The factory provider to create vehicle instances.</param>
    public UI(IGarageHandler<IVehicle> garageHandler, IVehicleFactoryProvider factoryProvider) {
        _garageHandler = garageHandler;
        _factoryProvider = factoryProvider;
        _menuOptions = [
            ("Exit", ExitProgram),
            ("Add garage", AddGarage),
            ("Add vehicle", AddVehicle),
            ("Remove vehicle", RemoveVehicle),
            ("Query on properties", QueryOnProperties),
            ("List parked vehicles (& garages)", ListParkedVehicles),
            ("List vehicle types", ListVehicleTypes),
            ("Pre-populate garages", PrePopulate),
            ("Search for vehicle", SearchForVehicle),
        ];
    }

    /// <summary>
    /// Displays the main menu and handles user input for various actions.
    /// </summary>
    public void MainMenu() {
        var continueRunning = true;
        OnExitRequested += () => continueRunning = false;

        while (continueRunning) {
            Console.WriteLine();
            var chosenMethod = SelectFromMenu(_menuOptions, "Action");
            Console.WriteLine();
            chosenMethod.Invoke();
        }
    }

    /// <summary>
    /// Triggers the OnExitRequested event to signal the application to exit.
    /// </summary>
    private void ExitProgram() {
        OnExitRequested?.Invoke();
    }

    /// <summary>
    /// Adds a new garage by prompting the user for capacity and then creates the garage using the garage handler.
    /// Outputs the result to the console.
    /// </summary>
    private void AddGarage() {
        var capacity = RetrieveInput("Capacity: ", ValidateNumber);

        _garageHandler.CreateGarage(capacity);
        Console.WriteLine($"New garage with {capacity} created. Garages: ");

        foreach (var garage in _garageHandler.Garages) {
            Console.WriteLine(garage);
        }
    }

    /// <summary>
    /// Guides the user through the process of adding a vehicle to a garage.
    /// This includes selecting a vehicle type, creating the vehicle, and choosing a garage to add the vehicle to.
    /// Outputs the result to the console.
    /// </summary>
    private void AddVehicle() {
        var garages = _garageHandler.Garages;

        if (garages.Count <= 0) {
            Console.WriteLine("Create a garage first!");
            return;
        }

        var vehicleFactory = SelectFromMenu(_factoryProvider.GetAvailableFactories(), "Vehicle");
        var vehicleInstance = vehicleFactory.CreateVehicle();
        Console.WriteLine("Vehicle Details:");
        Console.WriteLine(vehicleInstance.ToString());

        var garagesDescribed = garages.Select(g => (g.ShortDescription(), g));
        var garageSelected = SelectFromMenu(garagesDescribed, "Garage");
        var result = _garageHandler.AddVehicle(vehicleInstance, garageSelected);

        var output = result.Match(
            Succ: v => $"Added {vehicleInstance.GetType().Name} to {garageSelected.ShortDescription()}",
            Fail: ex => ex.Message
        );

        Console.WriteLine(output);
    }

    /// <summary>
    /// Prompts the user for a license plate and attempts to remove the vehicle with that plate from the garages.
    /// Outputs the result to the console.
    /// </summary>
    private void RemoveVehicle() {
        var licensePlate = RetrieveInput("Licence plate: ", ValidateLicensePlateSearch);
        var result = _garageHandler.RemoveVehicle(licensePlate);
        var message = result ? $"Removed vehicle with licence plate {licensePlate}" : "Could not find vehicle";
        Console.WriteLine(message);
    }

    /// <summary>
    /// Lists all available vehicle types that can be created and parked in garages.
    /// This is determined by the vehicle factory provider and outputs the types to the console.
    /// </summary>
    private void ListParkedVehicles() {
        var output = _garageHandler.ListContents();
        Console.WriteLine(output);
    }

    /// <summary>
    /// Lists all available vehicle types that can be created and parked in garages.
    /// This is determined by the vehicle factory provider and outputs the types to the console.
    /// </summary>
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

    /// <summary>
    /// Prompts the user to enter a license plate number to search for a vehicle across all garages.
    /// Searches for the vehicle using the garage handler and outputs the search result.
    /// If the vehicle is found, details of the vehicle and its location are displayed.
    /// Otherwise, a message indicating the vehicle could not be found is shown.
    /// </summary>
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

    /// <summary>
    /// Initiates a query based on vehicle properties specified by the user.
    /// Allows the user to build a query interactively and then performs a search based on the constructed query.
    /// Outputs the matching vehicles to the console.
    /// </summary>
    private void QueryOnProperties() {
        IVehicle searchObject = new QueryVehicle();

        var continueLooping = true;
        var mainOptions = new List<(string Description, Action)> {
            ("Add Condition to Query", () => AddQueryCondition(searchObject)),
            ("Run Query", () => {
                continueLooping = false;
            }),
        };

        while (continueLooping) {
            // var queryString = searchObject);
            Console.WriteLine($"Query string: {searchObject.ToString()}");
            var choice = SelectFromMenu(mainOptions, "Action");
            choice();
        }

        var searchResult = _garageHandler.QueryVehicles(searchObject);
        Console.WriteLine("Matches: ");

        if (searchResult.Count == 0) {
            Console.WriteLine("(None)");
        }
        else {
            foreach (var vehicle in searchResult) {
                Console.WriteLine(vehicle);
            }
        }
    }

    /// <summary>
    /// Prompts the user to add conditions to a vehicle query, such as filtering by license plate, number of wheels, color, or top speed.
    /// </summary>
    /// <param name="searchObject">The vehicle object used to accumulate search criteria.</param>
    private void AddQueryCondition(IVehicle searchObject) {
        IEnumerable<(string, Action<IVehicle>)> searchOptions = [
            ("LicencePlate", EnterLicensePlateSearch),
            ("NumWheels", EnterNumWheels),
            ("VehicleColor", EnterVehicleColor),
            ("TopSpeed", EnterTopSpeed),
            ("Go Back", vehicle => {
            })
        ];

        var action = SelectFromMenu(searchOptions, "Property");

        action(searchObject);
    }

    /// <summary>
    /// Pre-populates the system with a set of garages and vehicles for demonstration or testing purposes.
    /// Outputs details of the added entities to the console.
    /// </summary>
    private void PrePopulate() {
        _garageHandler.CreateGarage(4);
        _garageHandler.CreateGarage(10);
        _garageHandler.CreateGarage(3);
        _garageHandler.CreateGarage(8);

        List<(IVehicle vehicle, int Destination)> vehicles = [
            (new Car {LicencePlate = "CAR123", NumWheels = 4, Color = VehicleColor.Red, TopSpeed = 180, NumDoors = 4}, 0),
            (new Car {LicencePlate = "CAR456", NumWheels = 4, Color = VehicleColor.Red, TopSpeed = 190, NumDoors = 2}, 1),
            (new Car {LicencePlate = "ZXI987", NumWheels = 4, Color = VehicleColor.Blue, TopSpeed = 180, NumDoors = 2}, 2),
            (new Car {LicencePlate = "CAR789", NumWheels = 4, Color = VehicleColor.White, TopSpeed = 200, NumDoors = 4}, 3),

            // Motorcycles 
            (new Motorcycle {LicencePlate = "MCY123", NumWheels = 2, Color = VehicleColor.Black, TopSpeed = 200, CylinderVolume = 500},
                1),
            (new Motorcycle {LicencePlate = "MCY456", NumWheels = 2, Color = VehicleColor.Red, TopSpeed = 210, CylinderVolume = 750},
                2),
            (new Motorcycle {LicencePlate = "BIK999", NumWheels = 2, Color = VehicleColor.Black, TopSpeed = 220, CylinderVolume = 600},
                0),

            // Airplanes
            (new Airplane {LicencePlate = "AIR123", NumWheels = 6, Color = VehicleColor.White, TopSpeed = 800, NumberOfEngines = 2},
                2),
            (new Airplane {LicencePlate = "FLY456", NumWheels = 10, Color = VehicleColor.White, TopSpeed = 900, NumberOfEngines = 4},
                3),

            // Buses 
            (new Bus {LicencePlate = "BUS123", NumWheels = 6, Color = VehicleColor.Blue, TopSpeed = 100, NumberOfSeats = 50}, 3),
            (new Bus {LicencePlate = "BUS456", NumWheels = 8, Color = VehicleColor.Red, TopSpeed = 90, NumberOfSeats = 45}, 1),
            // Boats
            (new Boat {LicencePlate = "BOA123", NumWheels = 0, Color = VehicleColor.White, TopSpeed = 85, Length = 20}, 0),
            (new Boat {LicencePlate = "SAL789", NumWheels = 0, Color = VehicleColor.Blue, TopSpeed = 55, Length = 25}, 3),
        ];

        foreach (var (vehicle, destination) in vehicles) {
            var garage = _garageHandler.Garages[destination % _garageHandler.Garages.Count];
            var result = _garageHandler.AddVehicle(vehicle, garage);
            var output = result.Match(
                Succ: v => $"Added {vehicle.GetType().Name} to {garage.ShortDescription()}",
                Fail: ex => ex.Message
            );
            Console.WriteLine(output);
        }
    }
}