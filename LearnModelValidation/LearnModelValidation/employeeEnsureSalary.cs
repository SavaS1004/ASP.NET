using System.ComponentModel.DataAnnotations;
using LearnModelValidation.Models;  
namespace LearnModelValidation
{
    public class employeeEnsureSalary:ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var employee=validationContext.ObjectInstance as Employee;
            if(employee is not null && 
                !string.IsNullOrWhiteSpace(employee.Position)
                && employee.Position.Equals("Manager", StringComparison.OrdinalIgnoreCase))
            {
                if(employee.Salary<100000)
                {
                    return new ValidationResult("Manager salary must be at least 100000.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
