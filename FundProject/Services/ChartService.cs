using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using FundProject.Models;

namespace FundProject.Services
{
    public class ChartService
    {
        public void DrawStockChart(Chart chart, List<PriceEntry> prices)
        {
            if (prices == null || prices.Count == 0)
                return;

            // ✅ 가장 최근 5개 거래일 날짜만 추출
            var recentDates = prices
                .Select(p => p.TradeDate.Date)
                .Distinct()
                .OrderByDescending(d => d)
                .Take(5)
                .OrderBy(d => d) // 날짜 순서대로
                .ToList();

            // ✅ 해당 날짜들에 해당하는 데이터만 필터링
            var filtered = prices
                .Where(p => recentDates.Contains(p.TradeDate.Date))
                .OrderBy(p => p.TradeDate)
                .ToList();

            if (filtered.Count == 0)
                return;

            chart.Series.Clear();
            chart.ChartAreas.Clear();
            chart.Legends.Clear();

            var chartArea = new ChartArea("MainArea")
            {
                BackColor = Color.White
            };

            // ✅ 문자열 기반 X축
            chartArea.AxisX.MajorGrid.Enabled = false;
            chartArea.AxisX.LabelStyle.Angle = -45;
            chartArea.AxisX.Interval = 1;
            chartArea.AxisX.Title = "날짜";

            chartArea.AxisY.MajorGrid.LineWidth = 0;
            chartArea.AxisY.Title = "가격";

            chart.BackColor = Color.White;
            chart.ChartAreas.Add(chartArea);

            var legend = new Legend("MainLegend");
            chart.Legends.Add(legend);

            var barSeries = new Series("시가-종가 변화")
            {
                ChartType = SeriesChartType.RangeColumn,
                XValueType = ChartValueType.String,
                ChartArea = "MainArea",
                Legend = "MainLegend"
            };
            barSeries["PointWidth"] = "1.0";

            var highSeries = new Series("고가 (High)")
            {
                ChartType = SeriesChartType.Line,
                XValueType = ChartValueType.String,
                BorderWidth = 2,
                Color = Color.Orange,
                ChartArea = "MainArea",
                Legend = "MainLegend",
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 6
            };

            var lowSeries = new Series("저가 (Low)")
            {
                ChartType = SeriesChartType.Line,
                XValueType = ChartValueType.String,
                BorderWidth = 2,
                Color = Color.Green,
                ChartArea = "MainArea",
                Legend = "MainLegend",
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 6
            };

            var flatSeries = new Series("변동없음 (Flat)")
            {
                ChartType = SeriesChartType.Point,
                XValueType = ChartValueType.String,
                Color = Color.Gray,
                MarkerStyle = MarkerStyle.Cross,
                MarkerSize = 8,
                ChartArea = "MainArea",
                Legend = "MainLegend"
            };

            foreach (var entry in filtered)
            {
                string label = entry.TradeDate.ToString("MM-dd");

                double open = (double)entry.Open;
                double close = (double)entry.Close;
                double high = (double)entry.High;
                double low = (double)entry.Low;

                highSeries.Points.AddXY(label, high);
                lowSeries.Points.AddXY(label, low);

                if (open == close)
                {
                    flatSeries.Points.AddXY(label, open);
                }
                else
                {
                    var dp = new DataPoint
                    {
                        AxisLabel = label,
                        YValues = new double[] { Math.Min(open, close), Math.Max(open, close) },
                        Color = close >= open ? Color.Red : Color.Blue
                    };
                    barSeries.Points.Add(dp);
                }
            }

            chart.Series.Add(barSeries);
            chart.Series.Add(highSeries);
            chart.Series.Add(lowSeries);
            chart.Series.Add(flatSeries);
        }
    }
}
