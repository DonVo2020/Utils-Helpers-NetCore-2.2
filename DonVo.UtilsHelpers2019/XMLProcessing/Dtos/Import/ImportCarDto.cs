using System.Collections.Generic;
using System.Xml.Serialization;

namespace XMLProcessing.Dtos.Import
{
    [XmlType("Car")]
    public class ImportCarDto
    {
        [XmlElement("make")]
        public string Make { get; set; }

        [XmlElement("model")]
        public string Model { get; set; }

        public long TraveledDistance { get; set; }

        [XmlArray("parts")]
        public HashSet<PartIdDto> Parts { get; set; }
    }
}
