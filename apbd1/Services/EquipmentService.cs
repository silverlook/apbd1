using apbd1.Models.Equipment;
using apbd1.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apbd1.Services
{
    public class EquipmentService
    {
        private readonly List<Equipment> _equipment = new List<Equipment>();

        public Result<Equipment> AddEquipment(Equipment equipment)
        {
            _equipment.Add(equipment);
            return Result<Equipment>.Success(equipment);
        }

        public Equipment? GetById(string id) =>
            _equipment.FirstOrDefault(e => e.Id == id);

        public IReadOnlyList<Equipment> GetAll() => _equipment.AsReadOnly();

        public IReadOnlyList<Equipment> GetAvailable() =>
            _equipment.Where(e => e.Status == EquipmentStatus.Available).ToList().AsReadOnly();

        public Result MarkAsUnavailable(string equipmentId)
        {
            var equipment = GetById(equipmentId);
            if (equipment == null)
                return Result.Failure($"Sprzet o ID {equipmentId} nie istnieje.");

            if (equipment.Status == EquipmentStatus.Rented)
                return Result.Failure(
                    $"Sprzet '{equipment.Name}' jest aktualnie wypozyczony i nie moze zostac oznaczony jako niedostepny.");

            equipment.Status = EquipmentStatus.Unavailable;
            return Result.Success();
        }

        internal void SetStatus(string equipmentId, EquipmentStatus status)
        {
            var equipment = GetById(equipmentId);
            if (equipment != null)
                equipment.Status = status;
        }
    }
}
