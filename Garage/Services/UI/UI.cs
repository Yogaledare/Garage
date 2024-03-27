using System.Text;
using Garage.Entity.Vehicles;
using Garage.Services.FactoryProvider;
using Garage.Services.GarageHandler;
using static Garage.Services.Input.InputValidator;
using static Garage.Services.Input.InputRetriever;

namespace Garage.Services.UI;

public class UI : IUI {
    private readonly IGarageHandler<IVehicle> _garageHandler;
    private readonly IVehicleFactoryProvider _factoryProvider;
    private readonly List<(string Description, Action Method)> _menuOptions;
    public event Action OnExitRequested;


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


    private void ExitProgram() {
        OnExitRequested?.Invoke();
    }


    private void AddGarage() {
        var capacity = RetrieveInput("Capacity: ", ValidateNumber);

        _garageHandler.CreateGarage(capacity);
        Console.WriteLine($"New garage with {capacity} created. Garages: ");

        foreach (var garage in _garageHandler.Garages) {
            Console.WriteLine(garage);
        }
    }


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


    private void RemoveVehicle() {
        var licensePlate = RetrieveInput("Licence plate: ", ValidateLicensePlateSearch);
        var result = _garageHandler.RemoveVehicle(licensePlate);
        var message = result ? $"Removed vehicle with licence plate {licensePlate}" : "Could not find vehicle";
        Console.WriteLine(message);
    }


    private void ListParkedVehicles() {
        var output = _garageHandler.ListContents();
        Console.WriteLine(output);
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


//
//
// private string BuildQueryStringDisplay(IVehicle searchObject) {
//     StringBuilder stringBuilder = new StringBuilder();
//     stringBuilder.Append(searchObject.LicencePlate is not null ? $"LicencePlate={searchObject.LicencePlate} " : "");
//     stringBuilder.Append(searchObject.NumWheels is not null ? $"NumWheels={searchObject.NumWheels} " : "");
//     stringBuilder.Append(searchObject.Color is not null ? $"NumWheels={searchObject.Color} " : "");
//     stringBuilder.Append(searchObject.TopSpeed is not null ? $"NumWheels={searchObject.TopSpeed} " : "");
//     var output = stringBuilder.ToString().Trim();
//     return output != "" ? output : "(Empty)";
// }