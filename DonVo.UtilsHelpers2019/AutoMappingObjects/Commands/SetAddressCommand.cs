using AutoMappingObjects.Services;
using System.Linq;

namespace AutoMappingObjects.Commands
{
    class SetAddressCommand : ICommand
    {
        private EmployeeService employeeService;

        public SetAddressCommand(EmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        // <employeeId>, <address>
        public string Execute(params string[] args)
        {
            int employeeId = int.Parse(args[0]);
            string address = string.Join(" ", args.Skip(1));

            var employeeName = this.employeeService.SetAddress(employeeId, address);

            return $"Successfully set {employeeName} address to {address}";
        }
    }
}
