using System;

namespace AutoMappingObjects.DTOs
{
    public class EmployeePersonalInfoDto
    {
        public EmployeePersonalInfoDto()
        {

        }

        public EmployeePersonalInfoDto(int id, string firstName, string lastName, decimal salary, DateTime? birthday, string address)
        {
            this.Id = id;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Salary = salary;
            this.Birthday = birthday;
            this.Address = address;
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal Salary { get; set; }
        public DateTime? Birthday { get; set; }
        public string Address { get; set; }
    }
}
