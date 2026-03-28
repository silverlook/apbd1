using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apbd1.Models.Equipment
{
    public enum EquipmentStatus
    {
        Available,
        Rented,
        Unavailable
    }

    public abstract class Equipment
    {
        public string Id { get; }
        public string Name { get; set; }
        public EquipmentStatus Status { get; set; }

        protected Equipment(string name)
        {
            Id = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            Name = name;
            Status = EquipmentStatus.Available;
        }

        public bool IsAvailable => Status == EquipmentStatus.Available;

        public abstract string GetTypeDescription();
        public abstract string GetSpecifications();
    }
}
