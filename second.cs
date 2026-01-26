using System;
using System.Collections.Generic;
using System.Linq;

class VehicleUnavailableException : Exception
{
    public VehicleUnavailableException(string msg) : base(msg) { }
}

class Vehicle
{
    public int id;
    public string regNo;
    public string brand;
    public string model;
    public int costPerHour;
    public bool available = true;
}

class Car : Vehicle
{
    public string fuelType;
}

class Bike : Vehicle
{
    public int engineCapacity;
}

class Truck : Vehicle
{
    public int loadingCapacity;
}

class Customer
{
    public int id;
    public string name;
    public string contact;
}

class Rental
{
    public int rentalId;
    public Vehicle vehicle;
    public Customer customer;
    public int rentalStartHour;
    public int rentalDurationHours;
    public int baseCost;
    public int discount;
    public int finalCost;
}

class RentalSystem
{
    public List<Vehicle> vehicles = new List<Vehicle>();
    public List<Customer> customers = new List<Customer>();
    public List<Rental> rentals = new List<Rental>();
    int rid = 1;

    public static int festiveDiscount = 10; 

    public void Book(string key, Customer c, int hours, bool festive)
    {
        Vehicle v = vehicles.FirstOrDefault(x =>
            (x.brand == key || x.model == key || x.GetType().Name == key)
            && x.available);

        if (v == null)
            throw new VehicleUnavailableException("Vehicle Not Available");

        v.available = false;

        int baseCost = v.costPerHour * hours;
        int disc = 0;

        if (hours > 24)
            disc += baseCost * 20 / 100;

        if (festive)
            disc += baseCost * festiveDiscount / 100;

        int finalCost = baseCost - disc;

        Rental r = new Rental
        {
            rentalId = rid++,
            vehicle = v,
            customer = c,
            rentalStartHour = 0,
            rentalDurationHours = hours,
            baseCost = baseCost,
            discount = disc,
            finalCost = finalCost
        };

        rentals.Add(r);

        Console.WriteLine("\nBooking Successful:");
        Console.WriteLine($"Rental ID: {r.rentalId}");
        Console.WriteLine($"Vehicle: {v.GetType().Name} {v.model}");
        Console.WriteLine($"Customer: {c.name}");
        Console.WriteLine($"Duration (hrs): {hours}");
        Console.WriteLine($"Estimated Cost: {finalCost}");
    }

    public void ShowInvoice(int id)
    {
        Rental r = rentals.First(x => x.rentalId == id);

        Console.WriteLine("\nFinal Invoice:");
        Console.WriteLine($"Base Cost: {r.baseCost}");
        Console.WriteLine($"Discount: {r.discount}");
        Console.WriteLine($"Grand Total: {r.finalCost}");
    }

    public void ShowAvailable()
    {
        Console.WriteLine("\nAvailable Vehicles:");
        foreach (var v in vehicles.Where(x => x.available))
            Console.WriteLine($"{v.GetType().Name} - {v.brand} {v.model}");
    }

    public void RemoveVehicle(int id)
    {
        vehicles.RemoveAll(x => x.id == id);
    }

    public void Report()
    {
        Console.WriteLine("\nRental Report:");
        foreach (var r in rentals)
            Console.WriteLine($"{r.vehicle.model} rented by {r.customer.name}");
    }
}

class Program
{
    static void Main()
    {
        RentalSystem rs = new RentalSystem();
        rs.vehicles.Add(new Car { id = 1, regNo = "C1", brand = "Honda", model = "City", costPerHour = 100, fuelType = "Petrol" });
        rs.vehicles.Add(new Car { id = 2, regNo = "C2", brand = "Tesla", model = "Model3", costPerHour = 150, fuelType = "Electric" });
        rs.vehicles.Add(new Bike { id = 3, regNo = "B1", brand = "Yamaha", model = "R15", costPerHour = 50, engineCapacity = 155 });
        rs.vehicles.Add(new Bike { id = 4, regNo = "B2", brand = "Royal", model = "Classic", costPerHour = 60, engineCapacity = 350 });
        rs.vehicles.Add(new Truck { id = 5, regNo = "T1", brand = "Tata", model = "Ace", costPerHour = 200, loadingCapacity = 1000 });
        rs.vehicles.Add(new Truck { id = 6, regNo = "T2", brand = "Ashok", model = "Ecomet", costPerHour = 300, loadingCapacity = 3000 });


        Customer c1 = new Customer { id = 1, name = "Amit", contact = "9999999999" };
        Customer c2 = new Customer { id = 2, name = "Ravi", contact = "8888888888" };

        rs.customers.Add(c1);
        rs.customers.Add(c2);

        try
        {
            rs.Book("Car", c1, 30, true);
            rs.Book("R15", c2, 10, false);
        }
        catch (VehicleUnavailableException e)
        {
            Console.WriteLine(e.Message);
        }

        rs.ShowInvoice(1);
        rs.ShowAvailable();
        rs.Report();
    }
}
