using System;
using System.Linq;

namespace AutoMappingObjects
{
    class Engine
    {
        private IServiceProvider serviceProvider;
        bool isRunning;

        public Engine(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.isRunning = true;
        }

        internal void Run()
        {
            while (isRunning)
            {
                Console.WriteLine("Enter command, which 'AddEmployee', and space for each First Last Salary and hit Enter or");
                Console.WriteLine("Enter command, which 'EmployeeInfo' and Id = 1 and hit Enter or");
                Console.WriteLine("Enter command, which 'EmployeePersonalInfo' and Id = 1 and hit Enter or");
                Console.WriteLine("Enter command, which 'SetAddress' and Id = 1 and 'Address' and hit Enter or");
                Console.WriteLine("Enter command, which 'SetBirthday' and Id = 1 and '05-05-1980' and hit Enter or");
                Console.WriteLine("Enter 'Exit' command to exit.");
                string input = Console.ReadLine();

                string[] commandTokens = input.Split().ToArray();

                string commandName = commandTokens[0];

                string[] commandArgs = commandTokens.Skip(1).ToArray();

                var command = CommandParser.ParseCommand(this.serviceProvider, commandName);

                var result = command.Execute(commandArgs);

                Console.WriteLine(result);
            }
        }
    }
}
