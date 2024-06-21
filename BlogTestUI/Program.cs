using BlogDataLibrary.Data;
using BlogDataLibrary.Database;
using BlogDataLibrary.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;
using System.Reflection;

namespace BlogTestUI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SqlData db = GetConnection();
            AddPost(db);
            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }

        static SqlData GetConnection()
        {
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");
            IConfiguration config = builder.Build();
            ISqlDataAccess dbAccess = new SqlDataAccess(config);
            SqlData db = new SqlData(dbAccess);

            return db;
        }

        private static UserModel GetCurrentUser(SqlData db)
        {
            Console.Write("Username: ");
            string user = Console.ReadLine();
            Console.Write("Password: ");
            string pass = Console.ReadLine();

            UserModel u = db.Authenticate(user, pass);
            return u;
        }

        public static void Authenticate(SqlData db)
        {
            UserModel u = GetCurrentUser(db);
            if (u == null)
            {
                Console.WriteLine("Invalid credentials.");
            }
            else
            {
                Console.WriteLine($"Welcome, {u.UserName}");
            }
        }

        public static void Register(SqlData db)
        {
            Console.Write("Enter new username: ");
            string user = Console.ReadLine();
            Console.Write("Enter new password: ");
            string pass = Console.ReadLine();
            Console.Write("Enter first name: ");
            string first = Console.ReadLine();
            Console.Write("Enter last name: ");
            string last = Console.ReadLine();

            db.Register(user, first, last, pass);
        }

        public static void AddPost(SqlData db)
        {
            UserModel user = GetCurrentUser(db);

            Console.Write("Title: ");
            string title = Console.ReadLine();
            Console.Write("Write body: ");
            string body = Console.ReadLine();

            PostModel post = new PostModel
            {
                Title = title,
                Body = body,
                DateCreated = DateTime.Now,
                UserId = user.Id
            };

            db.AddPost(post);
        }

        private static void ListPosts(SqlData db)
        {
            List<ListPostModel> posts = db.ListPosts();
            foreach (ListPostModel p in posts)
            {
                Console.WriteLine($"{p.Id}. Title: {p.Title} by {p.UserName} [{p.DateCreated.ToString("yyyy-MM-dd")}]");
                Console.WriteLine($"{p.Body.Substring(0, 20)}...");
                Console.WriteLine();
            }
        }

        private static void ShowPostDetails(SqlData db)
        {
            Console.Write("Enter a post ID: ");
            int id = Int32.Parse(Console.ReadLine());

            ListPostModel p = db.ShowPostDetails(id);
            Console.WriteLine(p.Title);
            Console.WriteLine($"by {p.FirstName} {p.LastName} [{p.UserName}]");
            Console.WriteLine();
            Console.WriteLine(p.Body);
            Console.WriteLine(p.DateCreated);
        }
    }
}
