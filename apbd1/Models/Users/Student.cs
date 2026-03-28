using apbd1.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apbd1.Models.Users
{
    public class Student : User
    {
        public string StudentId { get; set; }
        public string FacultyName { get; set; }

        public Student(string firstName, string lastName, string studentId, string facultyName)
            : base(firstName, lastName)
        {
            StudentId = studentId;
            FacultyName = facultyName;
        }

        public override UserType Type => UserType.Student;
        public override int MaxActiveRentals => BusinessRules.StudentMaxActiveRentals;
        public override string GetRoleDescription() =>
            $"Student | Nr albumu: {StudentId} | Wydzial: {FacultyName}";
    }
}
