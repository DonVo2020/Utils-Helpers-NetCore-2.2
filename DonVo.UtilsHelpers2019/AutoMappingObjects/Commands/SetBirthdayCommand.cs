using AutoMappingObjects.Services;
using System;
using System.Globalization;

namespace AutoMappingObjects.Commands
{
    class SetBirthdayCommand : ICommand
    {
        private EmployeeService employeeService;

        public SetBirthdayCommand(EmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        // <employeeId>, <date: "dd-MM-yyyy">
        public string Execute(params string[] args)
        {
            int employeeId = int.Parse(args[0]);
            DateTime birthdayDate = DateTime.ParseExact(args[1], "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var employeeName = this.employeeService.SetBirthday(employeeId, birthdayDate);

            return $"Successfully set {employeeName} birthday to {birthdayDate}"; ;
        }
    }
}
