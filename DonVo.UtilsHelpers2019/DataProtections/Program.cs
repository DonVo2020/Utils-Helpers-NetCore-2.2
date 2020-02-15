using Microsoft.Extensions.DependencyInjection;
using System;

namespace DataProtections
{
    class Program
    {
        public static void Main(string[] args)
        {
            //DataProtectionSample();
            //Console.WriteLine("----------------------------------------------------");
            //NonDependencyInjectionExample01.Run();
            //Console.WriteLine("----------------------------------------------------");
            //NonDependencyInjectionExample02.Run();
            //Console.WriteLine("----------------------------------------------------");
            //Console.WriteLine("----------------------------------------------------");
            //DangerousUnprotect.Run();
            //Console.WriteLine("----------------------------------------------------");
            //LimitedLifetimePayloads.Run();
            //Console.WriteLine("----------------------------------------------------");
            //PasswordHasher.Run();
            //Console.WriteLine("----------------------------------------------------");
            //KeyManagementExtensibility.Run(); // DOES NOT WORK.
            Console.WriteLine("----------------------------------------------------");
            KeyManagement.Run();
        }

        private static void DataProtectionSample()
        {
            // add data protection services
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDataProtection();
            var services = serviceCollection.BuildServiceProvider();

            // create an instance of MyClass using the service provider
            var instance = ActivatorUtilities.CreateInstance<ProtectUnprotect>(services);
            instance.RunSample();
        }        
    }
}
