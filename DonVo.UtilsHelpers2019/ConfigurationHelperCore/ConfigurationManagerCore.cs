using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace ConfigurationHelperCore
{
    public class ConfigurationManagerCore
    {
        /// <summary>
        /// Indexer of Configurations in appsettings.json
        /// </summary>
        public static IConfiguration Configuration { get; }

        /// <summary>
        /// Indexer of CommandLine and InMemoryCollection configuration.
        /// </summary>
        /// <param name="cmdParameter">CommandLine</param>
        public string this[string cmdParameter] => _cmdConfiguration[cmdParameter];

        private static IServiceCollection _services;
        private IConfiguration _cmdConfiguration;

        static ConfigurationManagerCore()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            _services = new ServiceCollection();
        }

        /// <summary>
        /// Create an instance can read CommandLine and InMemoryCollection configuration.
        /// </summary>
        /// <param name="args">CommandLine args</param>
        /// <param name="memoryCollection">InMemoryCollection Configuration</param>
        public ConfigurationManagerCore(string[] args = null, Dictionary<string, string> memoryCollection = null)
        {
            var config = new ConfigurationBuilder();
            if (args != null)
                config.AddCommandLine(args);
            if (memoryCollection != null)
                config.AddInMemoryCollection(memoryCollection);

            _cmdConfiguration = config.Build();
        }

        /// <summary>
        /// Get layered configuration in appsettings.json
        /// </summary>
        /// <param name="keys">layered keys of configurations</param>
        /// <returns>configuration value</returns>
        public static string GetAppSettings(params string[] keys)
        {
            if (keys == null || keys.Length <= 0)
                return null;

            return Configuration[string.Join(":", keys)];
        }

        /// <summary>
        /// Get configurations in appsettings.json and map to a specify object.
        /// </summary>
        /// <param name="key">key of the specify configurations</param>
        /// <typeparam name="T">Type of the object to map to.</typeparam>
        /// <returns>configuration object</returns>
        public static T GetAppSettings<T>(string key) where T : class, new()
        {
            return _services.Configure<T>(Configuration.GetSection(key)).BuildServiceProvider()
                .GetService<IOptionsSnapshot<T>>().Value;
        }
    }
}
