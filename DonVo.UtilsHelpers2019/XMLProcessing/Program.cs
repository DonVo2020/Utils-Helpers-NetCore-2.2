using AutoMapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using XMLProcessing.Data;
using XMLProcessing.Dtos.Export;
using XMLProcessing.Dtos.Import;
using XMLProcessing.Models;

namespace XMLProcessing
{
    public class Program
    {
        public static void Main()
        {
            //const string Root = @"../../../Datasets/";
            //const string CarsXml = "cars.xml";
            //const string CustomersXml = "customers.xml";
            //const string PartsXml = "parts.xml";
            //const string SalesXml = "sales.xml";
            //const string SuppliersXml = "suppliers.xml";

            //Mapper.Initialize(cfg =>
            //{
            //    cfg.AddProfile<CarDealerProfile>();
            //});

            try
            {
                Mapper.Initialize(x => x.AddProfile<CarDealerProfile>());

                using (var context = new CarDealerContext())
                {
                    string result = GetSalesWithAppliedDiscount(context);
                    Console.WriteLine(result);
                }

                using (var context = new CarDealerContext())
                {
                    ////Database initialize.
                    //context.Database.EnsureCreated();
                    //Console.WriteLine("Database created successfully!");

                    ////III.Import data
                    // //09.ImportSuppliers
                    //string result = ImportSuppliers(context, File.ReadAllText(Root + SuppliersXml));
                    //Console.WriteLine(result);

                    ////10.Import Parts
                    //result = ImportParts(context, File.ReadAllText(Root + PartsXml));
                    //Console.WriteLine(result);

                    ////11.Import Cars
                    //result = ImportCars(context, File.ReadAllText(Root + CarsXml));
                    //Console.WriteLine(result);

                    ////12.Import Customers
                    //result = ImportCustomers(context, File.ReadAllText(Root + CustomersXml));
                    //Console.WriteLine(result);

                    ////13.Import Sales
                    //result = ImportSales(context, File.ReadAllText(Root + SalesXml));
                    //Console.WriteLine(result);

                    ////IV.Query and Export Data
                    ////14.Export Cars With Distance
                    //result = GetCarsWithDistance(context);
                    //Console.WriteLine(result);

                    ////15.Export Cars From Make BMW
                    //result = GetCarsFromMakeBmw(context);
                    //Console.WriteLine(result);

                    ////16.Export Local Suppliers
                    //result = GetLocalSuppliers(context);
                    //Console.WriteLine(result);

                    ////17.Export Cars With Their List Of Parts
                    //string result = GetCarsWithTheirListOfParts(context);
                    //Console.WriteLine(result);

                    ////18.Export Total Sales By Customer
                    //result = GetTotalSalesByCustomer(context);
                    //Console.WriteLine(result);

                    ////19.Export Sales With Applied Discount
                    //result = GetSalesWithAppliedDiscount(context);
                    //Console.WriteLine(result);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportSuppliersDto[]), new XmlRootAttribute("Suppliers"));

            var suppliersDto = (ImportSuppliersDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var suppliers = new List<Supplier>();

            foreach (var supplierDto in suppliersDto)
            {
                var supplier = Mapper.Map<Supplier>(supplierDto);

                suppliers.Add(supplier);
            }

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}";
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            var existingSuppliers = context.Suppliers
                .Select(s => s.Id)
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportPartDto[]), new XmlRootAttribute("Parts"));

            var partsDto = (ImportPartDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var parts = new List<Part>();

            foreach (var partDto in partsDto)
            {
                if (existingSuppliers.Contains(partDto.SupplierId) == false)
                {
                    continue;
                }

                var part = Mapper.Map<Part>(partDto);

                parts.Add(part);
            }

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count}";
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCarDto[]), new XmlRootAttribute("Cars"));

            var carsDto = (ImportCarDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var cars = new List<Car>();
            var partCars = new List<PartCar>();

            var partsId = context.Parts.Select(p => p.Id).ToList();

            foreach (var carDto in carsDto)
            {
                var car = Mapper.Map<Car>(carDto);

                var partsDtoId = carDto.Parts.Select(x => x.Id).Distinct().ToHashSet();

                foreach (var partDtoId in partsDtoId)
                {
                    if (partsId.Contains(partDtoId) == false)
                    {
                        continue;
                    }

                    var partCar = new PartCar()
                    {
                        CarId = car.Id,
                        PartId = partDtoId
                    };

                    car.PartCars.Add(partCar);
                }

                cars.Add(car);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCustomerDto[]), new XmlRootAttribute("Customers"));

            var customersDto = (ImportCustomerDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var customers = new List<Customer>();

            foreach (var customerDto in customersDto)
            {
                var customer = Mapper.Map<Customer>(customerDto);

                customers.Add(customer);
            }

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}";
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportSaleDto[]), new XmlRootAttribute("Sales"));

            var salesDto = (ImportSaleDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var sales = new List<Sale>();

            var carsId = context.Cars.Select(x => x.Id).ToList();

            foreach (var saleDto in salesDto)
            {
                if (carsId.Contains(saleDto.CarId) == false)
                {
                    continue;
                }

                var sale = Mapper.Map<Sale>(saleDto);

                sales.Add(sale);
            }

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}";
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.TravelledDistance > 2000000)
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Select(c => new ExportCarWithDistanceDto()
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .Take(10)
                .ToArray();

            var sb = new StringBuilder();

            var serializer = new XmlSerializer(typeof(ExportCarWithDistanceDto[]), new XmlRootAttribute("cars"));

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            serializer.Serialize(new StringWriter(sb), cars, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var bmvCars = context.Cars
                .Where(c => c.Make == "BMW")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .Select(c => new ExportBmvCarDto
                {
                    Id = c.Id,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .ToArray();

            var sb = new StringBuilder();

            var serializer = new XmlSerializer(typeof(ExportBmvCarDto[]), new XmlRootAttribute("cars"));

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            serializer.Serialize(new StringWriter(sb), bmvCars, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new NativeSupplierDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count
                })
                .ToArray();

            var sb = new StringBuilder();

            var serializer = new XmlSerializer(typeof(NativeSupplierDto[]), new XmlRootAttribute("suppliers"));

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            serializer.Serialize(new StringWriter(sb), suppliers, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .OrderByDescending(c => c.TravelledDistance)
                .ThenBy(c => c.Model)
                .Select(c => new ExportCarWithPartsDto
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance,
                    Parts = c.PartCars.Select(pc => new ExportPartDto
                    {
                        Name = pc.Part.Name,
                        Price = pc.Part.Price
                    })
                    .OrderByDescending(x => x.Price)
                    .ToArray()
                })
                .Take(5)
                .ToArray();

            var sb = new StringBuilder();

            var serializer = new XmlSerializer(typeof(ExportCarWithPartsDto[]), new XmlRootAttribute("cars"));

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            serializer.Serialize(new StringWriter(sb), cars, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(c => c.Sales.Any())
                .Select(c => new CustomerDto
                {
                    FullName = c.Name,
                    BoughtCars = c.Sales.Count,
                    SpentMoney = c.Sales.Sum(y => y.Car.PartCars.Sum(z => z.Part.Price))
                })
                .OrderByDescending(c => c.SpentMoney)
                .ToArray();

            var sb = new StringBuilder();

            var serializer = new XmlSerializer(typeof(CustomerDto[]), new XmlRootAttribute("customers"));

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            serializer.Serialize(new StringWriter(sb), customers, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Select(s => new ExportSaleWithDiscountDto()
                {
                    Car = new ExportCarDto()
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TravelledDistance = s.Car.TravelledDistance
                    },
                    Discount = s.Discount,
                    CustomerName = s.Customer.Name,
                    Price = $"{s.Car.PartCars.Sum(p => p.Part.Price)}",
                    PriceWithDiscount = $"{(s.Car.PartCars.Sum(p => p.Part.Price) - (s.Car.PartCars.Sum(p => p.Part.Price) * (s.Discount / 100)))}".TrimEnd('0')
                })
                .ToArray();

            var sb = new StringBuilder();

            var serializer = new XmlSerializer(typeof(ExportSaleWithDiscountDto[]), new XmlRootAttribute("sales"));

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            serializer.Serialize(new StringWriter(sb), sales, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}
