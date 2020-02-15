using AutoMapper;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace AutoMapperSamples
{
    class Program
    {
        [Obsolete]
        static void Main(string[] args)
        {
            //BasicMap();
            FlatMap();
            //IgnoreMap();
            //CustomMap();
            //MultilayerMap();
            //ValueResolverMap();
            //DynamicMap();

            Console.ReadKey();
        }

        [Obsolete]
        private static void BasicMap()
        {
            Mapper.Initialize(cfg => cfg.CreateMap<User, UserDto>());

            var user = new User { Id = 1, Age = 18, Name = "Colin" };
            var userDto = Mapper.Map<User, UserDto>(user);
        }

        [Obsolete]
        private static void FlatMap()
        {
            Mapper.Initialize(cfg => cfg.CreateMap<Employee, EmployeeDto>());

            var employee = new Employee
            {
                Name = "Colin",
                Company = new Company
                {
                    Name = "Chanyi",
                    Address = "Beijing"
                }
            };
            var employeeDto = Mapper.Map<Employee, EmployeeDto>(employee);
        }

        [Obsolete]
        private static void IgnoreMap()
        {
            Mapper.Initialize(cfg => cfg.CreateMap<User, UserDto>()
                .ForMember(d => d.Age, o => o.Ignore())); //Ignore the Age attribute map

            var user = new User { Id = 1, Age = 18, Name = "Colin" };
            var userDto = Mapper.Map<User, UserDto>(user);
        }

        [Obsolete]
        private static void CustomMap()
        {
            Mapper.Initialize(cfg => cfg.CreateMap<Article, ArticleDto>()
                .ForMember(d => d.Category, o => o.MapFrom(s => s.TypeName))
                .ForMember(d => d.Comments, o => o.MapFrom(s => s.Messages))
            );

            var article = new Article
            { Id = 0, Content = "content", TypeName = "fiction", Messages = new[] { "Good" } };
            var articleDto = Mapper.Map<Article, ArticleDto>(article);
        }

        [Obsolete]
        private static void MultilayerMap()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Order, OrderDto>();
                cfg.CreateMap<Customer, CustomerDto>()
                    .ForMember(d => d.OrderDtos, o => o.MapFrom(s => s.Orders));
            });

            var customer = new Customer()
            {
                Id = 0,
                Name = "Colin",
                Orders = new List<Order>
                {
                    new Order()
                    {
                        Id = 0,
                        TotalFee = 10,
                        TradeNo = "123456"
                    }
                }
            };
            var customerDto = Mapper.Map<Customer, CustomerDto>(customer);
        }

        [Obsolete]
        private static void ValueResolverMap()
        {
            Mapper.Initialize(cfg =>
                cfg.CreateMap<Student, StudentDto>().ForMember(d => d.Score, o => o.MapFrom<ScoreResolver>()));

            var student = new Student { Name = "Colin", Score = 95 };
            var studentDto = Mapper.Map<Student, StudentDto>(student);
        }

        [Obsolete]
        private static void DynamicMap()
        {
            Mapper.Initialize(cfg => { });

            dynamic user = new ExpandoObject();
            user.Id = 1;
            user.Name = "Colin";
            user.Age = 18;

            var u = Mapper.Map<User>(user);
        }
    }
}
