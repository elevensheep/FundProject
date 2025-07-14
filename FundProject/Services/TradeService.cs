using FundProject.Models;
using FundProject.Services;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace FundProject.Services
{
    public class TradeService
    {
        private readonly MongoService _mongo;

        public TradeService(MongoService mongo)
        {
            _mongo = mongo;
        }

        public async Task<string> BuyAsync(string nickname, string stockName, int quantity)
        {
            if (quantity <= 0)
                return "❌ 수량은 1 이상이어야 합니다.";

            var user = await _mongo.GetUserByNicknameAsync(nickname);
            if (user == null)
                return "❌ 사용자 정보를 찾을 수 없습니다.";

            var stock = await _mongo.GetStockByNameAsync(stockName);
            if (stock == null || stock.Prices == null || stock.Prices.Count == 0)
                return "❌ 시세 정보를 찾을 수 없습니다.";

            var latestPrice = stock.Prices.OrderByDescending(p => p.TradeDate).First().Close;
            var totalCost = latestPrice * quantity;

            if (user.Balance < totalCost)
                return $"❌ 잔액 부족 (보유: {user.Balance:N0}원, 필요: {totalCost:N0}원)";

            await _mongo.BuyStockAsync(nickname, stock.StockCode, stock.StockName, quantity, latestPrice);

            return $"✅ {stock.StockName} {quantity}주 매수 완료 (가격: {latestPrice:N0}원)";
        }

        public async Task<string> SellAsync(string nickname, string stockName, int quantity)
        {
            if (quantity <= 0)
                return "❌ 수량은 1 이상이어야 합니다.";

            var user = await _mongo.GetUserByNicknameAsync(nickname);
            if (user == null)
                return "❌ 사용자 정보를 찾을 수 없습니다.";

            var stock = await _mongo.GetStockByNameAsync(stockName);
            if (stock == null || stock.Prices == null || stock.Prices.Count == 0)
                return "❌ 시세 정보를 찾을 수 없습니다.";

            var owned = user.Portfolio.FirstOrDefault(s => s.StockCode == stock.StockCode);
            if (owned == null || owned.Quantity < quantity)
                return $"❌ 보유 수량 부족 (보유: {(owned?.Quantity ?? 0)}주)";

            var latestPrice = stock.Prices.OrderByDescending(p => p.TradeDate).First().Close;
            var totalSell = latestPrice * quantity;

            owned.Quantity -= quantity;
            if (owned.Quantity == 0)
                user.Portfolio.Remove(owned);

            user.Balance += totalSell;

            user.Trades.Add(new TradeHistory
            {
                StockCode = stock.StockCode,
                StockName = stock.StockName,
                Quantity = quantity,
                Price = latestPrice,
                TradeDate = DateTime.Now,
                Type = "매도"
            });

            await _mongo.UpdateUserAsync(user);

            return $"✅ {stock.StockName} {quantity}주 매도 완료 (가격: {latestPrice:N0}원)";
        }
    }
}
