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
        public DbSet<Category> Category { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(@"Data Source=(local)\SQLEXPRESS;Initial Catalog=BulletineBoard;Integrated Security=True");
        }
    }

    public class Account
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [MaxLength(10)]
        public string Username { get; set; }
        [Required]
        [MinLength(5)]
        [MaxLength(15)]
        public string Password { get; set; }
    
    }

    public class Post
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public Category Category { get; set; }
        [MaxLength(100)]
        [Required]
        public string Name { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public Account Account { get; set; }
        public int? Like { get; set; }
        public DateTime Date { get; set; }
    }

    public class Category
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }

    class Program
    {
        static AppDbContext database;
        static Account logedInUser;

        static void Main(string[] args)
        {
            using (database = new AppDbContext())
            {
                while (true)
                {
                    WriteUnderlined("Welcome to Bulletin Bored - for when you've got nothing better to do!");
                    string option = ShowMenu("", new[] {
                        "Sign In",
                        "Create Account",
                        "Quit"
                    });
                    Console.Clear();

                    if (option == "Sign In") SignIn();
                    else if (option == "Create Account") CreateAccount();
                    else Environment.Exit(0);

                    Console.WriteLine();
                }
            }
        }

        static void SignIn()
        {
            Account account = new Account();
            account.Username = ReadString("Write your username");

            Account[] users = database.Account.ToArray();

            if (users.Select(u => u.Username).Contains(account.Username))
            {
                Account selectedUser = users.First(u => u.Username == account.Username);

                account.Password  = ReadString("Enter password");

                if(account.Password == selectedUser.Password)
                {
                    logedInUser = selectedUser;
                    Console.Clear();
                    Console.WriteLine("You are now logged in as " + account.Username);
                    Console.WriteLine();
                    MainMenu();
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("The password is not korrect. Please try again or create a new account!");
                }
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("You don't have an account. Create a new one or sign in with another username!");
            }            
        }

        static void CreateAccount()
        {
            WriteUnderlined("Create new account");

            Account account = new Account();
            account.Username = ReadString("Choose username");
            Account[] users = database.Account.ToArray();
            if (users.Select(u => u.Username).Contains(account.Username))
            {
                Console.WriteLine();
                Console.WriteLine("That username is already taken. Please choose another username");
                Console.WriteLine();
                CreateAccount();
            }
            account.Password = ReadString("Choose password");
            
            database.Add(account);
            database.SaveChanges();
            Console.Clear();
            Console.WriteLine("You are now logged in as " + account.Username);
            Console.WriteLine();
            MainMenu();
        }

        static void MainMenu()
        {
            WriteUnderlined("Main menu");
            string option = ShowMenu("Choose what to do?", new[] {
                "Most Recent Posts",
                "Most Popular Posts",
                "Posts by Category",
                "Search",
                "Create a Post",
                "Delete a Post",
                "Quit"
                });
            Console.Clear();

            if (option == "Most Recent Posts") MostRecentPosts();
            else if (option == "Most Popular Posts") MostPopularPosts();
            else if (option == "Posts by Category") PostsByCategory();
            else if (option == "Search") Search();
            else if (option == "Create a Post") CreatePost();
            else if (option == "Delete a Post") DeletePost();
            else Environment.Exit(0);

            Console.WriteLine();
        }

        private static void DeletePost()
        {
            WriteUnderlined("Delete posts created by you");
            Console.WriteLine();

            string[] postsByLogedInUser = database.Post.Include(p => p.Account).Where(p => p.Account.Username == logedInUser.Username).Select(p => p.Name).ToArray();
            string postNameToDelete = ShowMenu("Which post do you want to delete?", postsByLogedInUser);
            var postToDelete = database.Post.Include(p => p.Account).Include(p => p.Category).First(p => p.Name == postNameToDelete);

            database.Remove(postToDelete);
            database.SaveChanges();

            Console.Clear();
            Console.WriteLine($"\"{postNameToDelete}\" succesfully deleted!");
            Console.WriteLine();
            MainMenu();
        }

        private static void CreatePost()
       {
            database.Post.Include(p => p.Category);
            database.Post.Include(p => p.Account);

            Post post = new Post();

            string[] categories = database.Category.Select(c => c.Name).ToArray();
            string selectedCategoryName = ShowMenu("Select category", categories);
            Console.WriteLine();

            Category selectedCategory = database.Category.First(c => c.Name == selectedCategoryName);

            post.Category = selectedCategory;
            post.Account = logedInUser;
            post.Name = ReadString("Write title");
            post.Content = ReadString("Write text");
            post.Like = 0;
            post.Date = DateTime.Now;

            database.Add(post);
            database.SaveChanges();
            Console.Clear();
            Console.WriteLine("The post is added.");
            Console.WriteLine();
            MainMenu();
        }

        private static void Search()
        {
            if (database.Post.Count() == 0)
            {
                Console.WriteLine("There are no posts in the database.");
            }
            else
            {
                WriteUnderlined("Search");
                Console.WriteLine();
                string searchWord = ReadString("Word or phrase");
                Console.Clear();

                WriteUnderlined("Post containing \"" + searchWord + "\"");
                Post[] contents = database.Post.Where(p => p.Content.Contains(searchWord)).ToArray();

                if(contents.Count() == 0)
                {
                    Console.WriteLine("There were no posts containing \"" + searchWord + "\"");
                }
                else
                {
                    foreach (var c in contents)
                    {
                        Console.WriteLine(c.Content);
                    }
                }                
            }
            Console.WriteLine();
            MainMenu();
        }

        private static void PostsByCategory()
        {
            WriteUnderlined("Select Category");

            Category category = new Category();

            string[] postCategories = database.Category.Select(c => c.Name).ToArray();

            postCategories = postCategories.Concat(new string[] { "Return to Main Menu" }).ToArray();
            string option = ShowMenu("", postCategories);
            Console.Clear();

            if (option == "Return to Main Menu") MainMenu();

            string[] postNames = database.Post
                .Where(p => p.Category.Name == option)
                .Select(p => p.Name)
                .ToArray();
            postNames = postNames.Concat(new string[] { "Return to Category", "Return to Main Menu" }).ToArray();

            WriteUnderlined("Posts in Category \"" + option + "\"");
            string selectedPostName = ShowMenu("", postNames);
            Console.Clear();

            if (selectedPostName == "Return to Category") PostsByCategory();
            else if (selectedPostName == "Return to Main Menu") MainMenu();

            Post selectedPost = database.Post.First(p => p.Name == selectedPostName);
            WriteUnderlined("Post in \"" + selectedPostName + "\"");
            Console.WriteLine();
            Console.WriteLine(selectedPost.Content);
            Console.WriteLine();

            string option2 = ShowMenu("", new[] {
                    "Like this Post",
                    "Return to Category",
                    "Return to Main Menu"
                });
            Console.Clear();

            if (option2 == "Like this Post") LikeThisPost(selectedPost);
            else if (option2 == "Return to Category") PostsByCategory();
            else if (option2 == "Return to Main Menu") MainMenu();
        }

        private static void MostPopularPosts()
        {
            if (database.Post.Count() == 0)
            {
                Console.WriteLine("There are no posts in the database.");
            }
            else
            {
                WriteUnderlined("Most Popular Posts");

                Post[] posts = database.Post.OrderBy(p => p.Like).ToArray();
                posts = posts.Reverse().ToArray();  //Reverse puts most Likes first
                foreach (var p in posts)
                {
                    Console.WriteLine("Likes " + p.Like + "; Name: \"" + p.Name + "\" ; Content: " + p.Content);
                }
            }
            Console.WriteLine();
            MainMenu();
        }

        private static void MostRecentPosts()
        {
            if (database.Post.Count() == 0)
                {
                    Console.WriteLine("There are no posts in the database.");
                }
            else while (true)
                {
                    WriteUnderlined("Most Recent Posts");
                    var postNames = database.Post.Select(p => p.Name).ToArray();

                    //"Reverse" puts newest Post first. "Return to Main Menu" added to postNames-array
                    postNames = postNames.Reverse().Concat(new string[] { "Return to Main Menu" }).ToArray();
                    string postName = ShowMenu("", postNames);
                    Console.Clear();
                    if (postName == "Return to Main Menu")
                    {
                        MainMenu();
                    }

                    WriteUnderlined(postName);
                    Console.WriteLine(database.Post.First(p => p.Name == postName).Content);
                    Console.WriteLine();

                    database.Post.Include(p => p.Category).ToArray();
                    database.Post.Include(p => p.Account).ToArray();
                    Post post = new Post();
                    post = database.Post.First(p => p.Name == postName);

                    Console.WriteLine("Posted by " + post.Account.Username + " in Category \"" + post.Category.Name
                        + "\" at " + post.Date.Hour + ":" + post.Date.Minute + " (" + post.Like + " likes)");

                    string option = ShowMenu("", new[] {
                        "Like this Post",
                        "Return to List",
                        "Return to Main Menu"
                    });
                    Console.Clear();

                    if (option == "Like this Post") LikeThisPost(post);
                    else if (option == "Return to Main Menu") MainMenu();
                }
            Console.WriteLine();
            MainMenu();
        }

        private static void LikeThisPost(Post post)
        {
            post.Like++;
            database.Update(post);
            database.SaveChanges();
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
    }
}
