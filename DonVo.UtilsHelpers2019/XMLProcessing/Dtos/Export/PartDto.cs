using System.Xml.Serialization;

namespace XMLProcessing.Dtos.Export
{
    [XmlType("part")]
    public class PartDto
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("price")]
        public decimal Price { get; set; }
    }
}
