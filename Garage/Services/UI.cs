using System.Reflection;
using Garage.Entity;
using LanguageExt.ClassInstances.Const;
using static Garage.Services.InputValidator;

namespace Garage.Services;

public class UI(IGarageHandler<IVehicle> garageHandler, ITypeConversionService converter) : IUI {
    public void MainMenu() {
        while (true) {
            ShowMenu();
            var shouldContinue = Navigate();
            if (!shouldContinue) {
                break; 
            }
        }
    }


    public bool Navigate() {
        var input = RetrieveInput("Choice: ", s => ValidateNumberBounded(s, 0, 2));

        switch (input) {
            case 1:
                AddVehicle();
                break;
            case 2:
                AddGarage();
                break;
            case 0:
                return false;  
        }

        return true; 
    }

    
    private void AddGarage() {
        var capacity = RetrieveInput("Capacity: ", ValidateNumber); 
        
        garageHandler.CreateGarage(capacity);
        Console.WriteLine($"New garage with {capacity} created. Garages: ");

        foreach (var garage in garageHandler.Garages) {
            Console.WriteLine(garage);
        }
    }


    public static void ShowMenu() {
        Console.WriteLine("Main menu: ");
        Console.WriteLine("1. Add a vehicle");
        Console.WriteLine("2. Add a Garage");
        Console.WriteLine("0. Exit");
    }


    public void AddVehicle() {

        var vehicleInstance = CreateVehicle(); 
        Console.WriteLine("Created Vehicle Details:");
        Console.WriteLine(vehicleInstance.ToString());

        
        
        
        var garages = garageHandler.Garages; 
        
        for (var i = 0; i < garages.Count; i++) {
            Console.WriteLine($"{i}: {garages[i]}");
        }


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
    }


    public object CreateVehicle() {
        Console.WriteLine("Choose a vehicle");
        var vehicleTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(IVehicle).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .ToList();

        for (var i = 0; i < vehicleTypes.Count; i++) {
            Console.WriteLine($"{i}: {vehicleTypes[i].Name}");
        }

        var numTypes = vehicleTypes.Count;
        var index = RetrieveInput(
            "Choice: ",
            i => ValidateNumberBounded(i, 0, numTypes - 1));

        var vehicleType = vehicleTypes[index];
        var vehicleInstance = Activator.CreateInstance(vehicleType);

        foreach (var propertyInfo in vehicleType.GetProperties()) {
            var entry = RetrieveInput(
                $"{propertyInfo.Name}: ",
                s => ValidateProperty(s, propertyInfo, converter, garageHandler));
            propertyInfo.SetValue(vehicleInstance, entry);
        }

        return vehicleInstance; 
    }
}