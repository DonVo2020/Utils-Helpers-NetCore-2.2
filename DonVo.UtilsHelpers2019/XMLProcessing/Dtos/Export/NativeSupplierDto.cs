using System.Xml.Serialization;

namespace XMLProcessing.Dtos.Export
{
    [XmlType("suplier")]
    public class NativeSupplierDto
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("parts-count")]
        public int PartsCount { get; set; }
    }
}
