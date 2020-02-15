using AdvancedQuerying.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Z.EntityFramework.Plus;

namespace AdvancedQuerying
{
    public class Program
    {
        public static void Main()
        {
            using (var db = new BookShopContext())
            {
                DbInitializer.ResetDatabase(db);
                Console.Write("Please key either Minor,Teen, or Adult and hit Enter: ");
                string command = Console.ReadLine();
                string bookTitles = GetBooksByAgeRestriction(db, command);
                Console.WriteLine(bookTitles);

                var goldenBooks = GetGoldenBooks(db);
                Console.WriteLine(goldenBooks);

                var booksByPrice = GetBooksByPrice(db);
                Console.WriteLine(booksByPrice);

                Console.Write("\n\nPlease key Year only and hit Enter: ");
                int skipYear = int.Parse(Console.ReadLine());
                Console.WriteLine(GetBooksNotReleasedIn(db, skipYear));

                Console.Write("\n\nPlease key either Action/History/Travel ... from Categories and hit Enter: ");
                string categoriesInput = Console.ReadLine();
                Console.WriteLine(GetBooksByCategory(db, categoriesInput));

                Console.Write("\n\nPlease key Date Released (01-01-2018) and hit Enter: ");
                string dateInput = Console.ReadLine();
                Console.WriteLine(GetBooksReleasedBefore(db, dateInput));

                Console.Write("\n\nPlease key FirstName at least 1 letter and hit Enter: ");
                string firstNameEndingInput = Console.ReadLine();
                Console.WriteLine(GetAuthorNamesEndingIn(db, firstNameEndingInput));

                Console.Write("\n\nPlease key Title at least 1 letter and hit Enter: ");
                string partialBookTitleInput = Console.ReadLine();
                Console.WriteLine(GetBookTitlesContaining(db, partialBookTitleInput));

                Console.Write("\n\nPlease key Author LastName at least 1 letter and hit Enter: ");
                string authorsLastNameStartInput = Console.ReadLine();
                Console.WriteLine(GetBooksByAuthor(db, authorsLastNameStartInput));

                Console.Write("\n\nPlease key Title Length between 2 and 50 and hit Enter: ");
                int bookCountInput = int.Parse(Console.ReadLine());
                Console.WriteLine(CountBooks(db, bookCountInput));

                Console.WriteLine(CountCopiesByAuthor(db));

                Console.WriteLine(GetTotalProfitByCategory(db));

                Console.WriteLine(GetMostRecentBooks(db));

                IncreasePrices(db);

                int booksDeleted = RemoveBooks(db);
                Console.WriteLine($"{booksDeleted} books were deleted.");
            }
        }

        public static int RemoveBooks(BookShopContext context)
        {
            int numberOfBooksToBeDeleted = context.Books
               .Where(x => x.Copies < 4200)
               .Count();

            context.Books
                .Where(x => x.Copies < 4200)
                .Delete();

            return numberOfBooksToBeDeleted;
        }
        public static void IncreasePrices(BookShopContext context)
        {
            context.Books
                .Where(x => x.ReleaseDate.Value.Year < 2010)
                .Update(x => new Book { Price = x.Price + 5 });
        }
        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categories = context.Categories
                .OrderBy(x => x.Name)
                .Select(x => new
                {
                    x.Name,
                    SortedBooks = x.CategoryBooks
                                    .OrderByDescending(b => b.Book.ReleaseDate)
                                    .Take(3)
                                    .Select(b => new
                                    {
                                        b.Book.Title,
                                        b.Book.ReleaseDate
                                    })
                                    .ToArray()
                })
                .ToArray();

            StringBuilder builder = new StringBuilder();

            foreach (var c in categories)
            {
                builder.AppendLine($"--{c.Name}");

                foreach (var b in c.SortedBooks)
                {
                    builder.AppendLine($"{b.Title} ({b.ReleaseDate.Value.Year})");
                }
            }

            return builder.ToString().Trim();
        }
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categoryProfits = context.Categories
                .Select(x => new
                {
                    x.Name,
                    TotalProfit = x.CategoryBooks.Sum(b => b.Book.Copies * b.Book.Price)
                })
                .OrderByDescending(x => x.TotalProfit)
                .ThenBy(x => x.Name)
                .ToArray();

            StringBuilder builder = new StringBuilder();

            foreach (var cp in categoryProfits)
            {
                builder.AppendLine($"{cp.Name} ${cp.TotalProfit:f2}");
            }

            return builder.ToString().Trim();
        }
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authors = context.Authors
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    CopiesCount = x.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(x => x.CopiesCount)
                .ToArray();

            StringBuilder builder = new StringBuilder();

            foreach (var a in authors)
            {
                builder.AppendLine($"{a.FirstName} {a.LastName} - {a.CopiesCount}");
            }

            return builder.ToString().Trim();
        }
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            int bookCount = context.Books
                .Where(x => x.Title.Length > lengthCheck)
                .Count();

            return bookCount;
        }
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var books = context.Books
                .Where(x => EF.Functions.Like(x.Author.LastName, $"{input}%"))
                .OrderBy(x => x.BookId)
                .Select(x => $"{x.Title} ({x.Author.FirstName} {x.Author.LastName})")
                .ToArray();

            return string.Join(Environment.NewLine, books);
        }
        // EFC Version
        public static string GetBookTitlesContainingEFC(BookShopContext context, string input)
        {
            var bookTitles = context.Books
                .Where(x => EF.Functions.Like(x.Title, $"%{input}%"))
                .Select(x => x.Title)
                .OrderBy(x => x)
                .ToArray();

            return string.Join(Environment.NewLine, bookTitles);
        }
        // .Net Core version
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var bookTitles = context.Books
                .Where(x => x.Title.ToLower().Contains(input.ToLower()))
                .Select(x => x.Title)
                .OrderBy(x => x)
                .ToArray();

            return string.Join(Environment.NewLine, bookTitles);
        }
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var books = context.Authors
                .Where(x => x.FirstName.EndsWith(input))
                .Select(x => $"{x.FirstName} {x.LastName}")
                .OrderBy(x => x)
                .ToArray();

            return string.Join(Environment.NewLine, books);
        }
        // Anonymous object and SB
        public static string GetBooksReleasedBeforeSB(BookShopContext context, string date)
        {
            var books = context.Books
                .Where(x => x.ReleaseDate < DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture))
                .OrderByDescending(x => x.ReleaseDate)
                .Select(x => new { x.Title, x.EditionType, x.Price })
                .ToArray();

            StringBuilder builder = new StringBuilder();

            foreach (var b in books)
            {
                builder.AppendLine($"{b.Title} - {b.EditionType} - ${b.Price:f2}");
            }

            return builder.ToString().Trim();
        }
        // Smarter Select
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var books = context.Books
                .Where(x => x.ReleaseDate < DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture))
                .OrderByDescending(x => x.ReleaseDate)
                .Select(b => $"{b.Title} - {b.EditionType} - ${b.Price:f2}")
                .ToArray();

            return string.Join(Environment.NewLine, books);
        }
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            string[] categories = input
                .Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                .ToArray();

            var books = context.Books.
                Where(x => x.BookCategories.Any(y => categories.Contains(y.Category.Name.ToLower())))
                .Select(x => x.Title)
                .OrderBy(x => x)
                .ToArray();

            return string.Join(Environment.NewLine, books);
        }
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context.Books
                .Where(x => x.ReleaseDate.Value.Year != year)
                .OrderBy(x => x.BookId)
                .Select(x => x.Title)
                .ToArray();

            return String.Join(Environment.NewLine, books);
        }
        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books
                .Where(x => x.Price > 40)
                .OrderByDescending(x => x.Price)
                .Select(x => $"{x.Title} - ${x.Price:f2}")
                .ToArray();

            return String.Join(Environment.NewLine, books);
        }
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var ageRestrictionCommand = (AgeRestriction)Enum.Parse(typeof(AgeRestriction), command, true);

            var books = context.Books
                .Where(x => x.AgeRestriction == ageRestrictionCommand)
                .Select(x => x.Title)
                .OrderBy(x => x)
                .ToArray();

            return string.Join(Environment.NewLine, books);
        }
        public static string GetGoldenBooks(BookShopContext context)
        {
            var books = context.Books
                .Where(x => x.EditionType == EditionType.Gold && x.Copies < 5000)
                .OrderBy(x => x.BookId)
                .Select(x => x.Title)
                .ToArray();

            return string.Join(Environment.NewLine, books);
        }
    }
}
