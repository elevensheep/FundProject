using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace FundProject.Models
{
    public class StockPrice
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string StockCode { get; set; }
        public string StockName { get; set; }

        public List<PriceEntry> Prices { get; set; } = new List<PriceEntry>();
    }

    public class PriceEntry
    {
        public DateTime TradeDate { get; set; }
        public decimal Open { get; set; }
        public decimal Close { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public long Volume { get; set; }

        public override bool Equals(object obj)
        {
            return obj is PriceEntry other && TradeDate == other.TradeDate;
        }

        public override int GetHashCode()
        {
            return TradeDate.GetHashCode();
        }
    }
}
