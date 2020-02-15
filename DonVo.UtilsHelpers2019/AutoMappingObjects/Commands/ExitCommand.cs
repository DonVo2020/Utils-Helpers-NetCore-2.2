using System;

namespace AutoMappingObjects.Commands
{
    class ExitCommand : ICommand
    {
        public string Execute(params string[] args)
        {
            Environment.Exit(0);
            return String.Empty;
        }
    }
}
