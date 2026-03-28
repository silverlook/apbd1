using apbd1.Models.Equipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apbd1.Services
{
    public class ReportService
    {
        private readonly EquipmentService _equipmentService;
        private readonly UserService _userService;
        private readonly RentalService _rentalService;

        public ReportService(EquipmentService equipmentService, UserService userService, RentalService rentalService)
        {
            _equipmentService = equipmentService;
            _userService = userService;
            _rentalService = rentalService;
        }

        public RentalReport GenerateReport()
        {
            var allEquipment = _equipmentService.GetAll();
            var allRentals = _rentalService.GetAllRentals();

            return new RentalReport
            {
                TotalEquipment = allEquipment.Count,
                AvailableEquipment = allEquipment.Count(e => e.Status == EquipmentStatus.Available),
                RentedEquipment = allEquipment.Count(e => e.Status == EquipmentStatus.Rented),
                UnavailableEquipment = allEquipment.Count(e => e.Status == EquipmentStatus.Unavailable),
                TotalUsers = _userService.GetAll().Count,
                TotalRentals = allRentals.Count,
                ActiveRentals = allRentals.Count(r => !r.IsReturned),
                CompletedRentals = allRentals.Count(r => r.IsReturned),
                OverdueRentals = _rentalService.GetOverdueRentals().Count,
                TotalLateFees = allRentals.Sum(r => r.LateFee),
                GeneratedAt = DateTime.Now
            };
        }
    }

    public class RentalReport
    {
        public int TotalEquipment { get; set; }
        public int AvailableEquipment { get; set; }
        public int RentedEquipment { get; set; }
        public int UnavailableEquipment { get; set; }
        public int TotalUsers { get; set; }
        public int TotalRentals { get; set; }
        public int ActiveRentals { get; set; }
        public int CompletedRentals { get; set; }
        public int OverdueRentals { get; set; }
        public decimal TotalLateFees { get; set; }
        public DateTime GeneratedAt { get; set; }
    }
}
