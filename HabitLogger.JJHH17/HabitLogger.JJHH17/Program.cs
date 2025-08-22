// Main program class
using System.Linq.Expressions;
using Microsoft.Data.Sqlite;

class Program
{
    static void Main(string[] args)
    {
        // Main program loop
        bool running = true;
        Database db = new Database();

        Console.WriteLine("Welcome to the Habit Logger!");

        while (running)
        {
            Console.WriteLine("Please select an option:");
            Console.WriteLine("1. Add a habit");
            Console.WriteLine("2. View habits");
            Console.WriteLine("3. Delete all habits");
            Console.WriteLine("4. Delete a habit by ID");
            Console.WriteLine("5. Edit a habit (by ID)");
            Console.WriteLine("6. Exit");

            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    Console.WriteLine("Enter habit name:");
                    string name = Console.ReadLine();
                    Console.WriteLine("Enter quantity of habit (number):");
                    int quantity;
                    while (!int.TryParse(Console.ReadLine(), out quantity))
                    {
                        Console.WriteLine("Invalid input. Please enter a number for quantity:");
                    }
                    Console.WriteLine("Enter date (YYYY-MM-DD, or use an alternative format):");
                    string date = Console.ReadLine();
                    Habit habit = new Habit();
                    habit.setHabit(name, quantity, date);
                    Console.WriteLine("Habit added successfully!");
                    break;

                case "2":
                    db.viewHabits();
                    break;

                case "3":
                    db.deleteAllHabits();
                    break;

                case "4":
                    Console.WriteLine("Enter the ID of the habit to delete:");
                    // Parse input to an integer (all habit IDs are integers)
                    int id;
                    while (!int.TryParse(Console.ReadLine(), out id))
                    {
                        Console.WriteLine("Invalid input. Please enter a number for the habit ID:");
                    }
                    db.deleteHabitById(id);
                    break;

                case "5":
                    Console.WriteLine("Enter the ID of the habit to edit:");
                    int editId;
                    while (!int.TryParse(Console.ReadLine(), out editId))
                    {
                        Console.WriteLine("Invalid input. Please enter a number for the habit ID:");
                    }
                    Console.WriteLine("Enter new habit name:");
                    string newName = Console.ReadLine();
                    Console.WriteLine("Enter new quantity of habit (number):");
                    int newQuantity;
                    while (!int.TryParse(Console.ReadLine(), out newQuantity))
                    {
                        Console.WriteLine("Invalid input. Please enter a number for quantity:");
                    }
                    Console.WriteLine("Enter new date (YYYY-MM-DD, or use an alternative format):");
                    string newDate = Console.ReadLine();
                    db.editHabitById(editId, newName, newQuantity, newDate);
                    break;

                case "6":
                    running = false;
                    Console.WriteLine("Exiting the program. Goodbye!");
                    break;

                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
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
                // Creates the database file in bin/debug/net7.0 directory of project
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

        // Method to delete all habits
        public void deleteAllHabits()
        {
            var sql = "DELETE FROM habits";
            // Provide a warning message before deleting all habits
            Console.WriteLine("Are you sure you want to delete all habits? This action cannot be undone. (y/n)");
            switch (Console.ReadLine().ToLower())
            {
                case "y":
                    try
                    {
                        using var connection = new SqliteConnection(@"Data Source=habits.db");
                        connection.Open();
                        using var command = new SqliteCommand(sql, connection);
                        int rowsAffected = command.ExecuteNonQuery();
                        Console.WriteLine($"{rowsAffected} habits deleted.");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error: " + e.Message);
                    }
                    break;

                case "n":
                    Console.WriteLine("Deletion request cancelled.");
                    break;

                default:
                    // We close this if the input to prevent accidental deletion 
                    Console.WriteLine("Invalid input. Closing request.");
                    break;
            }
        }

        // Method to delete a given habit by its ID
        public void deleteHabitById(int id)
        {
            var sql = "DELETE FROM habits WHERE id = $id";
            // Provides a pre deletion warning message
            Console.WriteLine($"Are you sure you want to delete habit ID {id}? This action cannot be reverted. (Y/N)");
            switch (Console.ReadLine().ToLower())
            {
                case "y":
                    try
                    {
                        using var connection = new SqliteConnection(@"Data Source=habits.db");
                        connection.Open();
                        using var command = new SqliteCommand(sql, connection);
                        command.Parameters.AddWithValue("$id", id);
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            Console.WriteLine($"Habit with ID {id} deleted.");
                        }
                        else
                        {
                            Console.WriteLine($"No habit found with ID {id}.");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error: " + e.Message);
                    }
                    break;

                case "n":
                    Console.WriteLine("Deletion request cancelled.");
                    return;

                default:
                    // We close this if the input to prevent accidental deletion 
                    Console.WriteLine("Invalid input. Closing request.");
                    return;
            }
        }

        // Method to edit a given habit by its ID
        public void editHabitById(int id, string newName, int newQuantity, string newDate)
        {
            var sql = "UPDATE habits SET name = $name, quantity = $quantity, date = $date WHERE id = $id";

            try
            {
                using var connection = new SqliteConnection(@"Data Source=habits.db");
                connection.Open();
                using var command = new SqliteCommand(sql, connection);
                command.Parameters.AddWithValue("$id", id);
                command.Parameters.AddWithValue("$name", newName);
                command.Parameters.AddWithValue("$quantity", newQuantity);
                command.Parameters.AddWithValue("$date", newDate);
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine($"Habit with ID {id} updated.");
                }
                else
                {
                    Console.WriteLine($"No habit found with ID {id}.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
        }
    }
}