using LearnModelValidation;
using LearnModelValidation.Models;
using System;

namespace LearnModelValidation.Models
{
    public class Employee : User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        [employeeEnsureSalary]
        public double Salary { get; set; }

        public Employee() : base() { }

        public Employee(int id, string name, string position, double salary, string email, string password)
            : base(email, password)
        {
            Id = id;
            Name = name;
            Position = position;
            Salary = salary;
        }

        public static ValueTask<Employee?> BindAsyncQueryString(HttpContext context)
        {
            var idStr = context.Request.Query["id"];
            int id = int.TryParse(idStr, out var parsedId) ? parsedId : 0;
            var nameStr = context.Request.Query["name"];
            var positionStr = context.Request.Query["position"];
            var salaryStr = double.TryParse(context.Request.Query["salary"], out var salary) ? salary : 0.0;
            var emailStr = context.Request.Query["email"];
            var passwordStr = context.Request.Query["password"];
            var confirmedPasswordStr = context.Request.Query["confirmedPassword"];
            if (id > 0)
            {
                return new ValueTask<Employee?>(new Employee { Id = id, Name = nameStr, Position = positionStr, Salary = salaryStr, Email = emailStr, Password = passwordStr, ConfirmedPassword = confirmedPasswordStr });
            }
            return new ValueTask<Employee?>(Task.FromResult<Employee?>(null));
        }
    }
}
