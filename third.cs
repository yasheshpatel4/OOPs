using System;
using System.Collections.Generic;
using System.Linq;

class DoctorBusyException : Exception
{
    public DoctorBusyException(string msg) : base(msg) { }
}

class Doctor
{
    public int id;
    public string name;
    public string spec;
    public int fee;
    public List<string> slots = new List<string>();
}

class Patient
{
    public int id;
    public string name;
    public int age;
    public List<string> history = new List<string>();
}

class Appointment
{
    public int ticketId;
    public Doctor doctor;
    public Patient patient;
    public string time;
    public bool done = false;
    public string diagnosis;
    public int bill;
}

class Hospital
{
    public List<Doctor> doctors = new List<Doctor>();
    public List<Patient> patients = new List<Patient>();
    public List<Appointment> apps = new List<Appointment>();
    int tid = 1;

    public void Book(string key, string time, Patient p)
    {
        Doctor d = doctors.FirstOrDefault(x =>
            (x.name == key || x.spec == key) && x.slots.Contains(time));

        if (d == null)
        {
            Doctor alt = doctors.FirstOrDefault(x => x.spec == key && x.slots.Count > 0);
            if (alt != null)
                throw new DoctorBusyException("Doctor busy. Try time: " + alt.slots[0]);
            else
                throw new DoctorBusyException("No doctor available");
        }

        d.slots.Remove(time);

        Appointment a = new Appointment
        {
            ticketId = tid++,
            doctor = d,
            patient = p,
            time = time,
            bill = d.fee
        };

        apps.Add(a);

        Console.WriteLine("\nAppointment Booked:");
        Console.WriteLine($"Ticket ID: {a.ticketId}");
        Console.WriteLine($"Doctor: {d.name}");
        Console.WriteLine($"Time: {time}");
        Console.WriteLine($"Fee: {d.fee}");
    }

    public void Complete(int id, string diag, int testFee)
    {
        Appointment a = apps.First(x => x.ticketId == id);
        a.done = true;
        a.diagnosis = diag;
        a.bill += testFee;
        a.patient.history.Add(diag);

        Console.WriteLine("\nAppointment Completed:");
        Console.WriteLine($"Patient: {a.patient.name}");
        Console.WriteLine($"Dignosis: {diag}");
        Console.WriteLine($"Final Bill: {a.bill}");
    }

    public void ShowAppointments()
    {
        Console.WriteLine("\nAll Appointments:");
        foreach (var a in apps)
            Console.WriteLine($"{a.ticketId} - {a.doctor.name} - {a.patient.name} - {a.time}");
    }
}

class Program
{
    static void Main()
    {
        Hospital h = new Hospital();

        h.doctors.Add(new Doctor
        {
            id = 1,
            name = "Dr. Sharma",
            spec = "Cardiologist",
            fee = 500,
            slots = new List<string> { "10:00 AM", "11:00 AM" }
        });

        h.doctors.Add(new Doctor
        {
            id = 2,
            name = "Dr. Mehta",
            spec = "Dermitologist",
            fee = 300,
            slots = new List<string> { "11:00 AM", "12:00 PM" }
        });

        h.doctors.Add(new Doctor
        {
            id = 3,
            name = "Dr. Rao",
            spec = "Cardiologist",
            fee = 600,
            slots = new List<string> { "12:00 PM" }
        });

        Patient p1 = new Patient { id = 1, name = "Amit", age = 25 };
        Patient p2 = new Patient { id = 2, name = "Ravi", age = 40 };

        h.patients.Add(p1);
        h.patients.Add(p2);

        try
        {
            h.Book("Dr. Sharma", "10:00 AM", p1);
            h.Book("Cardiologist", "11:00 AM", p2);
        }
        catch (DoctorBusyException e)
        {
            Console.WriteLine(e.Message);
        }

        h.ShowAppointments();

        h.Complete(1, "High BP", 200);
        h.Complete(2, "Heart Checkup", 300);
    }
}
