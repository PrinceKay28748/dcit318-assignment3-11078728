using System;
using System.Collections.Generic;


public interface IInventoryItem
{
    int Id { get; }
    string Name { get; }
    int Quantity { get; set; }
}

public class ElectronicItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public string Brand { get; }
    public int WarrantyMonths { get; }

    public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        Brand = brand;
        WarrantyMonths = warrantyMonths;
    }

    public override string ToString()
    {
        return $"Electronic: {Name} (ID: {Id}, Brand: {Brand}, Warranty: {WarrantyMonths} months, Qty: {Quantity})";
    }
}


public class GroceryItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; }

    public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        ExpiryDate = expiryDate;
    }

    public override string ToString()
    {
        return $"Grocery: {Name} (ID: {Id}, Expires: {ExpiryDate:yyyy-MM-dd}, Qty: {Quantity})";
    }
}

public class DuplicateItemException : Exception
{
    public DuplicateItemException(string message) : base(message) { }
}

public class ItemNotFoundException : Exception
{
    public ItemNotFoundException(string message) : base(message) { }
}

public class InvalidQuantityException : Exception
{
    public InvalidQuantityException(string message) : base(message) { }
}


public class InventoryRepository<T> where T : IInventoryItem
{
    private Dictionary<int, T> _items = new Dictionary<int, T>();

    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id))
            throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
        _items[item.Id] = item;
    }

    public T GetItemById(int id)
    {
        if (_items.TryGetValue(id, out var item))
            return item;
        throw new ItemNotFoundException($"Item with ID {id} not found.");
    }

    public void RemoveItem(int id)
    {
        if (!_items.ContainsKey(id))
            throw new ItemNotFoundException($"Item with ID {id} not found.");
        _items.Remove(id);
    }

    public List<T> GetAllItems()
    {
        return new List<T>(_items.Values);
    }

    public void UpdateQuantity(int id, int newQuantity)
    {
        if (newQuantity < 0)
            throw new InvalidQuantityException("Quantity cannot be negative.");

        if (!_items.ContainsKey(id))
            throw new ItemNotFoundException($"Item with ID {id} not found.");

        _items[id].Quantity = newQuantity;
    }
}

public class WareHouseManager
{
    private InventoryRepository<ElectronicItem> _electronics = new InventoryRepository<ElectronicItem>();
    private InventoryRepository<GroceryItem> _groceries = new InventoryRepository<GroceryItem>();

    public void SeedData()
    {
        // Add electronic items
        _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24));
        _electronics.AddItem(new ElectronicItem(2, "Smartphone", 15, "Samsung", 12));

        // Add grocery items
        _groceries.AddItem(new GroceryItem(101, "Milk", 20, DateTime.Now.AddDays(5)));
        _groceries.AddItem(new GroceryItem(102, "Bread", 30, DateTime.Now.AddDays(2)));
    }

    public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
    {
        foreach (var item in repo.GetAllItems())
        {
            Console.WriteLine(item);
        }
    }

    public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
    {
        try
        {
            var item = repo.GetItemById(id);
            repo.UpdateQuantity(id, item.Quantity + quantity);
            Console.WriteLine($"Stock updated: {item.Name} now has {item.Quantity + quantity} units.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] {ex.Message}");
        }
    }

    public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
    {
        try
        {
            repo.RemoveItem(id);
            Console.WriteLine($"Item with ID {id} removed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] {ex.Message}");
        }
    }

    public InventoryRepository<ElectronicItem> GetElectronicsRepo() => _electronics;
    public InventoryRepository<GroceryItem> GetGroceriesRepo() => _groceries;
}

public class Program
{
    public static void Main()
    {
        var manager = new WareHouseManager();
        manager.SeedData();

        Console.WriteLine("== Grocery Items ==");
        manager.PrintAllItems(manager.GetGroceriesRepo());

        Console.WriteLine("\n== Electronic Items ==");
        manager.PrintAllItems(manager.GetElectronicsRepo());

        Console.WriteLine("\n== TEST CASES ==");

        // Try to add duplicate item
        try
        {
            manager.GetElectronicsRepo().AddItem(new ElectronicItem(1, "TV", 5, "LG", 18));
        }
        catch (DuplicateItemException ex)
        {
            Console.WriteLine($"[Duplicate Error] {ex.Message}");
        }

        // Try to remove non-existent item
        manager.RemoveItemById(manager.GetGroceriesRepo(), 999);

        // Try to update with invalid quantity
        try
        {
            manager.GetGroceriesRepo().UpdateQuantity(101, -5);
        }
        catch (InvalidQuantityException ex)
        {
            Console.WriteLine($"[Invalid Quantity] {ex.Message}");
        }

        // Final state
        Console.WriteLine("\n== Final Grocery Inventory ==");
        manager.PrintAllItems(manager.GetGroceriesRepo());
    }
}

