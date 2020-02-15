using AutoMapper;
using AutoMappingObjects.Data;
using AutoMappingObjects.DTOs;
using AutoMappingObjects.Models;
using System;

namespace AutoMappingObjects.Services
{
    public class EmployeeService
    {
        private readonly EmployeesContext context;
        private readonly IMapper mapper;

        public EmployeeService(EmployeesContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public EmployeeDto ById(int id)
        {
            var employee = context.Employees.Find(id);

            var employeeDto = mapper.Map<EmployeeDto>(employee);

            return employeeDto;
        }

        public EmployeePersonalInfoDto PersonalById(int employeeId)
        {
            var employee = context.Employees.Find(employeeId);

            var employeeDto = mapper.Map<EmployeePersonalInfoDto>(employee);

            return employeeDto;
        }


        public void AddEmployee(EmployeeDto employeeDto)
        {
            var employee = mapper.Map<Employee>(employeeDto);

            this.context.Employees.Add(employee);

            this.context.SaveChanges();
        }


        public string SetBirthday(int employeeId, DateTime birthdayDate)
        {
            var employee = this.context.Employees.Find(employeeId);

            employee.Birthday = birthdayDate;

            this.context.SaveChanges();

            return $"{employee.FirstName}  {employee.LastName}";

        }

        public object SetAddress(int employeeId, string address)
        {
            var employee = this.context.Employees.Find(employeeId);

            employee.Address = address;

            this.context.SaveChanges();

            return $"{employee.FirstName}  {employee.LastName}";
        }
    }
}
