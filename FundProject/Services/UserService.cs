using System.Collections.Generic;
using System.Threading.Tasks;
using FundProject.Models;
using FundProject.Services;

namespace FundProject.Services
{
    public class UserService
    {
        private readonly MongoService _mongo;

        public UserService(MongoService mongoService)
        {
            _mongo = mongoService;
        }

        public async Task<User> GetOrCreateUserAsync(string nickname)
        {
            var user = await _mongo.GetUserByNicknameAsync(nickname);
            if (user == null)
            {
                user = new User
                {
                    Nickname = nickname,
                    Balance = 10000000,
                    Portfolio = new List<OwnedStock>(),
                    Trades = new List<TradeHistory>()
                };
                await _mongo.CreateUserAsync(user);
            }
            return user;
        }

        public async Task UpdatePortfolioPricesAsync(User user)
        {
            bool updated = false;
            foreach (var stock in user.Portfolio)
            {
                var price = await _mongo.GetLatestClosePriceAsync(stock.StockCode);
                if (price > 0)
                {
                    stock.LatestPrice = price;
                    updated = true;
                }
            }

            if (updated)
            {
                await _mongo.UpdateUserAsync(user);
            }
        }
    }
}
