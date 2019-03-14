using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace K4_L7_BulletinBoard
{
    public class AppDbContext : DbContext
    {
        public DbSet<Account> Account { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<Cathegory> Cathegory { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(@"Data Source=den1.mssql8.gear.host;Initial Catalog=bulletineboard;Persist Security Info=True;User ID=bulletineboard;Password=***********");
        }

        protected override void OnModelCreating(ModelBuilder model)
        {
            //model.Entity<AlbumProducer>().HasKey(ap => new { ap.AlbumID, ap.ProducerID });
        }
    }

    public class Account
    {
        [Key]
        public int ID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    
    }

    public class Category
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
    }

    class Program
    {
        static AppDbContext database;

        static void Main(string[] args)
        {
            using (database = new AppDbContext())
            {
                while (true)
                {
                    string option = ShowMenu("Welcome to Bulletin Board - for when you've got nothing better to do!", new[] {
                        "Sign in",
                        "Create Account",
                        "Quit"
                    });
                    Console.Clear();

                    if (option == "Sign in") SignIn();
                    else if (option == "Create Account") CreateAccount();
                    else Environment.Exit(0);

                    Console.WriteLine();
                }
            }
        }


        static string ShowMenu(string prompt, string[] options)
        {
            Console.WriteLine(prompt);

            int selected = 0;

            // Hide the cursor that will blink after calling ReadKey.
            Console.CursorVisible = false;

            ConsoleKey? key = null;
            while (key != ConsoleKey.Enter)
            {
                // If this is not the first iteration, move the cursor to the first line of the menu.
                if (key != null)
                {
                    Console.CursorLeft = 0;
                    Console.CursorTop = Console.CursorTop - options.Length;
                }

                // Print all the options, highlighting the selected one.
                for (int i = 0; i < options.Length; i++)
                {
                    var option = options[i];
                    if (i == selected)
                    {
                        Console.BackgroundColor = ConsoleColor.Blue;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    Console.WriteLine("- " + option);
                    Console.ResetColor();
                }

                // Read another key and adjust the selected value before looping to repeat all of this.
                key = Console.ReadKey().Key;
                if (key == ConsoleKey.DownArrow)
                {
                    selected = Math.Min(selected + 1, options.Length - 1);
                }
                else if (key == ConsoleKey.UpArrow)
                {
                    selected = Math.Max(selected - 1, 0);
                }
            }

            // Reset the cursor and return the selected option.
            Console.CursorVisible = true;
            return options[selected];
        }

        static void WriteUnderlined(string line)
        {
            Console.WriteLine(line);
            Console.WriteLine(new String('-', line.Length));
        }

        static string ReadString(string prompt)
        {
            Console.Write(prompt + ": ");
            return Console.ReadLine();
        }

        static int ReadInt(string prompt)
        {
            Console.Write(prompt + ": ");
            int? number = null;
            while (number == null)
            {
                string input = Console.ReadLine();
                try
                {
                    number = int.Parse(input);
                }
                catch
                {
                    Console.Write("Please enter a valid integer: ");
                }
            }

            return (int)number;
        }

        static bool ReadBool(string prompt)
        {
            Console.Write(prompt + ": ");
            bool? value = null;
            while (value == null)
            {
                string input = Console.ReadLine();
                if (input.ToUpper() == "Y")
                {
                    value = true;
                }
                else if (input.ToUpper() == "N")
                {
                    value = false;
                }
                else
                {
                    Console.Write("Please enter either Y or N: ");
                }
            }

            return (bool)value;
        }

        static DateTime ReadDate(string prompt)
        {
            Console.WriteLine(prompt + ": ");

            int year = ReadInt("- Year");
            int month = ReadInt("- Month (1-12)");
            int day = ReadInt("- Day (1-31)");

            DateTime date = new DateTime(year, month, day);
            return date;
        }
    }
}
