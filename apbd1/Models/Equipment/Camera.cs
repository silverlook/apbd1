using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apbd1.Models.Equipment
{
    class Camera : Equipment
    {
        public double MegaPixels { get; set; }
        public bool HasVideoSupport { get; set; }
        public string SensorType { get; set; }

        public Camera(string name, double megaPixels, bool hasVideoSupport, string sensorType) : base(name)
        {
            MegaPixels = megaPixels;
            HasVideoSupport = hasVideoSupport;
            SensorType = sensorType;
        }

        public override string GetTypeDescription() => "Aparat/Kamera";

        public override string GetSpecifications() =>
            $"Rozdzielczosc: {MegaPixels} MP, Wideo: {(HasVideoSupport ? "Tak" : "Nie")}, Sensor: {SensorType}";

    }
}
