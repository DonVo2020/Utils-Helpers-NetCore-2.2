using ExternalFormatProcessing.Data;
using ExternalFormatProcessing.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace ExternalFormatProcessing
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ProductsShopContext context = new ProductsShopContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                ImportUsersFromJson(context);

                ImportProductsFromJson(context);

                ImportCategoriesFromJson(context);

                AssignCategoriesToProducts(context);

                ExportAllProductsInPriceRange(context);

                ExportAllUsersWithAtleastOneSoldItem(context);

                ImportUsersFromXml(context);

                ImportProductsFromXml(context);

                ExportToXmlAllProductsInPriceRange(context);
            }
        }

        static void ExportToXmlAllProductsInPriceRange(ProductsShopContext context)
        {
            var products = context.Products
                .Where(x => x.Price > 1000 && x.Price <= 2000 && x.BuyerId != null)
                .Select(x => new
                {
                    x.Name,
                    x.Price,
                    Buyer = $"{x.Buyer.FirstName} {x.Buyer.LastName}"
                })
                .OrderBy(x => x.Price)
                .ToArray();

            XDocument xDoc = new XDocument();

            xDoc.Add(new XElement("products"));

            foreach (var product in products)
            {
                xDoc.Root.Add(new XElement("product", new XAttribute("name", product.Name), new XAttribute("price", product.Price), new XAttribute("buyer", product.Buyer)));
            }

            string xmlOutput = xDoc.Root.ToString();

            EnsureExportedDataDirectoryCreation();

            File.WriteAllText("../../../ExportedData/products-in-range.xml", xmlOutput);
        }

        static void ImportProductsFromXml(ProductsShopContext context)
        {
            var xmlString = File.ReadAllText("../../../Resources/products.xml");

            var xDocument = XDocument.Parse(xmlString);

            var elements = xDocument.Root.Elements();

            List<Product> products = new List<Product>();

            int[] userIds = context.Users.Select(x => x.Id).ToArray();
            Random rand = new Random();

            foreach (var element in elements)
            {
                Product currentProduct = new Product()
                {
                    Name = element.Element("name").Value,
                    Price = decimal.Parse(element.Element("price").Value),
                    BuyerId = rand.Next(userIds.Min(), userIds.Max()),
                    SellerId = rand.Next(userIds.Min(), userIds.Max())
                };

                products.Add(currentProduct);
            }

            context.Products.AddRange(products);
            context.SaveChanges();
        }

        static void ImportUsersFromXml(ProductsShopContext context)
        {
            var xmlString = File.ReadAllText("../../../Resources/users.xml");

            var xDocument = XDocument.Parse(xmlString);

            var elements = xDocument.Root.Elements();

            List<User> users = new List<User>();

            foreach (var element in elements)
            {
                User currentUser = new User()
                {
                    FirstName = element.Attribute("firstName")?.Value,
                    LastName = element.Attribute("lastName").Value,
                    Age = null
                };

                if (element.Attribute("age") != null)
                {
                    currentUser.Age = int.Parse(element.Attribute("age").Value);
                }

                users.Add(currentUser);
            }

            context.Users.AddRange(users);
            context.SaveChanges();
        }

        static void ExportAllUsersWithAtleastOneSoldItem(ProductsShopContext context)
        {
            var users = context.Users
                .Where(x => x.ProductsSold.Any())
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    SoldProducts = x.ProductsSold
                    .Select(p => new
                    {
                        p.Name,
                        p.Price,
                        buyerFirstName = p.Buyer.FirstName,
                        buyerLastName = p.Buyer.LastName
                    })
                })
                .ToArray();

            string jsonOutput = JsonConvert.SerializeObject(users, Formatting.Indented,
                new JsonSerializerSettings
                {
                    DefaultValueHandling = DefaultValueHandling.Ignore
                });

            EnsureExportedDataDirectoryCreation();

            File.WriteAllText("../../../ExportedData/users-sold-products.json", jsonOutput);
        }

        static void ExportAllProductsInPriceRange(ProductsShopContext context)
        {
            var products = context.Products
                .Where(x => x.Price > 500 && x.Price <= 1000)
                .Select(x => new
                {
                    x.Name,
                    x.Price,
                    Seller = $"{x.Seller.FirstName} {x.Seller.LastName}"
                })
                .OrderBy(x => x.Price)
                .ToArray();

            string jsonOutput = JsonConvert.SerializeObject(products, Formatting.Indented,
                new JsonSerializerSettings
                {
                    DefaultValueHandling = DefaultValueHandling.Ignore
                });

            EnsureExportedDataDirectoryCreation();

            File.WriteAllText("../../../ExportedData/products-in-range.json", jsonOutput);
        }

        private static void EnsureExportedDataDirectoryCreation()
        {
            if (!Directory.Exists("../../../ExportedData"))
            {
                Directory.CreateDirectory("../../../ExportedData");
            }
        }

        static string AssignCategoriesToProducts(ProductsShopContext context)
        {
            int[] categoryIds = context.Categories.Select(x => x.Id).ToArray();
            int[] productIds = context.Products.Select(x => x.Id).ToArray();

            var categoryProducts = new List<CategoryProduct>();

            Random rand = new Random();

            foreach (var productId in productIds)
            {
                for (int i = 0; i < 3; i++)
                {
                    var categoryProduct = new CategoryProduct();
                    categoryProduct.ProductId = productId;

                    int categoryId = rand.Next(categoryIds.Min(), categoryIds.Max());
                    if (!categoryProducts.Any(x => x.ProductId == productId && x.CategoryId == categoryId))
                    {
                        categoryProduct.CategoryId = categoryId;
                        categoryProducts.Add(categoryProduct);
                    }
                    else
                    {
                        i--;
                    }
                }
            }

            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();

            return "Successfully set categories to products.";
        }

        static string ImportCategoriesFromJson(ProductsShopContext context)
        {
            Category[] categories = ImportJson<Category>("../../../Resources/categories.json");

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return "Successfully added categories to the database.";
        }

        static string ImportUsersFromJson(ProductsShopContext context)
        {
            User[] users = ImportJson<User>("../../../Resources/users.json");

            context.Users.AddRange(users);

            context.SaveChanges();

            return "Successfully added users to the database";
        }

        static string ImportProductsFromJson(ProductsShopContext context)
        {
            Product[] products = ImportJson<Product>("../../../Resources/products.json");

            Random rand = new Random();

            int[] userIds = context.Users.Select(x => x.Id).ToArray();

            foreach (var product in products)
            {
                product.BuyerId = rand.Next(userIds.Min(), userIds.Max());
                product.SellerId = rand.Next(userIds.Min(), userIds.Max());
            }

            context.Products.AddRange(products);
            context.SaveChanges();

            return "Successfully added products to the database";
        }

        static T[] ImportJson<T>(string path)
        {
            string JsonString = File.ReadAllText(path);

            T[] objects = JsonConvert.DeserializeObject<T[]>(JsonString);

            return objects;
        }
    }
}
