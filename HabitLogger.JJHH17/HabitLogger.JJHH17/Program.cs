using System.Linq.Expressions;
using Microsoft.Data.Sqlite;
using System;

class Program
{
    static void Main(string[] args)
    {
        bool running = true;
        Database db = new Database();
        // Appending the seeded data to the db (Remove this line if you don't want seed data)
        db.seedData();
        // Used for date functionality
        DateTime currentDate = DateTime.Now;

        Console.WriteLine("\nWelcome to the Habit Logger!\n");

        // Main program loop
        while (running)
        {
            Console.WriteLine("\nPlease select an option\n");
            Console.WriteLine("\t1. Add a habit\n");
            Console.WriteLine("\t2. View habits\n");
            Console.WriteLine("\t3. Delete all habits\n");
            Console.WriteLine("\t4. Delete a habit by ID\n");
            Console.WriteLine("\t5. Edit a habit (by ID)\n");
            Console.WriteLine("\t6. Fetch habit category quantity\n");
            Console.WriteLine("\t7. Exit");

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
                    Console.WriteLine("1. Enter a date 2. Use todays date");
                    string date;
                    switch (Console.ReadLine())
                    {
                        case "1":
                            Console.WriteLine("Enter date (YYYY-MM-DD, or use an alternative format):");

                            while (true)
                            {
                                try
                                {
                                    date = DateTime.Parse(Console.ReadLine()).ToString("yyyy-MM-dd");
                                    break;
                                }
                                catch (FormatException)
                                {
                                    Console.WriteLine("Invalid date format. Please enter the date again (YYYY-MM-DD, or use an alternative format):");
                                }
                            }
                            break;

                        case "2":
                            Console.WriteLine($"Using today's date: {currentDate.ToString("yyyy-MM-dd")}");
                            date = currentDate.ToString("yyyy-MM-dd");
                            break;

                        default:
                            Console.WriteLine("Invalid option. Using today's date by default.");
                            date = currentDate.ToString("yyyy-MM-dd");
                            break;
                    };
                    string category = "other"; // Default category

                    // Prompts user to enter a category for the habit
                    Console.WriteLine("Enter category of habit (Run, Leisure, Work, Errands, Social, Other)");
                    string inputCategory = Console.ReadLine();
                    switch (inputCategory.ToLower())
                    {
                        case "run":
                            category = "run";
                            break;

                        case "leisure":
                            category = "leisure";
                            break;

                        case "work":
                            category = "work";
                            break;

                        case "errands":
                            category = "errands";
                            break;

                        case "social":
                            category = "social";
                            break;

                        case "other":
                            category = "other";
                            break;

                        default:
                            Console.WriteLine("Invalid category. Using 'Other' as default.");
                            category = "other";
                            break;
                    }


                    Habit habit = new Habit();
                    habit.setHabit(name, quantity, date, category);
                    Console.WriteLine("Habit added successfully!");
                    break;

                case "2":
                    db.viewHabits();
                    break;

                case "3":
                    db.deleteAllHabits();
                    break;

                case "4":
                    Console.WriteLine("\nEnter the ID of the habit to delete:");
                    // Parse input to an integer (all habit IDs are integers)
                    int id;
                    while (!int.TryParse(Console.ReadLine(), out id))
                    {
                        Console.WriteLine("Invalid input. Please enter a number for the habit ID:");
                    }
                    db.deleteHabitById(id);
                    break;

                case "5":
                    Console.WriteLine("\nEnter the ID of the habit to edit:");
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
                    DateTime newDate;
                    while (true)
                    {
                        try
                        {
                            newDate = DateTime.Parse(Console.ReadLine());
                            break;
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("Invalid date format. Please enter the date again (YYYY-MM-DD, or use an alternative format):");
                        }
                    }

                    db.editHabitById(editId, newName, newQuantity, newDate);
                    break;

                case "6":
                    Console.WriteLine("\nEnter the category to fetch total quantity (Run, Leisure, Work, Errands, Social, Other):");
                    string habitCategory = Console.ReadLine().ToLower();
                    db.fetchCategoryQuantities(habitCategory);
                    break;

                case "7":
                    running = false;
                    Console.WriteLine("\nExiting the program. Goodbye!");
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
    public string category { get; set; }
    // Connects to database
    private Database db = new Database();

    public void setHabit(string name, int quantity, string date, string category)
    {
        this.name = name;
        this.quantity = quantity;
        this.date = date;
        this.category = category;

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
            date TEXT,
            category TEXT
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
                command.CommandText = @"INSERT INTO habits (name, quantity, date, category) VALUES ($name, $quantity, $date, $category)";
                command.Parameters.AddWithValue("$name", habit.name);
                command.Parameters.AddWithValue("$quantity", habit.quantity);
                command.Parameters.AddWithValue("$date", habit.date);
                command.Parameters.AddWithValue("$category", habit.category);
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
                        var category = reader.GetString(4);
                        Console.WriteLine($"\nHabit ID: {id}, Name: {name}, Quantity: {quantity}, Date: {date}, Category: {category}");
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
            Console.WriteLine("\nAre you sure you want to delete all habits? This action cannot be undone. (y/n)");
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
            Console.WriteLine($"\nAre you sure you want to delete habit ID {id}? This action cannot be reverted. (Y/N)");
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
        public void editHabitById(int id, string newName, int newQuantity, DateTime newDate)
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

        // Method to fetch quantity of each category
        public void fetchCategoryQuantities(string category)
        {
            var sql = "SELECT SUM(quantity) FROM habits WHERE category = $category";

            try
            {
                using var connection = new SqliteConnection(@"Data Source=habits.db");
                connection.Open();
                using var command = new SqliteCommand(sql, connection);
                command.Parameters.AddWithValue("$category", category);

                var result = command.ExecuteScalar();

                // Check if result is null (or if table is null with given category)
                if (result == DBNull.Value || result == null)
                {
                    Console.WriteLine($"No habits found in category '{category}'.");
                    return;
                }
                else
                {
                    int totalQuantity = Convert.ToInt32(result);
                    Console.WriteLine($"Total quantity for category '{category}': {totalQuantity}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
        }

        public void seedData()
        {
            // Creating a loop to add 100 habits to the database
            for (int i = 0; i < 100; i++)
            {
                Habit habit = new Habit();
                habit.setHabit($"Habit {i + 1}", i + 1, "2023-10-07", "other");
            }
            Console.WriteLine("Database seed data added (Auto adds habits)");
        }
    }
}