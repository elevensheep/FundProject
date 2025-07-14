using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace FundProject.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }  // ← _id 필드 매핑
        public string Nickname { get; set; }
        public decimal Balance { get; set; }
        public List<OwnedStock> Portfolio { get; set; } = new List<OwnedStock>();
        public List<TradeHistory> Trades { get; set; } = new List<TradeHistory>();

    }

    public class OwnedStock
    {
        public string StockCode { get; set; }
        public string StockName { get; set; }
        public int Quantity { get; set; }
        public decimal AvgPrice { get; set; }
        public decimal LatestPrice { get; set; }
    }

    public class TradeHistory
    {
        public string StockCode { get; set; }
        public string StockName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime TradeDate { get; set; }
        public string Type { get; set; } // "Buy" or "Sell"
    }

}
