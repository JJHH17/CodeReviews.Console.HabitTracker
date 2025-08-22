// Main program class
using System.Linq.Expressions;
using Microsoft.Data.Sqlite;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello from Main");
        Habit newhabit = new Habit();
        newhabit.setHabit("Running", 5, "2024-06-01");
        Database db = new Database();
        db.viewHabits();
    }
}

// Habit class
class Habit
{
    public string name { get; set; }
    public int quantity { get; set; }
    public string date { get; set; }
    // Connects to database
    private Database db = new Database();

    public void setHabit(string name, int quantity, string date)
    {
        this.name = name;
        this.quantity = quantity;
        this.date = date;

        // Adds habit to database
        db.addHabit(this);
    }
}

// Database class
class Database
{

    // Table schema
    private readonly string tableCreate = @"CREATE TABLE IF NOT EXISTS habits (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            name TEXT,
            quantity INTEGER,
            date TEXT
        )";

    // Constructor should create a db if one doesn't exist
    public Database()
    {
        try
        {
            // Creates db in bin/debug/net7.0
            using (var connection = new SqliteConnection("Data Source=habits.db"))
            {
                // Creates table if it doesn't exist
                connection.Open();
                using var command = new SqliteCommand(tableCreate, connection);
                command.ExecuteNonQuery();

                Console.WriteLine("Database and table connection created.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
    }

    // Adds a habit to the database
    public void addHabit(Habit habit)
    {
        using (var connection = new SqliteConnection("Data Source=habits.db"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO habits (name, quantity, date) VALUES ($name, $quantity, $date)";
            command.Parameters.AddWithValue("$name", habit.name);
            command.Parameters.AddWithValue("$quantity", habit.quantity);
            command.Parameters.AddWithValue("$date", habit.date);
            command.ExecuteNonQuery();
        }
    }

    // Prints all habits in the database (Includes ID which we can later use to delete or update habits)
    public void viewHabits()
    {
        var sql = "SELECT * FROM habits";

        try
        {
            using var connection = new SqliteConnection(@"Data Source=habits.db");
            connection.Open();

            using var command = new SqliteCommand(sql, connection);

            using var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var name = reader.GetString(1);
                    var quantity = reader.GetInt32(2);
                    var date = reader.GetString(3);
                    Console.WriteLine($"Habit ID: {id}, Name: {name}, Quantity: {quantity}, Date: {date}");
                }
            }
            else
            {
                Console.WriteLine("No habits founds");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
    }

    // Method to edit a given habit by its ID

    // Method to delete a given habit by its ID

    // Method to delete all habits
}