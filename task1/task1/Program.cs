public record Transaction(int id, DateTime date, decimal amount, string category);

public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[BankTransfer] Processing {transaction.amount:C} for {transaction.category}");
    }
}

public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[MobileMoney] Processing {transaction.amount:C} for {transaction.category}");
    }
}

public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[CryptoWallet] Processing {transaction.amount:C} for {transaction.category}");
    }
}

public class Account
{
    public string AccountNumber { get; }
    public decimal Balance { get; protected set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    public virtual void ApplyTransaction(Transaction transaction)
    {
        Balance -= transaction.amount;
        Console.WriteLine($"[Account] {transaction.amount:C} deducted. New Balance: {Balance:C}");
    }
}

public sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountNumber, decimal initialBalance)
        : base(accountNumber, initialBalance) { }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.amount > Balance)
        {
            Console.WriteLine("Insufficient funds.");
        }
        else
        {
            base.ApplyTransaction(transaction);
        }
    }
}

public class FinanceApp
{
    List<Transaction> _transactions = new List<Transaction>();

    public void Run()
    {
        // Step i. Create an account  
        var account = new SavingsAccount("SB9040010470271", 1000);

        // Step ii. Create 3 transactions  
        Transaction t1 = new Transaction(1, DateTime.Now, 150, "Groceries");
        Transaction t2 = new Transaction(2, DateTime.Now, 300, "Utilities");
        Transaction t3 = new Transaction(3, DateTime.Now, 700, "Entertainment");

        // Step iii. Use processors  
        ITransactionProcessor p1 = new MobileMoneyProcessor();
        ITransactionProcessor p2 = new BankTransferProcessor();
        ITransactionProcessor p3 = new CryptoWalletProcessor();

        // Process transactions  
        p1.Process(t1);
        p2.Process(t2);
        p3.Process(t3);

        // Apply transactions to the account  
        account.ApplyTransaction(t1);
        account.ApplyTransaction(t2);
        account.ApplyTransaction(t3);
    }
}

class Program
{
    static void Main()
    {
        var app = new FinanceApp();
        app.Run();
    }
}
