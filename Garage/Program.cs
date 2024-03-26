using Garage.Entity.Factory;
using Garage.Entity.Vehicles;
using Garage.Services.FactoryProvider;
using Garage.Services.GarageHandler;
using Garage.Services.UI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Garage;

public class Program {
    static void Main(string[] args) {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services => {
                services.AddSingleton<IUI, UI>();
                services.AddSingleton<IGarageHandler<IVehicle>, GarageHandler<IVehicle>>();
                services.AddSingleton<IVehicleFactoryProvider>(sp => {
                    var garageHandler = sp.GetRequiredService<IGarageHandler<IVehicle>>();
                    
                    var carFactory = new CarFactory(garageHandler);
                    var airplaneFactory = new AirplaneFactory(garageHandler);
                    var motorcycleFactory = new MotorcycleFactory(garageHandler);
                    var busFactory = new BusFactory(garageHandler);
                    var boatFactory = new BoatFactory(garageHandler);
                    
                    return new VehicleFactoryProvider(new List<(string, IVehicleFactory)> {
                        ("Car", carFactory),
                        ("Airplane", airplaneFactory),
                        ("Motorcycle", motorcycleFactory),
                        ("Bus", busFactory),
                        ("Boat", boatFactory)
                    });
                });
            })
            .UseConsoleLifetime()
            .Build();

        var ui = host.Services.GetService<IUI>();
        ui.MainMenu();
    }
}