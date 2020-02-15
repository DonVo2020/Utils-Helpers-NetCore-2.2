using System.Xml.Serialization;

namespace XMLProcessing.Dtos.Export
{
    [XmlType("customer")]
    public class CustomerDto
    {
        [XmlAttribute("full-name")]
        public string FullName { get; set; }

        [XmlAttribute("bought-cars")]
        public int BoughtCars { get; set; }

        [XmlAttribute("spent-money")]
        public decimal SpentMoney { get; set; }
    }
}
