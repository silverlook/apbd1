using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apbd1.Configuration
{
    public static class BusinessRules
    {
        public const int StudentMaxActiveRentals = 2;
        public const int EmployeeMaxActiveRentals = 5;

        public const decimal LateFeePerDay = 10.00m;

        public const int DefaultRentalDurationDays = 7;
    }
}
