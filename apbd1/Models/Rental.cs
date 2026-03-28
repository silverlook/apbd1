using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apbd1.Models
{
    public class Rental
    {
        public string Id { get; }
        public string UserId { get; }
        public string EquipmentId { get; }
        public DateTime RentalDate { get; }
        public DateTime DueDate { get; }
        public DateTime? ReturnDate { get; private set; }
        public decimal LateFee { get; private set; }

        public bool IsReturned => ReturnDate.HasValue;
        public bool IsOverdue => !IsReturned && DateTime.Now > DueDate;

        public Rental(string userId, string equipmentId, DateTime rentalDate, DateTime dueDate)
        {
            Id = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            UserId = userId;
            EquipmentId = equipmentId;
            RentalDate = rentalDate;
            DueDate = dueDate;
        }

        public void CompleteReturn(DateTime returnDate, decimal lateFee)
        {
            ReturnDate = returnDate;
            LateFee = lateFee;
        }
    }
}
