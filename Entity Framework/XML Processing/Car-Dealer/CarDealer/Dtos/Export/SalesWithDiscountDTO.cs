﻿using System.Xml.Serialization;

namespace CarDealer.Dtos.Export
{
    [XmlType("sale")]
    public class SalesWithDiscountDTO
    {
        [XmlElement("car")]
        public CarSaleDTO Car { get; set; }

        [XmlElement("discount")]
        public decimal Discount { get; set; }

        [XmlElement("customer-name")]
        public string CustomerName { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }

        [XmlElement("price-with-discount")]
        public decimal PriceWithtDiscount { get; set; }
    }

    [XmlType("car")]
    public class CarSaleDTO
    {
        [XmlAttribute("make")]
        public string Make { get; set; }

        [XmlAttribute("model")]
        public string Model { get; set; }

        [XmlAttribute("travelled-distance")]
        public long TravelledDistance { get; set; }
    }
}
