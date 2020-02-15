using AutoMapper;
using JSONProcessing.Data;
using JSONProcessing.DTO.Export;
using JSONProcessing.DTO.Import;
using JSONProcessing.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JSONProcessing
{
    public class Program
    {
        public static void Main()
        {
            //const string Root = @"C:\DEVELOPMENT\UTILS_HELPERS_PROJECTS\DonVo.UtilsHelpers2019\JSONProcessing\Datasets\";
            //const string CarsJson = "cars.json";
            //const string CustomersJson = "customers.json";
            //const string PartsJson = "parts.json";
            //const string SalesJson = "sales.json";
            //const string SuppliersJson = "suppliers.json";

            try
            {
                string path = "../../../Datasets/sales.json";
                string json = File.ReadAllText(path);

                Mapper.Initialize(configuration => configuration.AddProfile<CarDealerProfile>());

                using (var context = new CarDealerContext())
                {
                    string result = GetSalesWithAppliedDiscount(context);
                    Console.WriteLine(result);
                }


                //// If Database is not existed, then do these comments below first.
                //using (var context = new CarDealerContext())
                //{
                //    //Database initialize.
                //    context.Database.EnsureCreated();
                //    Console.WriteLine("Database created successfully!");

                //    //III.Import data
                //    //09.ImportSuppliers
                //    string result = ImportSuppliers(context, File.ReadAllText(Root + SuppliersJson));
                //    Console.WriteLine(result);

                //    //10.Import Parts
                //    result = ImportParts(context, File.ReadAllText(Root + PartsJson));
                //    Console.WriteLine(result);

                //    //11.Import Cars
                //    result = ImportCars(context, File.ReadAllText(Root + CarsJson));
                //    Console.WriteLine(result);

                //    //12.Import Customers
                //    result = ImportCustomers(context, File.ReadAllText(Root + CustomersJson));
                //    Console.WriteLine(result);

                //    //13.Import Sales
                //    result = ImportSales(context, File.ReadAllText(Root + SalesJson));
                //    Console.WriteLine(result);

                //    //IV.Query and Export Data
                //    //14.Export Ordered Customers
                //    result = GetOrderedCustomers(context);
                //    Console.WriteLine(result);

                //    //15.Export Cars From Make Toyota
                //    result = GetCarsFromMakeToyota(context);
                //    Console.WriteLine(result);

                //    //16.Export Local Suppliers
                //    result = GetLocalSuppliers(context);
                //    Console.WriteLine(result);

                //    //17.Export Cars With Their List Of Parts
                //    result = GetCarsWithTheirListOfParts(context);
                //    Console.WriteLine(result);

                //    //18.Export Total Sales By Customer
                //    result = GetTotalSalesByCustomer(context);
                //    Console.WriteLine(result);

                //    //19.Export Sales With Applied Discount
                //    result = GetSalesWithAppliedDiscount(context);
                //    Console.WriteLine(result);
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Take(10)
                .Select(s => new
                {
                    car = new
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TravelledDistance = s.Car.TravelledDistance
                    },
                    customerName = s.Customer.Name,
                    Discount = $"{s.Discount:F2}",
                    price = $"{s.Car.PartCars.Sum(p => p.Part.Price):F2}",
                    priceWithDiscount = $"{(s.Car.PartCars.Sum(p => p.Part.Price) - (s.Car.PartCars.Sum(p => p.Part.Price) * (s.Discount / 100))):F2}"
                })
                .ToList();

            var json = JsonConvert.SerializeObject(sales, Formatting.Indented);

            return json;
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(c => c.Sales.Any())
                .Select(c => new
                {
                    fullName = c.Name,
                    boughtCars = c.Sales.Count,
                    spentMoney = c.Sales.Sum(y => y.Car.PartCars.Sum(z => z.Part.Price))
                })
                .OrderByDescending(c => c.spentMoney)
                .ThenByDescending(c => c.boughtCars)
                .ToList();

            var json = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return json;
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(c => new
                {
                    car = new
                    {
                        Make = c.Make,
                        Model = c.Model,
                        TravelledDistance = c.TravelledDistance
                    },
                    parts = c.PartCars.Select(pc => new
                    {
                        Name = pc.Part.Name,
                        Price = $"{pc.Part.Price:f2}"
                    })
                    .ToList()
                })
                .ToList();

            var json = JsonConvert.SerializeObject(cars, Formatting.Indented);

            return json;
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count
                })
                .ToList();

            var json = JsonConvert.SerializeObject(suppliers, Formatting.Indented);

            return json;
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var toyotaCars = context.Cars
                .Where(c => c.Make == "Toyota")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .Select(c => new
                {
                    c.Id,
                    c.Make,
                    c.Model,
                    c.TravelledDistance
                })
                .ToList();

            var json = JsonConvert.SerializeObject(toyotaCars, Formatting.Indented);

            return json;
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .Select(c => new
                {
                    Name = c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy"),
                    IsYoungDriver = c.IsYoungDriver
                })
                .ToList();

            var json = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return json;
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var sales = JsonConvert.DeserializeObject<Sale[]>(inputJson);

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Length}.";
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customers = JsonConvert.DeserializeObject<Customer[]>(inputJson);

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Length}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var cars = JsonConvert.DeserializeObject<CarImport[]>(inputJson);
            var mappedCars = new HashSet<Car>();

            foreach (var car in cars)
            {
                var mappedCar = new Car()
                {
                    Make = car.Make,
                    Model = car.Model,
                    TravelledDistance = car.TravelledDistance
                };


                var partsIds = car.PartsId.Distinct().ToHashSet();

                if (partsIds != null)
                {
                    foreach (var partId in partsIds)
                    {
                        var partCar = new PartCar()
                        {
                            CarId = car.Id,
                            PartId = partId
                        };

                        mappedCar.PartCars.Add(partCar);
                    }
                }

                mappedCars.Add(mappedCar);
            }

            context.Cars.AddRange(mappedCars);
            context.SaveChanges();

            return $"Successfully imported {mappedCars.Count}.";
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var existingSuppliers = context.Suppliers
                .Select(s => s.Id)
                .ToArray();

            var parts = JsonConvert.DeserializeObject<Part[]>(inputJson)
                .Where(p => existingSuppliers.Contains(p.SupplierId))
                .ToArray();

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Length}.";
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var suppliers = JsonConvert.DeserializeObject<Supplier[]>(inputJson);

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Length}.";
        }
    }
}
