using System;
using System.IO;
using Microsoft.AspNetCore.DataProtection;

namespace DataProtections
{
    public class NonDependencyInjectionExample01
    {
        public static void Run()
        {
            // Get the path to %LOCALAPPDATA%\myapp-keys
            var destFolder = Path.Combine(
                System.Environment.GetEnvironmentVariable("LOCALAPPDATA"),
                "temp\\myapp-keys");

            // Instantiate the data protection system at this folder
            var dataProtectionProvider = DataProtectionProvider.Create(
                new DirectoryInfo(destFolder));

            var protector = dataProtectionProvider.CreateProtector("Program.No-DI");
            Console.Write("Enter input: ");
            var input = Console.ReadLine();

            // Protect the payload
            var protectedPayload = protector.Protect(input);
            Console.WriteLine($"Protect returned: {protectedPayload}");

            // Unprotect the payload
            var unprotectedPayload = protector.Unprotect(protectedPayload);
            Console.WriteLine($"Unprotect returned: {unprotectedPayload}");

            Console.WriteLine();
            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}
