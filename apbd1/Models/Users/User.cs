using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apbd1.Models.Users
{
    public enum UserType
    {
        Student,
        Employee
    }

    public abstract class User
    {
        public string Id { get; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public abstract UserType Type { get; }
        public string FullName => $"{FirstName} {LastName}";

        protected User(string firstName, string lastName)
        {
            Id = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            FirstName = firstName;
            LastName = lastName;
        }

        public abstract int MaxActiveRentals { get; }
        public abstract string GetRoleDescription();
    }
}
