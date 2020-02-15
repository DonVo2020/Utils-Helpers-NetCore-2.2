using AutoMappingObjects.Commands;
using System;

using System.Linq;
using System.Reflection;

namespace AutoMappingObjects
{
    class CommandParser
    {
        public static ICommand ParseCommand(IServiceProvider serviceProvider, string commandName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var commandTypes = assembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(ICommand)));

            var commandType = commandTypes.SingleOrDefault(c => c.Name == commandName + "Command");

            if (commandType == null)
            {
                throw new InvalidOperationException("Invalid command name");
            }

            var commandConstructor = commandType.GetConstructors().FirstOrDefault();

            var ctorParamTypes = commandConstructor
                .GetParameters()
                .Select(p => p.ParameterType)
                .ToArray();

            var ctorArgs = ctorParamTypes
                .Select(p => serviceProvider.GetService(p))
                .ToArray();

            var command = (ICommand)commandConstructor.Invoke(ctorArgs);

            return command;
        }
    }
}
