using apbd1.Common;
using apbd1.Configuration;
using apbd1.Models;
using apbd1.Models.Equipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apbd1.Services
{
    public class RentalService
    {
        private readonly List<Rental> _rentals = new List<Rental>();
        private readonly EquipmentService _equipmentService;
        private readonly UserService _userService;

        public RentalService(EquipmentService equipmentService, UserService userService)
        {
            _equipmentService = equipmentService;
            _userService = userService;
        }

        public Result<Rental> RentEquipment(string userId, string equipmentId, DateTime? dueDate = null)
        {
            var user = _userService.GetById(userId);
            if (user == null)
                return Result<Rental>.Failure($"Uzytkownik o ID {userId} nie istnieje.");

            var equipment = _equipmentService.GetById(equipmentId);
            if (equipment == null)
                return Result<Rental>.Failure($"Sprzet o ID {equipmentId} nie istnieje.");

            if (!equipment.IsAvailable)
                return Result<Rental>.Failure(
                    $"Sprzet '{equipment.Name}' jest niedostepny (status: {equipment.Status}).");

            var activeCount = GetActiveRentalsForUser(userId).Count;
            if (activeCount >= user.MaxActiveRentals)
                return Result<Rental>.Failure(
                    $"Uzytkownik {user.FullName} osiagnal limit aktywnych wypozyczen ({user.MaxActiveRentals}).");

            var rentalDate = DateTime.Now;
            var returnDeadline = dueDate ?? rentalDate.AddDays(BusinessRules.DefaultRentalDurationDays);

            var rental = new Rental(userId, equipmentId, rentalDate, returnDeadline);
            _rentals.Add(rental);
            _equipmentService.SetStatus(equipmentId, EquipmentStatus.Rented);

            return Result<Rental>.Success(rental);
        }

        public Result<Rental> ReturnEquipment(string rentalId, DateTime? returnDate = null)
        {
            var rental = _rentals.FirstOrDefault(r => r.Id == rentalId);
            if (rental == null)
                return Result<Rental>.Failure($"Wypozyczenie o ID {rentalId} nie istnieje.");

            if (rental.IsReturned)
                return Result<Rental>.Failure($"Wypozyczenie {rentalId} zostalo juz zamkniete.");

            var actualReturnDate = returnDate ?? DateTime.Now;
            decimal lateFee = 0;

            if (actualReturnDate > rental.DueDate)
            {
                var daysLate = (int)Math.Ceiling((actualReturnDate - rental.DueDate).TotalDays);
                lateFee = daysLate * BusinessRules.LateFeePerDay;
            }

            rental.CompleteReturn(actualReturnDate, lateFee);
            _equipmentService.SetStatus(rental.EquipmentId, EquipmentStatus.Available);

            return Result<Rental>.Success(rental);
        }

        public IReadOnlyList<Rental> GetActiveRentalsForUser(string userId) =>
            _rentals.Where(r => r.UserId == userId && !r.IsReturned).ToList().AsReadOnly();

        public IReadOnlyList<Rental> GetActiveRentals() =>
            _rentals.Where(r => !r.IsReturned).ToList().AsReadOnly();

        public IReadOnlyList<Rental> GetOverdueRentals() =>
            _rentals.Where(r => r.IsOverdue).ToList().AsReadOnly();

        public IReadOnlyList<Rental> GetAllRentals() => _rentals.AsReadOnly();
    }
}
