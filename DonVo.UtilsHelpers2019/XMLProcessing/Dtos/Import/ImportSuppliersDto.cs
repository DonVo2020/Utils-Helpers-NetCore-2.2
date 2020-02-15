using System.Xml.Serialization;

namespace XMLProcessing.Dtos.Import
{
    [XmlType("Supplier")]
    public class ImportSuppliersDto
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("isImporter")]
        public bool  IsImporter { get; set; }
    }
}
