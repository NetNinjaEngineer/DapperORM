namespace Dapper.Shared.Models
{
    public class Department
    {
        public int Id { get; set; }

        public string? DepartmentName { get; set; }

        public string? Code { get; set; }

        public DateTime DateOfCreation { get; set; }

        public ICollection<Employee> Employees { get; set; } = [];
    }
}
