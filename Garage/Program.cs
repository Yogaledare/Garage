using Garage.Entity;
using Garage.Entity.Factory;
using Garage.Entity.Vehicles;
using Garage.Services;
using Garage.Services.Conversion;
using Garage.Services.FactoryProvider;
using Garage.Services.GarageHandler;
using Garage.Services.UI;
using Garage.Validation;
using LanguageExt;
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
                // services.AddSingleton<IVehicleFactory, CarFactory>();
                services.AddSingleton<IVehicleFactoryProvider>(sp => {
                    var carFactory = new CarFactory(sp.GetRequiredService<IGarageHandler<IVehicle>>());
                    return new VehicleFactoryProvider(new List<(string, IVehicleFactory)>
                    {
                        ("Car", carFactory),
                        // ... (add tuples for other factories)
                    });
                }); 
                services.AddSingleton<IUI, UI>();
                
                
            })
            .UseConsoleLifetime()
            .Build();


        var ui = host.Services.GetService<IUI>();
        ui.MainMenu();
        

        // UI.AddVehicle(host.Services.GetService<ITypeConversionService>());

        // RetrieveInteger(0, 5); 
    }



}
