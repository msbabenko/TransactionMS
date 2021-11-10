using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransactionMS.Models
{
    public class AccountDTO
    {
        public int AccountId { get; set; }
        public int? CustomerId { get; set; }
        public string AccountType { get; set; }
        public double? Balance { get; set; }
    }
}
