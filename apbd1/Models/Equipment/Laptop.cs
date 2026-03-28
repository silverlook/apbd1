using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apbd1.Models.Equipment
{
    public class Laptop : Equipment
    {
        public int RamGb { get; set; }
        public int StorageGb { get; set; }
        public string Processor { get; set; }

        public Laptop(string name, int ramGb, int storageGb, string processor) : base(name)
        {
            RamGb = ramGb;
            StorageGb = storageGb;
            Processor = processor;
        }

        public override string GetTypeDescription() => "Laptop";

        public override string GetSpecifications() =>
            $"RAM: {RamGb} GB, Dysk: {StorageGb} GB, Procesor: {Processor}";
    }
}
