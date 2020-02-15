using System.Xml.Serialization;

namespace XMLProcessing.Dtos.Export
{
    [XmlType("sale")]
    public class SaleDto
    {
        [XmlElement("car")]
        public CarSaleDto Car { get; set; }

        [XmlElement("discount")]
        public int Discount { get; set; }

        [XmlElement("customer-name")]
        public string CustomerName { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }

        [XmlElement("price-with-discount")]
        public string PriceWithDiscount { get; set; }
    }
}
