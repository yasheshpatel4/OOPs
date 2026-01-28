using System;
using System.Collections.Generic;

interface IRentable
{
    bool IsAvailable { get; }
    void Rent();
    void Return();
}

interface IDiscountPolicy
{
    double CalculateDiscount(double baseCost, int rentalHours);
}

abstract class Vehicle : IRentable
{
    public string RegistrationNumber { get; }
    public string Brand { get; }
    public string Model { get; }
    public double CostPerHour { get; }
    public bool IsAvailable { get; private set; } = true;

    protected Vehicle(string regNo, string brand, string model, double costPerHour)
    {
        RegistrationNumber = regNo;
        Brand = brand;
        Model = model;
        CostPerHour = costPerHour;
    }

    public void Rent()
    {
        if (!IsAvailable)
            throw new VehicleUnavailableException("Vehicle not available");
        IsAvailable = false;
    }

    public void Return()
    {
        IsAvailable = true;
    }
}

class Car : Vehicle
{
    public string FuelType { get; }

    public Car(string regNo, string brand, string model, double costPerHour, string fuelType)
        : base(regNo, brand, model, costPerHour)
    {
        FuelType = fuelType;
    }
}

class Bike : Vehicle
{
    public int EngineCC { get; }

    public Bike(string regNo, string brand, string model, double costPerHour, int engineCC)
        : base(regNo, brand, model, costPerHour)
    {
        EngineCC = engineCC;
    }
}

class Truck : Vehicle
{
    public int LoadCapacity { get; }

    public Truck(string regNo, string brand, string model, double costPerHour, int loadCapacity)
        : base(regNo, brand, model, costPerHour)
    {
        LoadCapacity = loadCapacity;
    }
}

class Customer
{
    public string Name { get; }
    public string Contact { get; }

    public Customer(string name, string contact)
    {
        Name = name;
        Contact = contact;
    }
}

class LongDurationDiscount : IDiscountPolicy
{
    public double CalculateDiscount(double baseCost, int rentalHours)
    {
        return rentalHours > 24 ? baseCost * 0.10 : 0;
    }
}

class FestiveSeasonDiscount : IDiscountPolicy
{
    public double CalculateDiscount(double baseCost, int rentalHours)
    {
        return baseCost * 0.05;
    }
}

class RentalTransaction
{
    private static int counter = 1;

    public int RentalId { get; }
    public Vehicle Vehicle { get; }
    public Customer Customer { get; }
    public int RentalHours { get; }
    public double BaseCost { get; }
    public double FinalCost { get; private set; }

    public RentalTransaction(Vehicle vehicle, Customer customer, int hours)
    {
        RentalId = counter++;
        Vehicle = vehicle;
        Customer = customer;
        RentalHours = hours;
        BaseCost = vehicle.CostPerHour * hours;
    }

    public void ApplyDiscounts(List<IDiscountPolicy> discountPolicies)
    {
        double totalDiscount = 0;

        for (int i = 0; i < discountPolicies.Count; i++)
            totalDiscount += discountPolicies[i].CalculateDiscount(BaseCost, RentalHours);

        FinalCost = BaseCost - totalDiscount;
    }
}

class RentalService
{
    private readonly List<RentalTransaction> rentals = new();

    public RentalTransaction BookVehicle(
        Vehicle vehicle,
        Customer customer,
        int rentalHours,
        List<IDiscountPolicy> discounts)
    {
        vehicle.Rent();

        var rental = new RentalTransaction(vehicle, customer, rentalHours);
        rental.ApplyDiscounts(discounts);
        rentals.Add(rental);

        Console.WriteLine($"Rental ID: {rental.RentalId}");
        Console.WriteLine($"Vehicle: {vehicle.Brand} {vehicle.Model}");
        Console.WriteLine($"Customer: {customer.Name}");
        Console.WriteLine($"Duration: {rentalHours} hours");
        Console.WriteLine($"Base Cost: ₹{rental.BaseCost}");
        Console.WriteLine($"Final Cost: ₹{rental.FinalCost}");
        Console.WriteLine("--------------------------------");

        return rental;
    }

    public void ReturnVehicle(RentalTransaction rental)
    {
        rental.Vehicle.Return();
    }

    public void PrintRentalReport()
    {
        Console.WriteLine("Rental Report:");
        for (int i = 0; i < rentals.Count; i++)
        {
            Console.WriteLine($"{rentals[i].Vehicle.Model} | {rentals[i].Customer.Name} | ₹{rentals[i].FinalCost}");
        }
    }
}

class VehicleUnavailableException : Exception
{
    public VehicleUnavailableException(string message) : base(message) { }
}

class Program
{
    static void Main()
    {
        var vehicles = new List<Vehicle>
        {
            new Car("C1", "Tata", "Nexon", 200, "Petrol"),
            new Car("C2", "Tesla", "Model3", 300, "Electric"),
            new Bike("B1", "Yamaha", "R15", 100, 155),
            new Bike("B2", "Honda", "Shine", 80, 125),
            new Truck("T1", "Tata", "LoadPro", 400, 1000),
            new Truck("T2", "Ashok", "HeavyX", 450, 1200)
        };

        var customer1 = new Customer("Rahul", "9999999999");

        var discountPolicies = new List<IDiscountPolicy>
        {
            new LongDurationDiscount(),
            new FestiveSeasonDiscount()
        };

        var rentalService = new RentalService();

        try
        {
            var rental = rentalService.BookVehicle(
                vehicles[0],
                customer1,
                30,
                discountPolicies);

            rentalService.ReturnVehicle(rental);
            rentalService.PrintRentalReport();
        }
        catch (VehicleUnavailableException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
