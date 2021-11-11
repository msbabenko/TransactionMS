using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransactionMS.Models;

namespace TransactionMS.Services
{
    public interface ITransactionService
    {
        public StatusDTO DepositMoney(DepositDTO depositDTO);
        public StatusDTO WithdrawMoney(DepositDTO depositDTO);
        public StatusDTO Transfer(TransferDTO transferDTO);
        public List<TransactionHistory> getTransactions(int customerId);
    }
}
