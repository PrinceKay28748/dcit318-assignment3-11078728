using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public interface IInventoryEntity
{
    int Id { get; }
}

public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;  // The record uses a positional syntax

public class InventoryLogger<T> where T : IInventoryEntity
{
    private List<T> _log = new();
    private readonly string _filePath;

    public InventoryLogger(string filePath)
    {
        _filePath = filePath;
    }

    public void Add(T item)
    {
        _log.Add(item);
    }

    public List<T> GetAll()
    {
        return _log;
    }

    public void SaveToFile()
    {
        try
        {
            var json = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
            Console.WriteLine("Data saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving data: {ex.Message}");
        }
    }

    public void LoadFromFile()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine("No data file found.");
                return;
            }

            string json = File.ReadAllText(_filePath);
            var items = JsonSerializer.Deserialize<List<T>>(json);
            _log = items ?? new List<T>();
            Console.WriteLine("Data loaded successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading data: {ex.Message}");
        }
    }
}


public class InventoryApp
{
    private InventoryLogger<InventoryItem> _logger;

    public InventoryApp(string filePath)
    {
        _logger = new InventoryLogger<InventoryItem>(filePath);
    }

    public void SeedSampleData()
    {
        _logger.Add(new InventoryItem(1, "Laptop", 5, DateTime.Now));
        _logger.Add(new InventoryItem(2, "Mouse", 15, DateTime.Now));
        _logger.Add(new InventoryItem(3, "Keyboard", 10, DateTime.Now));
        _logger.Add(new InventoryItem(4, "Monitor", 7, DateTime.Now));
        _logger.Add(new InventoryItem(5, "Printer", 3, DateTime.Now));
    }

    public void SaveData()
    {
        _logger.SaveToFile();
    }

    public void LoadData()
    {
        _logger.LoadFromFile();
    }

    public void PrintAllItems()
    {
        var items = _logger.GetAll();
        foreach (var item in items)
        {
            Console.WriteLine($"{item.Id}: {item.Name} - Qty: {item.Quantity} (Added: {item.DateAdded.ToShortDateString()})");
        }
    }
}


