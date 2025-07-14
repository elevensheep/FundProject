using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using FundProject.Models;
using DotNetEnv;
using System.IO;

namespace FundProject.Services
{
    public class ApiService
    {
        private readonly HttpClient _client = new HttpClient();

        public async Task<List<StockPrice>> FetchStockPricesAsync(string date)
        {
            DotNetEnv.Env.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../.env"));

            var connectionString = Environment.GetEnvironmentVariable("API_KEY");

            string url = $"https://apis.data.go.kr/1160100/service/GetStockSecuritiesInfoService/getStockPriceInfo?" +
                         $"serviceKey={connectionString}" +
                         $"&numOfRows=1000" +
                         $"&pageNo=1" +
                         $"&resultType=xml" +
                         $"&basDt={date}";

            var response = await _client.GetStringAsync(url);
            var xml = XDocument.Parse(response);

            // 1일 기준 개별 항목 리스트
            var flatList = xml.Descendants("item")
                .Select(x => new
                {
                    StockCode = x.Element("srtnCd")?.Value,
                    StockName = x.Element("itmsNm")?.Value,
                    PriceEntry = new PriceEntry
                    {
                        TradeDate = DateTime.ParseExact(x.Element("basDt")?.Value ?? "", "yyyyMMdd", null),
                        Open = decimal.Parse(x.Element("mkp")?.Value ?? "0"),
                        Close = decimal.Parse(x.Element("clpr")?.Value ?? "0"),
                        High = decimal.Parse(x.Element("hipr")?.Value ?? "0"),
                        Low = decimal.Parse(x.Element("lopr")?.Value ?? "0"),
                        Volume = long.Parse(x.Element("trqu")?.Value ?? "0")
                    }
                })
                .ToList();

            // 종목별로 그룹화하여 StockPrice 객체 생성
            var grouped = flatList
                .GroupBy(x => new { x.StockCode, x.StockName })
                .Select(g => new StockPrice
                {
                    StockCode = g.Key.StockCode,
                    StockName = g.Key.StockName,
                    Prices = g.Select(x => x.PriceEntry).ToList()
                })
                .ToList();

            return grouped;
        }
    }
}
