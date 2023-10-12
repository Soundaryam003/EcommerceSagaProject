using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class OrderRequest
    {
        public int OrderId { get; set; }

        public int CatalogueId { get; set; }
        public int Units { get; set; }


        public string ProductName { get; set; }


    }
}
