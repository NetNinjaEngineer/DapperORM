using Dapper;
using Dapper.Shared;
using Dapper.Shared.Models;
using Microsoft.Extensions.Configuration;

namespace QueryingDataWithDapper
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var _context = new DapperContext(configuration);

            //DapperJoin(_context);

            //GetSingleItemWithDapper(_context);

            //QueryScalerValues(_context);

            //QueryingSingleRowWithDapper(_context);

            //QueryMultipleRows(_context);

            //QueryMultipleResults(_context);

            //QuerySpecificColumns(_context);

            ExecutingNonQueryCommands(_context);


            Console.ReadKey();
        }

        private static void ExecutingNonQueryCommands(DapperContext context)
        {
            using (var connection = context.CreateConnection())
            {
                var sql = @"INSERT INTO Departments(Code, DepartmentName, DateOfCreation) 
                    VALUES (@code, @departmentName, @dateOfCreate)";

                var rowsAffected = connection.Execute(sql, new
                { code = "FIN000", departmentName = "Financce", dateOfCreate = "2024-04-04" });

                if (rowsAffected > 0)
                    Console.WriteLine("Added Successfully");

            }
        }

        private static void QuerySpecificColumns(DapperContext context)
        {
            using (var connection = context.CreateConnection())
            {
                var sql = "Select DepartmentName, Code, DateOfCreation From Departments";

                var departments = connection.Query<Department>(sql);

                foreach (var department in departments)
                    Console.WriteLine($"\nDept: {department.DepartmentName}" +
                        $"\nDateOfCreation: {department.DateOfCreation}" +
                        $"\nCode: {department.Code}");

            }
        }

        private static void QueryMultipleResults(DapperContext context)
        {
            using (var connection = context.CreateConnection())
            {
                #region QueryMultiple method allows you to select multiple results from a database query

                /*
                    The QueryMultiple method provides a simple way of fetching data stored in different tables or views with a single database query.
                    By using dapper's QueryMultiple method, you can select multiple rows from different tables in one go, saving time and resources.
                    Dapper's QueryMultiple method allows you to map results from multiple queries into strongly typed collections that can be easily consumed within your application code.
                    Using Dapper QueryMultiple is an efficient way to select multiple results from a single database query.
                    It saves time and resources by avoiding unnecessary round trips to the database server and eliminates the need for complex SQL queries.

                 */

                var sql = @"
                    Select * From Employees;
                    Select * From Departments;
                    Select * From Departments Where Id = @departmentId
                ";

                using (var multiResult = connection.QueryMultiple(sql, new { departmentId = 7 }))
                {
                    var employees = multiResult.Read<Employee>().ToList();

                    var departments = multiResult.Read<Department>().ToList();

                    var departmentWithId7 = multiResult.ReadFirstOrDefault<Department>();

                    if (departmentWithId7 is not null)
                        Console.WriteLine(departmentWithId7.Id + $"\t Code: {departmentWithId7.Code}");


                    foreach (var employee in employees)
                        Console.WriteLine($"\nEmployeeName: {employee.Name}" +
                            $"\nAge: {employee.Age}" +
                            $"\nSalary: {employee.Salary:C}" +
                            $"\nEmail: {employee.Email}" +
                            $"\nPhoneNumber: {employee.PhoneNumber}" +
                            $"\nHireDate: {employee.HireDate}");

                    foreach (var department in departments)
                        Console.WriteLine($"\nName: {department.DepartmentName}" +
                            $"\nCode: {department.Code}" +
                            $"\nDateOfCreation: {department.DateOfCreation}");

                }


                #endregion

            }
        }

        private static void QueryMultipleRows(DapperContext context)
        {
            using (var connection = context.CreateConnection())
            {
                var sql = "Select * From Employees";

                var employeesAsGeneric = connection.Query<Employee>(sql);

                if (employeesAsGeneric.Any())
                    foreach (var employee in employeesAsGeneric)
                    {
                        Console.WriteLine($"\nEmployeeName: {employee.Name}" +
                          $"\nAge: {employee.Age}" +
                          $"\nSalary: {employee.Salary:C}" +
                          $"\nEmail: {employee.Email}" +
                          $"\nPhoneNumber: {employee.PhoneNumber}" +
                          $"\nHireDate: {employee.HireDate}");
                    }


                var employeesAsDynamic = connection.Query(sql);

                if (employeesAsDynamic.Any())
                    foreach (var employee in employeesAsDynamic)
                    {
                        Console.WriteLine($"\nEmployeeName: {employee.Name}" +
                          $"\nAge: {employee.Age}" +
                          $"\nSalary: {employee.Salary:C}" +
                          $"\nEmail: {employee.Email}" +
                          $"\nPhoneNumber: {employee.PhoneNumber}" +
                          $"\nHireDate: {employee.HireDate}");
                    }

            }
        }

        private static void QueryingSingleRowWithDapper(DapperContext context)
        {
            #region Query Single Row
            // QuerySingle - QueryFirst - QuerySingleOrDefault - QueryFirstOrDefault
            #endregion

            using (var connection = context.CreateConnection())
            {
                var sql = "Select * From Employees Where Id = @employeeId";

                var employeeAsGenericType = connection.QuerySingleOrDefault<Employee>(sql, new { employeeId = 17 });

                var employeeAsDynamic = connection.QuerySingleOrDefault(sql, new { employeeId = 16 });

                if (employeeAsDynamic is not null)
                    Console.WriteLine($"\nEmployeeName: {employeeAsDynamic.Name}" +
                           $"\nAge: {employeeAsDynamic.Age}" +
                           $"\nSalary: {employeeAsDynamic.Salary:C}" +
                           $"\nEmail: {employeeAsDynamic.Email}" +
                           $"\nPhoneNumber: {employeeAsDynamic.PhoneNumber}" +
                           $"\nHireDate: {employeeAsDynamic.HireDate}");


                if (employeeAsGenericType is not null)
                    Console.WriteLine($"\nEmployeeName: {employeeAsGenericType.Name}" +
                        $"\nAge: {employeeAsGenericType.Age}" +
                        $"\nSalary: {employeeAsGenericType.Salary:C}" +
                        $"\nEmail: {employeeAsGenericType.Email}" +
                        $"\nPhoneNumber: {employeeAsGenericType.PhoneNumber}" +
                        $"\nHireDate: {employeeAsGenericType.HireDate}");


            }
        }

        private static void QueryScalerValues(DapperContext context)
        {
            using (var connection = context.CreateConnection())
            {
                var sql = "Select Count(*) From Employees";

                #region Returns The first column of the first row as a dynamic type
                var employeesCountDynamic = connection.ExecuteScalar(sql);
                var employeesCountAsInt = connection.ExecuteScalar<int>(sql);
                #endregion

                Console.WriteLine(employeesCountDynamic);
                Console.WriteLine(employeesCountAsInt);

            }
        }

        private static void GetSingleItemWithDapper(DapperContext _context)
        {
            using (var connection = _context.CreateConnection())
            {
                var sql = "Select * From Employees Where Id = @employeeId";

                var employee = connection.Query<Employee>(sql, new { employeeId = 16 })
                    .FirstOrDefault();

                if (employee is not null)
                    Console.WriteLine($"\nEmployeeName: {employee.Name}" +
                        $"\nAge: {employee.Age}" +
                        $"\nSalary: {employee.Salary.ToString("C")}" +
                        $"\nEmail: {employee.Email}" +
                        $"\nPhoneNumber: {employee.PhoneNumber}" +
                        $"\nHireDate: {employee.HireDate}");

            }
        }

        private static void DapperJoin(DapperContext _context)
        {
            using (var connection = _context.CreateConnection())
            {
                var sql = "SELECT * FROM Employees INNER JOIN Departments On Employees.DepartmentId = Departments.Id";

                var employees = connection.Query<Employee, Department, Employee>(sql, (employee, department) =>
                {
                    employee.Department = department;
                    return employee;
                }).ToList();

                foreach (var employee in employees)
                {
                    Console.WriteLine($"\nName: {employee.Name}" +
                        $"\nDepartment: {employee.Department.DepartmentName}");
                }
            }
        }
    }
}
