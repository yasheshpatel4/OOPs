using System;
using System.Collections.Generic;

abstract class Book 
{
    public string Title { get; }    
    public string Category { get; }
    public bool IsAvailable { get; private set; } = true;

    protected Book(string title, string category)
    {
        Title = title;
        Category = category;
    }

    public void Borrow()
    {
        if (!IsAvailable)
            throw new BookUnavailableException("Book not available...");
        IsAvailable = false;
    }

    public void Return()
    {
        IsAvailable = true;
    }
}

class EBook : Book
{
    public double FileSizeMB { get; }

    public EBook(string title, string category, double size)
        : base(title, category)
    {
        FileSizeMB = size;
    }
}

class PhysicalBook : Book
{
    public string Shelf { get; }

    public PhysicalBook(string title, string category, string shelf)
        : base(title, category)
    {
        Shelf = shelf;
    }
}
abstract class Membership 
{
    public int MaxBooks { get; }
    public int BorrowDays { get; }

    protected Membership(int maxBooks, int days)
    {
        MaxBooks = maxBooks;
        BorrowDays = days;
    }

    public abstract double CalculateLateFee(int lateDays);
}

class StudentMembership : Membership
{
    public StudentMembership() : base(3, 15) { }

    public override double CalculateLateFee(int lateDays)
        => lateDays * 5;
}

class RegularMembership : Membership
{
    public RegularMembership() : base(5, 30) { }

    public override double CalculateLateFee(int lateDays)
        => lateDays * 10;
}

class PremiumMembership : Membership
{
    public PremiumMembership() : base(int.MaxValue, 45) { }

    public override double CalculateLateFee(int lateDays)
        => 0; // No late fee
}
class Member
{
    public string Name { get; }
    public Membership Membership { get; }
    private readonly List<Book> borrowedBooks = new();

    public Member(string name, Membership membership)
    {
        Name = name;
        Membership = membership;
    }

    public void BorrowBook(Book book)
    {
        if (borrowedBooks.Count >= Membership.MaxBooks)
            throw new Exception("Borrow limit exceeded");

        book.Borrow();
        borrowedBooks.Add(book);
    }

    public void ReturnBook(Book book)
    {
        book.Return();
        borrowedBooks.Remove(book);
    }

    public int BorrowedCount => borrowedBooks.Count;
}
class Transaction
{
    private static int counter = 1;

    public int TransactionId { get; }
    public Book Book { get; }
    public Member Member { get; }
    public int BorrowDay { get; }
    public int DueDay { get; }
    public int? ReturnDay { get; private set; }

    public Transaction(Book book, Member member, int currentDay)
    {
        TransactionId = counter++;
        Book = book;
        Member = member;
        BorrowDay = currentDay;
        DueDay = BorrowDay + member.Membership.BorrowDays;
    }

    public double CloseTransaction(int returnDay)
    {
        ReturnDay = returnDay;
        int lateDays = Math.Max(0, returnDay - DueDay);
        return Member.Membership.CalculateLateFee(lateDays);
    }
}
class BorrowService
{
    private readonly List<Transaction> transactions = new();

    public Transaction Borrow(Member member, Book book, int currentDay)
    {
        member.BorrowBook(book);
        var tx = new Transaction(book, member, currentDay);
        transactions.Add(tx);

        Console.WriteLine($"TX:{tx.TransactionId} | {book.Title} | {member.Name} | Due Day:{tx.DueDay}");
        return tx;
    }

    public void Return(Transaction tx, int currentDay)
    {
        double fee = tx.CloseTransaction(currentDay);
        tx.Member.ReturnBook(tx.Book);

        Console.WriteLine($"TX:{tx.TransactionId} | Returned Day:{currentDay} | Fee:₹{fee}");
    }

    public void PrintOverdue(int currentDay)
    {
        foreach (var tx in transactions)
            if (tx.ReturnDay == null && currentDay > tx.DueDay)
                Console.WriteLine("Overdue: " + tx.Book.Title);
    }
}

class BookUnavailableException : Exception
{
    public BookUnavailableException(string msg) : base(msg) { }
}
class Program
{
    static void Main()
    {
        var books = new List<Book>
        {
            new EBook("C# Pro", "Programming", 5.4),
            new EBook("Java Master", "Programming", 6.1),
            new PhysicalBook("Physics", "Science", "A1"),
            new PhysicalBook("Chemistry", "Science", "B2"),
            new PhysicalBook("Maths", "Education", "C3")
        };
        var student = new Member("Amit", new StudentMembership());

        var borrowService = new BorrowService();
        int currentDay = 1;

        var tx1 = borrowService.Borrow(student, books[0], currentDay);

        currentDay = 20;

        borrowService.Return(tx1, currentDay);
        borrowService.PrintOverdue(currentDay);
    }
}
