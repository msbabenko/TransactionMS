using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransactionMS.Models
{
    public class DepositDTO
    {
        

        public int AccountId { get; set; }
        public double Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Descriptions { get; set; }
    }
}
