using System;
using System.Collections.Generic;

namespace ConfigurationHelperCore
{
    class Program
    {
        static void Main(string[] args)
        {
            //try to run "dotnet run --name Colin --age 18"
            CommandLineConfiguration(args);

            InMemoryCollectionConfiguration();

            JsonFileConfiguration();

            JsonFileObjectConfiguration();

            Console.ReadKey();
        }

        private static void CommandLineConfiguration(string[] args)
        {
            var config = new ConfigurationManagerCore(args);

            Console.WriteLine($"name:{config["name"]} \t age:{config["age"]}");
        }

        private static void InMemoryCollectionConfiguration()
        {
            var settings = new Dictionary<string, string>
            {
                {"gender", "male"},
                {"nationality", "China"}
            };
            var config = new ConfigurationManagerCore(memoryCollection: settings);
            Console.WriteLine($"gender:{config["gender"]} \t nationality:{config["nationality"]}");
        }

        private static void JsonFileConfiguration()
        {
            var appName = ConfigurationManagerCore.Configuration["AppName"];
            //var className = ConfigurationManagerCore.Configuration["Class:ClassName"];
            var className = ConfigurationManagerCore.GetAppSettings("Class", "ClassName");
            var firstStudentName = ConfigurationManagerCore.Configuration["Class:Students:0:Name"];

            Console.WriteLine($"AppName:{appName}\tClassName:{className}\tFirstStudentName:{firstStudentName}");
        }

        private static void JsonFileObjectConfiguration()
        {
            var cls = ConfigurationManagerCore.GetAppSettings<Class>("Class");
            Console.WriteLine(cls.ClassName);
        }
    }
}
