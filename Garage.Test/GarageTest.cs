using Garage.Entity.Vehicles;
using Garage.Entity;

namespace Garage.Test;

public class GarageTest {
    [Fact]
    public void Add_ShouldAddVehicle_WhenNotFull() {
        var garage = new Garage<IVehicle>(capacity: 1);
        var car = new Car {LicencePlate = "ABC123"};

        var result = garage.Add(car);

        Assert.True(result);
        Assert.Equal(1, garage.NumItems);
    }

    
    

    [Fact]
    public void Add_ShouldFail_WhenFull() {
        var garage = new Garage<IVehicle>(capacity: 1);
        garage.Add(new Car {LicencePlate = "ABC123"});

        var result = garage.Add(new Car {LicencePlate = "DEF456"});

        Assert.False(result);
        Assert.True(garage.IsFull);
    }
    
    
    
    [Fact]
    public void Add_ShouldHandleMultipleVehicleTypes() {
        var garage = new Garage<IVehicle>(capacity: 3);
        var car = new Car { LicencePlate = "CAR123" };
        var motorcycle = new Motorcycle { LicencePlate = "MOTO123" };

        garage.Add(car);
        garage.Add(motorcycle);

        Assert.Equal(2, garage.NumItems);
        Assert.Contains(car, garage);
        Assert.Contains(motorcycle, garage);
    }
    
    
    
    [Fact]
    public void NewGarage_ShouldBeEmpty() {
        var garage = new Garage<IVehicle>(capacity: 5);

        Assert.Equal(0, garage.NumItems);
    }
    
    

    
    [Fact]
    public void Add_ShouldFail_WhenAddingSameVehicleTwice() {
        var garage = new Garage<IVehicle>(capacity: 2);
        var car = new Car { LicencePlate = "ABC123" };

        garage.Add(car);
        var result = garage.Add(car); // Attempt to add the same car again

        Assert.False(result);
        Assert.Equal(1, garage.NumItems); // Ensure item count did not increase
    }
    
    

    [Fact]
    public void Remove_ShouldDecreaseItemCount_WhenVehicleExists() {
        var garage = new Garage<IVehicle>(capacity: 2);
        var car = new Car {LicencePlate = "ABC123"};
        garage.Add(car);

        var result = garage.Remove(car);

        Assert.True(result);
        Assert.Equal(0, garage.NumItems);
    }


    [Fact]
    public void Remove_ShouldFail_WhenVehicleDoesNotExist() {
        var garage = new Garage<IVehicle>(capacity: 1);
        garage.Add(new Car {LicencePlate = "ABC123"});

        var result = garage.Remove(new Car {LicencePlate = "DEF456"});

        Assert.False(result);
    }


    
    [Fact]
    public void Garage_ShouldNotBeFull_AfterVehicleRemoval() {
        var garage = new Garage<IVehicle>(capacity: 1);
        var car = new Car { LicencePlate = "ABC123" };
        garage.Add(car);

        garage.Remove(car);

        Assert.False(garage.IsFull);
        Assert.Equal(0, garage.NumItems);
    }

    
    
    [Fact]
    public void ShortDescription_ShouldReturnCorrectFormat() {
        var garage = new Garage<IVehicle>(capacity: 5);
        garage.Add(new Car {LicencePlate = "ABC123"});

        string expected = "Garage with capacity = 5, #stored = 1";
        string actual = garage.ShortDescription();

        Assert.Equal(expected, actual);
    }


    [Fact]
    public void ToString_ShouldIncludeShortDescriptionAndVehicleDetails() {
        var garage = new Garage<IVehicle>(capacity: 2);
        var car = new Car {LicencePlate = "ABC123", NumWheels = 4, Color = VehicleColor.Blue, TopSpeed = 150};

        garage.Add(car);

        string expectedStart = "Garage with capacity = 2, #stored = 1";
        string expectedVehicle = $"Car LicencePlate=ABC123 NumWheels=4 Color=Blue TopSpeed=150";

        string actual = garage.ToString();

        Assert.StartsWith(expectedStart, actual);
        Assert.Contains(expectedVehicle, actual);
    }


    [Fact]
    public void IsFull_ShouldBeTrue_WhenCapacityReached() {
        var garage = new Garage<IVehicle>(capacity: 1);
        garage.Add(new Car {LicencePlate = "ABC123"});

        Assert.True(garage.IsFull);
    }


    [Fact]
    public void GetEnumerator_ShouldReturnAllAddedVehicles() {
        var garage = new Garage<IVehicle>(capacity: 2);
        var car1 = new Car {LicencePlate = "ABC123"};
        var car2 = new Car {LicencePlate = "DEF456"};
        garage.Add(car1);
        garage.Add(car2);

        var vehicles = garage.ToList();

        Assert.Contains(car1, vehicles);
        Assert.Contains(car2, vehicles);
        Assert.Equal(2, vehicles.Count);
    }
    
    
    [Fact]
    public void GetEnumerator_OnEmptyGarage_ShouldNotThrow() {
        var garage = new Garage<IVehicle>(capacity: 1);

        var exception = Record.Exception(() => garage.ToList());

        Assert.Null(exception);
    }
    
    
    
}