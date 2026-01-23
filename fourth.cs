using System;
using System.Collections.Generic;

class OutOfStockException : Exception
{
    public OutOfStockException(string msg) : base(msg) { }
}

class Product
{
    public int pid;
    public string pName;
    public string pCat;
    public int pPrice;
    public int pStock;
}

class Electronics : Product { }
class Clothing : Product { }

class Customer
{
    public string cName;
    public string cMail;
}

class Admin
{
    public string aName;
}

class OrderItem
{
    public Product prod;
    public int qty;
    public int total;
}

class Order
{
    public List<OrderItem> items = new List<OrderItem>();
    public string status = "Pending";
    public int sum;
    public int disc;
    public int finalSum;
}

class Shop
{
    public void AddItem(Order ord, Product p, int q)
    {
        if (q > p.pStock)
            throw new OutOfStockException("Available Quantity: " + p.pStock);

        OrderItem oi = new OrderItem();
        oi.prod = p;
        oi.qty = q;
        oi.total = p.pPrice * q;

        ord.items.Add(oi);
    }

    public void Discount(Order o, int per)
    {
        o.disc = o.sum * per / 100;
    }

    public void Discount(Order o, string code)
    {
        if (code == "SAVE20")
            o.disc = o.sum * 20 / 100;
    }
}

class Program
{
    static void Main()
    {
        Electronics lap = new Electronics();
        lap.pid = 1;
        lap.pName = "Laptop";
        lap.pCat = "Electronics";
        lap.pPrice = 50000;
        lap.pStock = 5;

        Clothing tee = new Clothing();
        tee.pid = 2;
        tee.pName = "T-Shirt";
        tee.pCat = "Clothing";
        tee.pPrice = 1000;
        tee.pStock = 10;

        Customer cust = new Customer();
        cust.cName = "yashesh";
        cust.cMail = "yashesh@gmail.com";

        Admin adm = new Admin();
        adm.aName = "Admin1";

        Order ord = new Order();
        Shop sh = new Shop();

        try
        {
            sh.AddItem(ord, lap, 1);
            sh.AddItem(ord, tee, 2);
        }
        catch (OutOfStockException e)
        {
            Console.WriteLine(e.Message);
        }

        foreach (OrderItem it in ord.items)
            ord.sum += it.total;

        sh.Discount(ord, "SAVE20");

        ord.finalSum = ord.sum - ord.disc;
        ord.status = "Delivered";

        Console.WriteLine("Invoice");
        foreach (OrderItem it in ord.items)
            Console.WriteLine(it.prod.pName + " " + it.qty + " " + it.total);

        Console.WriteLine("Total: " + ord.sum);
        Console.WriteLine("Discount: " + ord.disc);
        Console.WriteLine("Final Amount: " + ord.finalSum);
        Console.WriteLine("Order Status: " + ord.status);

        lap.pStock -= 1;
        tee.pStock -= 2;

        Console.WriteLine("Remaining Stock");
        Console.WriteLine(lap.pName + " " + lap.pStock);
        Console.WriteLine(tee.pName + " " + tee.pStock);
    }
}
