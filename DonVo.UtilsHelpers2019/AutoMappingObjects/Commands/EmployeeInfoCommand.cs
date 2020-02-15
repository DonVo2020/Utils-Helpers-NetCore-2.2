using AutoMappingObjects.DTOs;
using AutoMappingObjects.Services;

namespace AutoMappingObjects.Commands
{
    class EmployeeInfoCommand : ICommand
    {
        private EmployeeService employeeService;

        public EmployeeInfoCommand(EmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        // <employeeId>
        public string Execute(params string[] args)
        {
            int employeeId = int.Parse(args[0]);

            EmployeeDto employee = this.employeeService.ById(employeeId);

            return $"{employee.Id} - {employee.FirstName} {employee.LastName} - ${employee.Salary:f2}";
        }
    }
}
