using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apbd1.Models.Equipment
{
    public class Projector : Equipment
    {
        public int LumensOutput { get; set; }
        public bool HasRemoteControl { get; set; }
        public string Resolution { get; set; }

        public Projector(string name, int lumensOutput, bool hasRemoteControl, string resolution) : base(name)
        {
            LumensOutput = lumensOutput;
            HasRemoteControl = hasRemoteControl;
            Resolution = resolution;
        }

        public override string GetTypeDescription() => "Projektor";

        public override string GetSpecifications() =>
            $"Jasnosc: {LumensOutput} lm, Rozdzielczosc: {Resolution}, Pilot: {(HasRemoteControl ? "Tak" : "Nie")}";
    }
}
