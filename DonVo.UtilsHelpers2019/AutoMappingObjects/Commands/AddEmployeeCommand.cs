using AutoMappingObjects.DTOs;
using AutoMappingObjects.Services;

namespace AutoMappingObjects.Commands
{
    class AddEmployeeCommand : ICommand
    {
        private EmployeeService employeeService;

        public AddEmployeeCommand(EmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        // <firstName>, <lastName>, <salary>
        public string Execute(params string[] args)
        {
            string firstName = args[0];
            string lastName = args[1];
            decimal salary = decimal.Parse(args[2]);

            EmployeeDto employeeDto = new EmployeeDto(firstName, lastName, salary);

            this.employeeService.AddEmployee(employeeDto);

            return $"Sucessfully added {firstName}";
        }
    }
}
