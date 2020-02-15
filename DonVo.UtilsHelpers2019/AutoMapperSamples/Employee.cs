namespace AutoMapperSamples
{
    public class Employee
    {
        public string Name { get; set; }
        public Company Company { get; set; }
    }

    public class Company
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public class EmployeeDto
    {
        public string Name { get; set; }

        //The target class attribute must be the internal attribute name of the complex attribute name + complex attribute type in the source type
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
    }
}