using System.Xml.Serialization;

namespace XMLProcessing.Dtos.Export
{
    [XmlType("car")]
    public class CarInfoDto
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlAttribute("model")]
        public string Model { get; set; }

        [XmlAttribute("travelled-distance")]
        public long TravelledDistance { get; set; }
    }
}
