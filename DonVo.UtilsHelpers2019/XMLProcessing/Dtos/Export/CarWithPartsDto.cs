using System.Collections.Generic;
using System.Xml.Serialization;

namespace XMLProcessing.Dtos.Export
{
    [XmlType("car")]
    public class CarWithPartsDto
    {
        [XmlAttribute("make")]
        public string Make { get; set; }

        [XmlAttribute("model")]
        public string Model { get; set; }

        [XmlAttribute("travelled-distance")]
        public long TravelledDistance { get; set; }

        [XmlArray("parts")]
        public List<PartDto> Parts{ get; set; }
    }
}
