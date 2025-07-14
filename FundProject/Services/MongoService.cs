using FundProject.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FundProject.Services
{
    public class MongoService
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<StockPrice> _stockCollection;

        public MongoService()
        {
            var client = new MongoClient("mongodb+srv://leehj:qwertyasdfzxcv@cluster.d2zx1ey.mongodb.net/chaincar?retryWrites=true&w=majority");
            _database = client.GetDatabase("StockDB");
            _stockCollection = _database.GetCollection<StockPrice>("StocksWithPrices");
        }

        public async Task InsertOrUpdatePricesAsync(List<PriceEntry> prices, string stockCode, string stockName)
        {
            var filter = Builders<StockPrice>.Filter.Eq(x => x.StockCode, stockCode);

            var update = Builders<StockPrice>.Update
                .SetOnInsert(x => x.StockName, stockName)
                .AddToSetEach(x => x.Prices, prices); // 여러 개 추가

            var options = new UpdateOptions { IsUpsert = true };

            await _stockCollection.UpdateOneAsync(filter, update, options);
        }


        public async Task<List<string>> GetAllCompanyNamesAsync()
        {
            return await _stockCollection.Distinct<string>("StockName", FilterDefinition<StockPrice>.Empty).ToListAsync();
        }

        public async Task<StockPrice> GetStockByNameAsync(string name)
        {
            var filter = Builders<StockPrice>.Filter.Eq(x => x.StockName, name);
            return await _stockCollection.Find(filter).FirstOrDefaultAsync();
        }

        // ✅ 유저 닉네임으로 조회
        public async Task<User> GetUserByNicknameAsync(string nickname)
        {
            var userCollection = _database.GetCollection<User>("Users");
            var filter = Builders<User>.Filter.Eq(u => u.Nickname, nickname);
            return await userCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateUserAsync(User user)
        {
            var collection = _database.GetCollection<User>("Users");
            await collection.InsertOneAsync(user);
        }


        public async Task UpdateUserAsync(User user)
        {
            var collection = _database.GetCollection<User>("Users"); // ✅ 작은따옴표 제거
            var filter = Builders<User>.Filter.Eq(u => u.Nickname, user.Nickname);
            await collection.ReplaceOneAsync(filter, user, new ReplaceOptions { IsUpsert = false });
        }


        internal async Task BuyStockAsync(string nickname, string stockCode, string stockName, int quantity, decimal close)
        {
            var userCollection = _database.GetCollection<User>("Users");
            var filter = Builders<User>.Filter.Eq(u => u.Nickname, nickname);
            var user = await userCollection.Find(filter).FirstOrDefaultAsync();

            if (user == null)
                throw new Exception("사용자를 찾을 수 없습니다.");

            // 1. 잔액 차감
            user.Balance -= quantity * close;

            // 2. 포트폴리오에 종목이 이미 있으면 수량과 평균단가 갱신
            var existingStock = user.Portfolio.FirstOrDefault(p => p.StockCode == stockCode);
            if (existingStock != null)
            {
                int totalQuantity = existingStock.Quantity + quantity;
                decimal totalCost = (existingStock.AvgPrice * existingStock.Quantity) + (close * quantity);
                existingStock.Quantity = totalQuantity;
                existingStock.AvgPrice = totalCost / totalQuantity;
            }
            else
            {
                user.Portfolio.Add(new Models.OwnedStock
                {
                    StockCode = stockCode,
                    StockName = stockName,
                    Quantity = quantity,
                    AvgPrice = close
                });
            }

            // 3. 거래 내역 추가
            user.Trades.Add(new Models.TradeHistory
            {
                StockCode = stockCode,
                StockName = stockName,
                Quantity = quantity,
                Price = close,
                TradeDate = DateTime.Now,
                Type = "매수"
            });

            // 4. 업데이트 저장
            await userCollection.ReplaceOneAsync(u => u.Nickname == nickname, user);
        }
        public async Task<List<User>> GetAllUsersAsync()
        {
            var collection = _database.GetCollection<User>("Users");
            return await collection.Find(FilterDefinition<User>.Empty).ToListAsync();
        }


        public async Task<decimal> GetLatestClosePriceAsync(string stockCode)
        {
            var filter = Builders<StockPrice>.Filter.Eq(x => x.StockCode, stockCode);
            var stock = await _stockCollection.Find(filter).FirstOrDefaultAsync();

            if (stock == null || stock.Prices == null || stock.Prices.Count == 0)
                return 0;

            var latestPrice = stock.Prices.OrderByDescending(p => p.TradeDate).FirstOrDefault();
            return latestPrice?.Close ?? 0;
        }

    }
}
