# Habit Tracker Application

- A command line based habit tracker, built following the "HabitLogger" project on CSharpAcademy: https://www.thecsharpacademy.com/project/12/habit-logger

# Technologies used:
- C#, .Net
- SQLite

# Project Objectives:
- Write a command line based C# application.
- Allows users to add, update and delete Habits.
- Habits are all stored via a Database (This project uses SQLite)
- The Database file, by default, is stored via: HabitLogger.JJHH17\bin\Debug\net8.0\habits

# Project Details

```How to use Project```
- Clone the repo to your local directory and run it
You're presented with a number of options, enter the number option you wish to run:
<img width="700" height="550" alt="image" src="https://github.com/user-attachments/assets/a1d8e68e-dd2d-4031-8481-a2333841849b" />

1. Add a habit (allows you to add a name, quantity, category and date of the habit.
2. View a list of habits
3. Delete all habits from the database
4. Delete a habit from a given habit ID (which are all unique to the given habit).
5. Edit a habit (from a given habit ID).
6. Fetch the quantity of a habit, based on its habit category.
7. Exit the program

You'll also see the "Database seed data added" text at the top of the terminal.
This is managed via a db.seedData() method at the top of the main program class, it auto-appends 100 habits to the database on program launch.
I'd recommend removing this call (db.seedData, on line 12 of Program.cs) if you wish for data seeding to not occur (or you can alternatively use the "Delete all habits" command upon launching the app).

# Key Learnings from Project:
In previous projects, I had been bouncing between using VSCode on my local machine (mac) and Visual Studio via a Windows VM - Based on the general documentation that Microsoft and CSharpAcademy provides, I opted to use Visual Studio, which I found a rewarding and new experience.

It's also my first time using SQLite; I have some experience with Postgresql through Java and Python, but this helped me understand the importance of SQLite for this type of project.

I also learned more key concepts such as:

- Data seeding, what it is and how to do it (I'm certain there's a more efficient way of doing it than how I did it (a loop to 100 entries), which I will explore).
- Object Oriented Programming.
- Beginner C# program structure.
- How C# projects are generally structured.
- .Net libraries and NuGet packages.

# What would I do differently?
It's still early days as I suppose this is the first project that the academy has students complete without any real formal guidance (if you opt to tackle the project without watching the tutorial), although the Program.cs file that I have feels quite long and there is a total of 3 classes in there, so I definitely need to get better at splitting these into seperate class libraries, which i'm trying to get more confident in.

I will look to perform certain tasks more efficiently, such as data seeding.

I also need to gain a lower level understanding of the C# project structure, the IDE and what the different components mean within the project.

Overall, I'd like to thank the CSharpAcademy for the resources provided here, the discord community for the inspiration gained from the relevant inspiration, and I look forward to tackling the next project.

