using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TransactionMS.Models;

namespace TransactionMS.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly TransactionApiDbContext _Context;

        public TransactionService(TransactionApiDbContext transactionApiDbContext)
        {
            _Context = transactionApiDbContext;
        }
        
        public string DepositMoney(DepositDTO depositDTO)
        {
            //try
            //{
                string status = "";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:49937/api/");
                    var postTask = client.PostAsJsonAsync<DepositDTO>("Transaction/Deposit", depositDTO);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var data = result.Content.ReadFromJsonAsync<StatusDTO>();
                        data.Wait();
                        status = data.Result.status;

                    }
                }
                UpdateTransactionHistory("DEPOSIT", 1, depositDTO.AccountId, depositDTO.Amount, status);

                return status;
      //  }
            //catch (Exception)
            //{

            //    return "Navve  DOWN";
            //}



}

        public string WithdrawMoney(DepositDTO depositDTO)
        {
            //try
            //{
                string status = "";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:49937/api/");
                    var postTask = client.PostAsJsonAsync<DepositDTO>("Transaction/Withdraw", depositDTO);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var data = result.Content.ReadFromJsonAsync<StatusDTO>();
                        data.Wait();
                        status = data.Result.status;

                    }
                }
                UpdateTransactionHistory("WITHDRAW", depositDTO.AccountId, 1, depositDTO.Amount, status);
                return status;
           // }
            //catch (Exception)
            //{

            //    return "SERRVER DOWN";
            //}
           
        }

        public StatusDTO Transfer(TransferDTO transferDTO)
        {
            StatusDTO statusDTO = new();
            //try
            //{
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:49937/api/");
                    var postTask = client.PostAsJsonAsync<TransferDTO>("Transaction/Transfer", transferDTO);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var data = result.Content.ReadFromJsonAsync<StatusDTO>();
                        data.Wait();
                        statusDTO = data.Result;

                    }
                }
          //  }
            //catch (Exception)
            //{

            //    return null;
            //}
            

            if (statusDTO.status=="SUCESS" && statusDTO.ToAccountstatus == "SUCESS")
            {
                UpdateTransactionHistory("DEBITED", transferDTO.FromAccountId,transferDTO.ToAccountId,transferDTO.Amount, "SUCESS");
                UpdateTransactionHistory("CREDITED", transferDTO.FromAccountId, transferDTO.ToAccountId, transferDTO.Amount, "SUCESS");
            }
            else if (statusDTO.status == "INSUFFICIENT FUNDS")
            {
                UpdateTransactionHistory("DEBITED", transferDTO.FromAccountId, transferDTO.ToAccountId, transferDTO.Amount, "INSUFFICIENT FUNDS");
            }
            else if (statusDTO.status == "From ID NOT FOUND" || statusDTO.status == "TO ID NOT FOUND")
            {
                UpdateTransactionHistory("DEBITED", transferDTO.FromAccountId, transferDTO.ToAccountId, transferDTO.Amount, statusDTO.status);
            }
            return statusDTO;
        }

        private void UpdateTransactionHistory(string transactionType,int fromAccount,int toAccount,double amount,string transactionStatus)
        {
            TransactionHistory transactionHistory = new();
            transactionHistory.TransactionDate = DateTime.Now;
            transactionHistory.TransactionType = transactionType;
            transactionHistory.FromAccount = fromAccount;
            transactionHistory.ToAccount = toAccount;
            transactionHistory.Amount = amount;
            transactionHistory.TransactionStatus = transactionStatus;
            _Context.TransactionHistories.Add(transactionHistory);
            _Context.SaveChanges();

        }

        public List<TransactionHistory> getTransactions(int customerId)
        {
            List<TransactionHistory> transactionHistories = null;
            transactionHistories = _Context.TransactionHistories.Where(i => i.ToAccount == customerId || i.FromAccount == customerId ).ToList();
            return transactionHistories;
        }
    }
}
