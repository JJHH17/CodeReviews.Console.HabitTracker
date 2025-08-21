// Main program class
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello from Main");
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