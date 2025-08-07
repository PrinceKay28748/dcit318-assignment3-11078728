using System;
using System.Collections.Generic;
using System.Linq; // Language Integrated Query

public class Repository<T>
{
    List<T> items = new List<T>();
    public void Add(T item) => items.Add(item);


    public List<T> GetAll() => new List<T>(items);

    public T? GetById(Func<T, bool> predicate)
    {
        return items.FirstOrDefault(predicate);
    }

    public bool Remove(Func<T, bool> predicate)
    {
        var item = items.FirstOrDefault(predicate);
        if (item != null)
        {
            items.Remove(item);
            return true;
        }
        return false;
    }

}

public class Patient 
{
    public int Id { get; }
    public string Name { get; }
    public int Age { get; }
    public string Gender { get; }


    public Patient(int id, string name, int age, string gender)
    {
        Id = id;
        Name = name;
        Age = age;
        Gender = gender;
    }

    public override string ToString()
    {
        return $"Patient [Id={Id}, Name={Name}, Age={Age}, Gender={Gender}]";
    }
}

public class Prescription
{
    public int Id { get; }
    public int PatientId { get; }
    public string MedicationName { get; }
    public DateTime DateIssued { get; }

    public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
    {
        Id = id;
        PatientId = patientId;
        MedicationName = medicationName;
        DateIssued = dateIssued;
    }

    public override string ToString()
    {
        return $"Prescription [Id={Id}, PatientId={PatientId}, Medication={MedicationName}, DateIssued={DateIssued:d}]";
    }
}

public class HealthSystemApp 
{
    private Repository<Patient> _patientRepo = new Repository<Patient>();
    private Repository<Prescription> _prescriptionRepo = new Repository<Prescription>();
    private Dictionary<int, List<Prescription>> _prescriptionMap = new Dictionary<int, List<Prescription>>();


    public void SeedData()
    {
        // Add patients
        _patientRepo.Add(new Patient(1, "Alice Johnson", 30, "Female"));
        _patientRepo.Add(new Patient(2, "Bob Smith", 45, "Male"));
        _patientRepo.Add(new Patient(3, "Carla Davis", 27, "Female"));

        // Add prescriptions
        _prescriptionRepo.Add(new Prescription(101, 1, "Amoxicillin", DateTime.Now.AddDays(-5)));
        _prescriptionRepo.Add(new Prescription(102, 1, "Ibuprofen", DateTime.Now.AddDays(-2)));
        _prescriptionRepo.Add(new Prescription(103, 2, "Metformin", DateTime.Now.AddDays(-7)));
        _prescriptionRepo.Add(new Prescription(104, 3, "Lisinopril", DateTime.Now.AddDays(-3)));
        _prescriptionRepo.Add(new Prescription(105, 1, "Paracetamol", DateTime.Now.AddDays(-1)));
    }

    // Group prescriptions by PatientId
    public void BuildPrescriptionMap()
    {
        _prescriptionMap.Clear();

        foreach (var prescription in _prescriptionRepo.GetAll())
        {
            if (!_prescriptionMap.ContainsKey(prescription.PatientId))
            {
                _prescriptionMap[prescription.PatientId] = new List<Prescription>();
            }
            _prescriptionMap[prescription.PatientId].Add(prescription);
        }
    }

    // Get all patients 
    public void PrintAllPatients()
    {
        Console.WriteLine("=== Patient List ===");
        foreach (var patient in _patientRepo.GetAll())
        {
            Console.WriteLine(patient);
        }
        Console.WriteLine();
    }

    // // Fetch prescriptions for one patient
    public void PrintPrescriptionsForPatient(int patientId)
    {
        if (_prescriptionMap.ContainsKey(patientId))
        {
            Console.WriteLine($"=== Prescriptions for Patient ID {patientId} ===");
            foreach (var p in _prescriptionMap[patientId])
            {
                Console.WriteLine(p);
            }
        }
        else
        {
            Console.WriteLine($"No prescriptions found for patient ID {patientId}.");
        }
    }


}

// Main Program

public class Program
{
    public static void Main()
    {
        var app = new HealthSystemApp();

        app.SeedData();
        app.BuildPrescriptionMap();
        app.PrintAllPatients();

        // Choose a PatientId (e.g., 1)
        Console.Write("Enter Patient ID to view prescriptions: ");
        if (int.TryParse(Console.ReadLine(), out int patientId))
        {
            app.PrintPrescriptionsForPatient(patientId);
        }
        else
        {
            Console.WriteLine("Invalid input.");
        }
    }
}

