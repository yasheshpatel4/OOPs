using System;
using System.Collections.Generic;
using System.Linq;

class Book
{
    public int id;
    public string ttl;
    public string cat;
    public bool avl = true;
}

class EBook : Book
{
    public int sz;
}

class PBook : Book
{
    public string loc;
}

class Member
{
    public int id;
    public string nm;
    public int cnt = 0;
}

class Student : Member
{
    public int lim = 3;
    public int days = 15;
    public int fee = 2;
}

class Regular : Member
{
    public int lim = 5;
    public int days = 30;
    public int fee = 3;
}

class Premium : Member
{
    public int lim = 99;
    public int days = 45;
    public int fee = 1;
}

class Tran
{
    public int transactionId;
    public Book book;
    public Member member;
    public int borrowDay;
    public int dueDay;
    public int returnDay;  
}

class Library
{
    public List<Book> bks = new List<Book>();
    public List<Tran> trs = new List<Tran>();
    int tid = 1;

    public void Borrow(string key, Member m)
    {
        int lim = 0, days = 0;

        if (m is Student s) { lim = s.lim; days = s.days; }
        else if (m is Regular r) { lim = r.lim; days = r.days; }
        else if (m is Premium p) { lim = p.lim; days = p.days; }

        if (m.cnt >= lim)
        {
            Console.WriteLine("Limit Reached");
            return;
        }

        Book b = bks.FirstOrDefault(x =>
            (x.ttl == key || x.cat == key) && x.avl);

        if (b == null)
        {
            Console.WriteLine("Book Not Available");
            return;
        }

        b.avl = false;
        m.cnt++;

        Tran t = new Tran
        {
            transactionId = tid++,
            book = b,
            member = m,
            borrowDay = 0,
            dueDay = days,
            returnDay = -1
        };

        trs.Add(t);

        Console.WriteLine("\nBorrowed:");
        Console.WriteLine($"Transaction ID: {t.transactionId}");
        Console.WriteLine($"Book: {b.ttl}");
        Console.WriteLine($"Member: {m.nm}");
        Console.WriteLine($"Borrow Day: {t.borrowDay}");
        Console.WriteLine($"Due Day: {t.dueDay}");
    }

    public void Return(int id, int retDay)
    {
        Tran t = trs.First(x => x.transactionId == id);
        t.returnDay = retDay;
        t.book.avl = true;
        t.member.cnt--;

        int rate = 0;
        if (t.member is Student s) rate = s.fee;
        else if (t.member is Regular r) rate = r.fee;
        else if (t.member is Premium p) rate = p.fee;

        int lateFee = 0;
        if (t.returnDay > t.dueDay)
            lateFee = (t.returnDay - t.dueDay) * rate;

        Console.WriteLine("\nReturned:");
        Console.WriteLine($"Transaction ID: {t.transactionId}");
        Console.WriteLine($"Return Day: {t.returnDay}");
        Console.WriteLine($"Late Fee: {lateFee}");
    }

    public void ShowBorrowed()
    {
        Console.WriteLine("\nAll Borrowed Books:");
        foreach (var t in trs.Where(x => x.returnDay == -1))
            Console.WriteLine($"{t.book.ttl} borrowed by {t.member.nm}");
    }

    public void ShowOverdue(int currentDay)
    {
        Console.WriteLine("\nOverdue Books:");
        foreach (var t in trs.Where(x => x.returnDay == -1 && currentDay > x.dueDay))
            Console.WriteLine($"{t.book.ttl} borrowed by {t.member.nm}");
    }
}

class Program
{
    static void Main()
    {
        Library lb = new Library();

        lb.bks.Add(new EBook { id = 1, ttl = "AI", cat = "Sci", sz = 5 });
        lb.bks.Add(new EBook { id = 2, ttl = "Cloud", cat = "Tech", sz = 8 });
        lb.bks.Add(new PBook { id = 3, ttl = "Java", cat = "Tech", loc = "A1" });
        lb.bks.Add(new PBook { id = 4, ttl = "DBMS", cat = "Edu", loc = "B2" });
        lb.bks.Add(new PBook { id = 5, ttl = "Story", cat = "Fic", loc = "C3" });

        Student s = new Student { id = 1, nm = "Amit" };
        Regular r = new Regular { id = 2, nm = "Ravi" };
        Premium p = new Premium { id = 3, nm = "Neha" };

        lb.Borrow("AI", s);
        lb.Borrow("Tech", r);
        lb.Borrow("Story", p);

        lb.Return(1, 20);

        lb.ShowBorrowed();
        lb.ShowOverdue(30);
    }
}
