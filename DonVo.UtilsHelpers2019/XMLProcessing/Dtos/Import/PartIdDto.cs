using System.Xml.Serialization;

namespace XMLProcessing.Dtos.Import
{
    [XmlType("partId")]
    public class PartIdDto
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        public override bool Equals(object obj)
        {
            return this.Id == ((PartIdDto)obj).Id;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}