namespace AutoMappingObjects.Commands
{
    internal interface ICommand
    {
        string Execute(params string[] args);
    }
}
