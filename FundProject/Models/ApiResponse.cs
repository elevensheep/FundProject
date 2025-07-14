using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundProject.Models
{
    public class ApiResponse
    {
        public Response response { get; set; }
    }

    public class Response
    {
        public Body body { get; set; }
    }

    public class Body
    {
        public Items items { get; set; }
    }

    public class Items
    {
        public List<StockItem> item { get; set; }
    }

    public class StockItem
    {
        public string srtnCd { get; set; }
        public string itmsNm { get; set; }
        public string basDt { get; set; }
        public string mkp { get; set; }
        public string clpr { get; set; }
        public string hipr { get; set; }
        public string lopr { get; set; }
        public string trqu { get; set; }
    }
}
