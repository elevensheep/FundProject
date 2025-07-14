using System;
using System.Collections.Generic;

namespace FundProject.Utils
{
    public static class DateHelper
    {
        public static string GetProperTradeDate()
        {
            var today = DateTime.Today;

            if (today.DayOfWeek == DayOfWeek.Monday)
            {
                return today.AddDays(-3).ToString("yyyyMMdd");
            }
            else
            {
                return today.AddDays(-1).ToString("yyyyMMdd");
            }
        }

        public static List<DateTime> GetRecentTradeDates(int count = 5)
        {
            var dates = new List<DateTime>();
            var date = DateTime.Today;

            while (dates.Count < count)
            {
                date = date.AddDays(-1);

                // 주말 제외
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    dates.Add(date);
                }
            }

            return dates;
        }
    }
}
