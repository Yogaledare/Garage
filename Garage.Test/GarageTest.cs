using Garage.Entity.Vehicles;
using Garage.Entity;

namespace Garage.Test;

public class GarageTest {
    /// <summary>
    /// Tests if a vehicle can be added to the garage when it is not full.
    /// </summary>
    [Fact]
    public void Add_ShouldAddVehicle_WhenNotFull() {
        var garage = new Garage<IVehicle>(capacity: 1);
        var car = new Car {LicencePlate = "ABC123"};

        var result = garage.Add(car);

        Assert.True(result);
        Assert.Equal(1, garage.NumItems);
    }

    /// <summary>
    /// Tests if adding a vehicle to a full garage fails as expected.
    /// </summary>
    [Fact]
    public void Add_ShouldFail_WhenFull() {
        var garage = new Garage<IVehicle>(capacity: 1);
        garage.Add(new Car {LicencePlate = "ABC123"});

        var result = garage.Add(new Car {LicencePlate = "DEF456"});

        Assert.False(result);
        Assert.True(garage.IsFull);
    }

    /// <summary>
    /// Tests if the garage can handle multiple types of vehicles.
    /// </summary>
    [Fact]
    public void Add_ShouldHandleMultipleVehicleTypes() {
        var garage = new Garage<IVehicle>(capacity: 3);
        var car = new Car {LicencePlate = "CAR123"};
        var motorcycle = new Motorcycle {LicencePlate = "MOTO123"};

        garage.Add(car);
        garage.Add(motorcycle);

        Assert.Equal(2, garage.NumItems);
        Assert.Contains(car, garage);
        Assert.Contains(motorcycle, garage);
    }

    /// <summary>
    /// Tests if a new garage is initially empty.
    /// </summary>
    [Fact]
    public void NewGarage_ShouldBeEmpty() {
        var garage = new Garage<IVehicle>(capacity: 5);

        Assert.Equal(0, garage.NumItems);
    }

    /// <summary>
    /// Tests if adding the same vehicle twice is handled correctly and fails as expected.
    /// </summary>
    [Fact]
    public void Add_ShouldFail_WhenAddingSameVehicleTwice() {
        var garage = new Garage<IVehicle>(capacity: 2);
        var car = new Car {LicencePlate = "ABC123"};

        garage.Add(car);
        var result = garage.Add(car); // Attempt to add the same car again

        Assert.False(result);
        Assert.Equal(1, garage.NumItems); // Ensure item count did not increase
    }

    /// <summary>
    /// Tests if removing a vehicle decreases the item count in the garage.
    /// </summary>
    [Fact]
    public void Remove_ShouldDecreaseItemCount_WhenVehicleExists() {
        var garage = new Garage<IVehicle>(capacity: 2);
        var car = new Car {LicencePlate = "ABC123"};
        garage.Add(car);

        var result = garage.Remove(car);

        Assert.True(result);
        Assert.Equal(0, garage.NumItems);
    }

    /// <summary>
    /// Tests if attempting to remove a vehicle that does not exist in the garage fails as expected.
    /// </summary>
    [Fact]
    public void Remove_ShouldFail_WhenVehicleDoesNotExist() {
        var garage = new Garage<IVehicle>(capacity: 1);
        garage.Add(new Car {LicencePlate = "ABC123"});

        var result = garage.Remove(new Car {LicencePlate = "DEF456"});

        Assert.False(result);
    }

    /// <summary>
    /// Tests if the garage is not considered full after a vehicle is removed.
    /// </summary>
    [Fact]
    public void Garage_ShouldNotBeFull_AfterVehicleRemoval() {
        var garage = new Garage<IVehicle>(capacity: 1);
        var car = new Car {LicencePlate = "ABC123"};
        garage.Add(car);

        garage.Remove(car);

        Assert.False(garage.IsFull);
        Assert.Equal(0, garage.NumItems);
    }

    /// <summary>
    /// Tests if the short description of the garage returns the correct format.
    /// </summary>
    [Fact]
    public void ShortDescription_ShouldReturnCorrectFormat() {
        var garage = new Garage<IVehicle>(capacity: 5);
        garage.Add(new Car {LicencePlate = "ABC123"});

        string expected = "Garage with capacity = 5, #stored = 1";
        string actual = garage.ShortDescription();

        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Tests if the ToString method includes both the short description and vehicle details.
    /// </summary>
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

    /// <summary>
    /// Tests if a garage is correctly identified as full when its capacity is reached.
    /// </summary>
    [Fact]
    public void IsFull_ShouldBeTrue_WhenCapacityReached() {
        var garage = new Garage<IVehicle>(capacity: 1);
        garage.Add(new Car {LicencePlate = "ABC123"});

        Assert.True(garage.IsFull);
    }

    /// <summary>
    /// Tests if the GetEnumerator method returns all added vehicles.
    /// </summary>
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

    /// <summary>
    /// Tests if calling GetEnumerator on an empty garage does not throw any exceptions.
    /// </summary>
    [Fact]
    public void GetEnumerator_OnEmptyGarage_ShouldNotThrow() {
        var garage = new Garage<IVehicle>(capacity: 1);

        var exception = Record.Exception(() => garage.ToList());

        Assert.Null(exception);
    }
}