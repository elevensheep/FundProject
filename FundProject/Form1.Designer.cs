using System.Drawing;
using System.Windows.Forms;

namespace FundProject
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            this.lstCompanies = new System.Windows.Forms.ListBox();
            this.stockChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.txtNickname = new System.Windows.Forms.TextBox();
            this.btnUserInfo = new System.Windows.Forms.Button();
            this.txtBuyQuantity = new System.Windows.Forms.TextBox();
            this.btnBuy = new System.Windows.Forms.Button();
            this.btnSell = new System.Windows.Forms.Button();
            this.lblNickname = new System.Windows.Forms.Label();
            this.lblBalance = new System.Windows.Forms.Label();
            this.lblStockValue = new System.Windows.Forms.Label();
            this.lblTotalAsset = new System.Windows.Forms.Label();
            this.lstPortfolio = new System.Windows.Forms.ListView();
            this.lstTradeHistory = new System.Windows.Forms.ListView();
            ((System.ComponentModel.ISupportInitialize)(this.stockChart)).BeginInit();
            this.SuspendLayout();

            // 전체 배경색
            this.BackColor = ColorTranslator.FromHtml("#E2E2E2");

            // lstCompanies
            this.lstCompanies.FormattingEnabled = true;
            this.lstCompanies.ItemHeight = 15;
            this.lstCompanies.Location = new System.Drawing.Point(25, 150);
            this.lstCompanies.Size = new System.Drawing.Size(200, 574);
            this.lstCompanies.SelectedIndexChanged += new System.EventHandler(this.lstCompanies_SelectedIndexChanged);

            // stockChart
            chartArea1.AxisX.Interval = 1D;
            chartArea1.AxisX.LabelStyle.Format = "MM-dd";
            chartArea1.AxisX.MajorGrid.LineColor = Color.LightGray;
            chartArea1.AxisY.MajorGrid.LineColor = Color.LightGray;
            chartArea1.AxisY.Minimum = 0D;
            chartArea1.BackColor = Color.White;
            chartArea1.Name = "MainArea";
            this.stockChart.ChartAreas.Add(chartArea1);
            this.stockChart.Location = new System.Drawing.Point(240, 150);
            this.stockChart.Size = new System.Drawing.Size(997, 574);
            this.stockChart.Click += new System.EventHandler(this.stockChart_Click);

            // txtNickname
            this.txtNickname.ForeColor = Color.Gray;
            this.txtNickname.Location = new System.Drawing.Point(25, 30);
            this.txtNickname.Size = new System.Drawing.Size(200, 25);
            this.txtNickname.Text = "닉네임 입력";
            this.txtNickname.GotFocus += new System.EventHandler(this.txtNickname_GotFocus);
            this.txtNickname.LostFocus += new System.EventHandler(this.txtNickname_LostFocus);

            // btnUserInfo
            this.btnUserInfo.Location = new System.Drawing.Point(240, 25);
            this.btnUserInfo.Size = new System.Drawing.Size(160, 40);
            this.btnUserInfo.Text = "사용자 정보 조회";
            this.btnUserInfo.Click += new System.EventHandler(this.btnUserInfo_Click);

            // txtBuyQuantity
            this.txtBuyQuantity.ForeColor = Color.Gray;
            this.txtBuyQuantity.Location = new System.Drawing.Point(25, 98);
            this.txtBuyQuantity.Size = new System.Drawing.Size(200, 25);
            this.txtBuyQuantity.Text = "수량 입력";
            this.txtBuyQuantity.GotFocus += new System.EventHandler(this.txtBuyQuantity_GotFocus);
            this.txtBuyQuantity.LostFocus += new System.EventHandler(this.txtBuyQuantity_LostFocus);

            // btnBuy
            this.btnBuy.Location = new System.Drawing.Point(240, 88);
            this.btnBuy.Size = new System.Drawing.Size(71, 40);
            this.btnBuy.Text = "매수";
            this.btnBuy.Click += new System.EventHandler(this.btnBuy_Click);

            // btnSell
            this.btnSell.Location = new System.Drawing.Point(329, 88);
            this.btnSell.Size = new System.Drawing.Size(71, 40);
            this.btnSell.Text = "매도";
            this.btnSell.Click += new System.EventHandler(this.btnSell_Click);

            // 사용자 정보 라벨
            this.lblNickname.Location = new System.Drawing.Point(1255, 25);
            this.lblNickname.Size = new System.Drawing.Size(300, 24);
            this.lblNickname.ForeColor = Color.Black;

            this.lblBalance.Location = new System.Drawing.Point(1255, 55);
            this.lblBalance.Size = new System.Drawing.Size(300, 24);
            this.lblBalance.ForeColor = Color.Black;

            this.lblStockValue.Location = new System.Drawing.Point(1255, 85);
            this.lblStockValue.Size = new System.Drawing.Size(300, 24);
            this.lblStockValue.ForeColor = Color.Black;

            this.lblTotalAsset.Location = new System.Drawing.Point(1255, 115);
            this.lblTotalAsset.Size = new System.Drawing.Size(300, 24);
            this.lblTotalAsset.ForeColor = Color.Black;

            // lstPortfolio
            this.lstPortfolio.FullRowSelect = true;
            this.lstPortfolio.GridLines = true;
            this.lstPortfolio.Location = new System.Drawing.Point(1255, 150);
            this.lstPortfolio.Size = new System.Drawing.Size(320, 276);
            this.lstPortfolio.View = System.Windows.Forms.View.Details;
            this.lstPortfolio.Columns.Add("종목명", 100, HorizontalAlignment.Left);
            this.lstPortfolio.Columns.Add("보유수량", 100, HorizontalAlignment.Right);
            this.lstPortfolio.Columns.Add("평가금액", 100, HorizontalAlignment.Right);

            // lstTradeHistory
            this.lstTradeHistory.FullRowSelect = true;
            this.lstTradeHistory.GridLines = true;
            this.lstTradeHistory.Location = new System.Drawing.Point(1255, 432);
            this.lstTradeHistory.Size = new System.Drawing.Size(320, 292);
            this.lstTradeHistory.View = System.Windows.Forms.View.Details;
            this.lstTradeHistory.Columns.Add("일자", 100, HorizontalAlignment.Left);
            this.lstTradeHistory.Columns.Add("구분", 60, HorizontalAlignment.Left);
            this.lstTradeHistory.Columns.Add("종목명", 100, HorizontalAlignment.Right);
            this.lstTradeHistory.Columns.Add("수량", 60, HorizontalAlignment.Center);

            // Form1
            this.ClientSize = new System.Drawing.Size(1600, 760);
            this.Controls.Add(this.lstCompanies);
            this.Controls.Add(this.stockChart);
            this.Controls.Add(this.txtNickname);
            this.Controls.Add(this.btnUserInfo);
            this.Controls.Add(this.txtBuyQuantity);
            this.Controls.Add(this.btnBuy);
            this.Controls.Add(this.btnSell);
            this.Controls.Add(this.lblNickname);
            this.Controls.Add(this.lblBalance);
            this.Controls.Add(this.lblStockValue);
            this.Controls.Add(this.lblTotalAsset);
            this.Controls.Add(this.lstPortfolio);
            this.Controls.Add(this.lstTradeHistory);
            this.Name = "Form1";
            this.Text = "모의 주식 투자";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.stockChart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ListBox lstCompanies;
        private System.Windows.Forms.DataVisualization.Charting.Chart stockChart;
        private System.Windows.Forms.TextBox txtNickname;
        private System.Windows.Forms.Button btnUserInfo;
        private System.Windows.Forms.TextBox txtBuyQuantity;
        private System.Windows.Forms.Button btnBuy;
        private System.Windows.Forms.Button btnSell;
        private System.Windows.Forms.Label lblNickname;
        private System.Windows.Forms.Label lblBalance;
        private System.Windows.Forms.Label lblStockValue;
        private System.Windows.Forms.Label lblTotalAsset;
        private System.Windows.Forms.ListView lstPortfolio;
        private System.Windows.Forms.ListView lstTradeHistory;
    }
}
