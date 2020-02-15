﻿namespace XMLProcessing.Dtos.Export
{
    using System.Xml.Serialization;

    [XmlType("car")]
    public class ExportCarWithDistanceDto
    {
        [XmlElement("make")]
        public string Make { get; set; }

        [XmlElement("model")]
        public string Model { get; set; }

        [XmlElement("travelled-distance")]
        public long TravelledDistance { get; set; }
    }
}
