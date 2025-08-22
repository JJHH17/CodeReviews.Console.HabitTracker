// Main program class
using Microsoft.Data.Sqlite;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello from Main");
        Database db = new Database();

    }
}

// Habit class
class Habit
{
    public string name { get; set; }
    public int quantity { get; set; }
    public string date { get; set; }

    public Habit(string name, int quantity, string date)
    {
        this.name = name;
        this.quantity = quantity;
        this.date = date;
    }
}

// Database class
class Database
{
    // Constructor should create a db if one doesn't exist
    public Database()
    {
        try
        {
            // Creates db in bin/debug/net7.0
            using (var connection = new SqliteConnection("Data Source=test.db"))
            {
                connection.Open();
                Console.WriteLine("Database opened");
                connection.Close();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
    }
}