using apbd1.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apbd1.Models.Users
{
    public class Employee : User
    {
        public string Department { get; set; }
        public string Position { get; set; }

        public Employee(string firstName, string lastName, string department, string position)
            : base(firstName, lastName)
        {
            Department = department;
            Position = position;
        }

        public override UserType Type => UserType.Employee;
        public override int MaxActiveRentals => BusinessRules.EmployeeMaxActiveRentals;
        public override string GetRoleDescription() =>
            $"Pracownik | Dzial: {Department} | Stanowisko: {Position}";
    }
}
