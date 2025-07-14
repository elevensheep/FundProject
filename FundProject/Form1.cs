using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FundProject.Models;
using FundProject.Services;
using FundProject.Utils;
using System.Windows.Forms.DataVisualization.Charting;

namespace FundProject
{
    public partial class Form1 : Form
    {
        private readonly ApiService _apiService = new ApiService();
        private readonly MongoService _mongoService = new MongoService();
        private readonly TradeService _tradeService;
        private readonly UserService _userService;
        private readonly ChartService _chartService = new ChartService();

        public Form1()
        {
            InitializeComponent();
            _tradeService = new TradeService(_mongoService);
            _userService = new UserService(_mongoService);
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                stockChart.GetToolTipText += stockChart_GetToolTipText;
                await LoadStockPricesAsync(); // 시세 자동 불러오기
            }
            catch (Exception ex)
            {
                MessageBox.Show("시세 자동 로딩 실패: " + ex.Message);
            }
        }

        private async Task LoadStockPricesAsync()
        {
            try
            {
                var targetDates = DateHelper.GetRecentTradeDates();
                var allStockData = new List<StockPrice>();

                foreach (var date in targetDates)
                {
                    string strDate = date.ToString("yyyyMMdd");
                    var data = await _apiService.FetchStockPricesAsync(strDate);
                    allStockData.AddRange(data);
                }

                foreach (var group in allStockData.GroupBy(s => new { s.StockCode, s.StockName }))
                {
                    var mergedPrices = group.SelectMany(s => s.Prices).ToList();

                    var distinctPrices = mergedPrices
                        .GroupBy(p => p.TradeDate)
                        .Select(g => g.First())
                        .OrderBy(p => p.TradeDate)
                        .ToList();

                    await _mongoService.InsertOrUpdatePricesAsync(distinctPrices, group.Key.StockCode, group.Key.StockName);
                }

                MessageBox.Show($"{targetDates.Min():yyyy-MM-dd} ~ {targetDates.Max():yyyy-MM-dd} 기준 {allStockData.Count}건 저장 완료!");

                var allNames = await _mongoService.GetAllCompanyNamesAsync();
                lstCompanies.Items.Clear();
                foreach (var name in allNames)
                    lstCompanies.Items.Add(name);

                var users = await _mongoService.GetAllUsersAsync();
                foreach (var user in users)
                    await _userService.UpdatePortfolioPricesAsync(user);

                if (!string.IsNullOrEmpty(txtNickname.Text))
                    btnUserInfo_Click(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show("오류 발생: " + ex.Message);
            }
        }

        private async void btnUserInfo_Click(object sender, EventArgs e)
        {
            string nickname = txtNickname.Text.Trim();
            if (string.IsNullOrEmpty(nickname))
            {
                MessageBox.Show("닉네임을 입력하세요.");
                return;
            }

            var user = await _userService.GetOrCreateUserAsync(nickname);
            await _userService.UpdatePortfolioPricesAsync(user);

            decimal totalStockValue = 0;
            lstPortfolio.Items.Clear();
            foreach (var stock in user.Portfolio)
            {
                var close = await _mongoService.GetLatestClosePriceAsync(stock.StockCode);
                if (close == 0) close = stock.AvgPrice;
                decimal value = stock.Quantity * close;
                totalStockValue += value;

                var item = new ListViewItem(stock.StockName);
                item.SubItems.Add(stock.Quantity.ToString());
                item.SubItems.Add($"{stock.AvgPrice:N0}");
                item.SubItems.Add($"{close:N0}");
                item.SubItems.Add($"{value:N0}");
                lstPortfolio.Items.Add(item);
            }

            decimal totalAsset = user.Balance + totalStockValue;

            lblNickname.Text = $"👤 닉네임: {user.Nickname}";
            lblBalance.Text = $"💰 현금: {user.Balance:N0}원";
            lblStockValue.Text = $"📈 평가액: {totalStockValue:N0}원";
            lblTotalAsset.Text = $"🧾 총 자산: {totalAsset:N0}원";

            lstTradeHistory.Items.Clear();
            foreach (var trade in user.Trades.OrderByDescending(t => t.TradeDate).Take(5))
            {
                var item = new ListViewItem(trade.TradeDate.ToString("MM-dd"));
                item.SubItems.Add(trade.Type);
                item.SubItems.Add(trade.StockName);
                item.SubItems.Add($"{trade.Quantity}주");
                item.SubItems.Add($"{trade.Price:N0}원");
                lstTradeHistory.Items.Add(item);
            }
        }

        private async void btnBuy_Click(object sender, EventArgs e)
        {
            string nickname = txtNickname.Text.Trim();
            string stockName = lstCompanies.SelectedItem?.ToString();
            if (!int.TryParse(txtBuyQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("구매 수량을 올바르게 입력하세요.");
                return;
            }

            var result = await _tradeService.BuyAsync(nickname, stockName, quantity);
            MessageBox.Show(result);
            btnUserInfo_Click(this, EventArgs.Empty);
        }

        private async void btnSell_Click(object sender, EventArgs e)
        {
            string nickname = txtNickname.Text.Trim();
            string stockName = lstCompanies.SelectedItem?.ToString();
            if (!int.TryParse(txtBuyQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("매도 수량을 올바르게 입력하세요.");
                return;
            }

            var result = await _tradeService.SellAsync(nickname, stockName, quantity);
            MessageBox.Show(result);
            btnUserInfo_Click(this, EventArgs.Empty);
        }

        private async void lstCompanies_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedName = lstCompanies.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedName)) return;

            try
            {
                var stock = await _mongoService.GetStockByNameAsync(selectedName);
                if (stock == null || stock.Prices == null || stock.Prices.Count < 1)
                {
                    MessageBox.Show("해당 종목의 데이터가 없습니다.");
                    return;
                }

                // ❌ 잘못된 필터링 제거 → 전체 데이터를 넘기고
                // ✅ 필터링은 ChartService에서 날짜 기준으로 처리
                _chartService.DrawStockChart(stockChart, stock.Prices);
            }
            catch (Exception ex)
            {
                MessageBox.Show("그래프 로딩 실패: " + ex.Message);
            }
        }



        private void txtNickname_GotFocus(object sender, EventArgs e)
        {
            if (txtNickname.Text == "닉네임 입력")
            {
                txtNickname.Text = "";
                txtNickname.ForeColor = System.Drawing.Color.Black;
            }
        }

        private void txtNickname_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNickname.Text))
            {
                txtNickname.Text = "닉네임 입력";
                txtNickname.ForeColor = System.Drawing.Color.Gray;
            }
        }

        private void stockChart_Click(object sender, EventArgs e) { }

        private void stockChart_GetToolTipText(object sender, ToolTipEventArgs e)
        {
            if (e.HitTestResult.ChartElementType == ChartElementType.DataPoint)
            {
                var point = e.HitTestResult.Series.Points[e.HitTestResult.PointIndex];
                e.Text = point.ToolTip;
            }
        }

        private void txtBuyQuantity_GotFocus(object sender, EventArgs e)
        {
            if (txtBuyQuantity.Text == "수량 입력")
            {
                txtBuyQuantity.Text = "";
                txtBuyQuantity.ForeColor = System.Drawing.Color.Black;
            }
        }

        private void txtBuyQuantity_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBuyQuantity.Text))
            {
                txtBuyQuantity.Text = "수량 입력";
                txtBuyQuantity.ForeColor = System.Drawing.Color.Gray;
            }
        }
        
    }
}
