using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace lab5
{
    class Program
    {
        static List<string> favorites = new List<string>();
        static string favoritesFilePath = "C:\\Users\\willi\\OneDrive\\Desktop\\Server Side Web\\lab5\\lab5\\favoritess.txt";

        static async Task Main(string[] args)
        {
            LoadFavorites(); // Load favorites from file

            while (true)
            {
                Console.WriteLine("1. Search for a quote by author");
                Console.WriteLine("2. Search for a random quote");
                Console.WriteLine("3. View Favorites");
                Console.WriteLine("4. Exit");
                Console.Write("Select an option: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await SearchByAuthor();
                        break;
                    case "2":
                        await SearchRandomQuote();
                        break;
                    case "3":
                        ViewFavorites();
                        break;
                    case "4":
                        SaveFavorites(); // Save favorites to file before exiting
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        static async Task SearchByAuthor()
        {
            Console.Write("Enter the name of the author (or 'random' for a random author): ");
            string authorName = Console.ReadLine();

            if (authorName.ToLower() == "random")
            {
                authorName = await GetRandomAuthor();
            }

            using (HttpClient client = new HttpClient())
            {
                string url = $"https://api.quotable.io/quotes?author={authorName}";
                HttpResponseMessage response = await client.GetAsync(url);
                string responseBody = await response.Content.ReadAsStringAsync();

                // Parse JSON response to extract the quote
                JsonDocument json = JsonDocument.Parse(responseBody);

                // Display only one quote when searching by author
                JsonElement quote = json.RootElement.GetProperty("results")[0];
                Console.WriteLine(quote.GetProperty("content").GetString());
                Console.WriteLine($"- {quote.GetProperty("author").GetString()}");

                AskToAddToFavorites(quote.GetProperty("content").GetString() + " - " + quote.GetProperty("author").GetString());
            }
        }

        static async Task<string> GetRandomAuthor()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://api.quotable.io/authors";
                HttpResponseMessage response = await client.GetAsync(url);
                string responseBody = await response.Content.ReadAsStringAsync();

                // Parse JSON response to extract a random author
                JsonDocument json = JsonDocument.Parse(responseBody);
                JsonElement authors = json.RootElement.GetProperty("results");
                Random random = new Random();
                int index = random.Next(authors.GetArrayLength());
                return authors[index].GetProperty("name").GetString();
            }
        }

        static async Task SearchRandomQuote()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://api.quotable.io/random";
                HttpResponseMessage response = await client.GetAsync(url);
                string responseBody = await response.Content.ReadAsStringAsync();

                // Parse JSON response to extract the quote
                JsonDocument json = JsonDocument.Parse(responseBody);
                JsonElement quote = json.RootElement;
                Console.WriteLine(quote.GetProperty("content").GetString());
                Console.WriteLine($"- {quote.GetProperty("author").GetString()}");

                AskToAddToFavorites(quote.GetProperty("content").GetString() + " - " + quote.GetProperty("author").GetString());
            }
        }

        static void AskToAddToFavorites(string quote)
        {
            Console.Write("Do you want to add this quote to favorites? (Y/N): ");
            string addToFavorites = Console.ReadLine().ToUpper();

            if (addToFavorites == "Y")
            {
                favorites.Add(quote);
                Console.WriteLine("Quote added to favorites!");
            }
            else if (addToFavorites != "N")
            {
                Console.WriteLine("Invalid input. Quote not added to favorites.");
            }
        }

        static void ViewFavorites()
        {
            Console.WriteLine("Favorite Quotes:");
            foreach (string quote in favorites)
            {
                Console.WriteLine("=====");
                Console.WriteLine(quote);
                Console.WriteLine("=====");
            }
        }

        static void LoadFavorites()
        {
            if (System.IO.File.Exists(favoritesFilePath))
            {
                favorites = new List<string>(System.IO.File.ReadAllLines(favoritesFilePath));
            }
        }

        static void SaveFavorites()
        {
            System.IO.File.WriteAllLines(favoritesFilePath, favorites);
        }
    }
}
