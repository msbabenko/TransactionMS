using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        
        public StatusDTO DepositMoney(DepositDTO depositDTO)
        {
            StatusDTO statusDTO = new();
            //try
            //{
            string status = "";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://account-ms.azurewebsites.net/api/");
                    var postTask = client.PostAsJsonAsync<DepositDTO>("Transaction/Deposit", depositDTO);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var data = result.Content.ReadFromJsonAsync<StatusDTO>();
                        data.Wait();
                        status = data.Result.status;
                        statusDTO = data.Result;
                    }
                }
                UpdateTransactionHistory("DEPOSIT", depositDTO.AccountId, depositDTO.AccountId, depositDTO.Amount, statusDTO.status);

                return statusDTO;
      //  }
            //catch (Exception)
            //{

            //    return "Navve  DOWN";
            //}



}

        public StatusDTO WithdrawMoney(DepositDTO depositDTO)
        {

            StatusDTO statusDTO = new();
            try
            {
                // string status = "";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://account-ms.azurewebsites.net/api/");
                    var postTask = client.PostAsJsonAsync<DepositDTO>("Transaction/Withdraw", depositDTO);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var data = result.Content.ReadFromJsonAsync<StatusDTO>();
                        data.Wait();
                        // status = data.Result.status;
                        statusDTO = data.Result;

                    }
                }
                UpdateTransactionHistory("WITHDRAW", depositDTO.AccountId, depositDTO.AccountId, depositDTO.Amount, statusDTO.status);
                return statusDTO;
            }
            catch (Exception e)
            {
                statusDTO.status = e.Message;
                return statusDTO;
            }


        }

        public StatusDTO Transfer(TransferDTO transferDTO)
        {
            StatusDTO statusDTO = new();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://account-ms.azurewebsites.net/api/");
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
           }
            catch (Exception)
            {
               return null;
            }
            

            if (statusDTO.status=="SUCESS" && statusDTO.ToAccountstatus == "SUCESS")
            {
                UpdateTransactionHistory("TRANSFER", transferDTO.FromAccountId, transferDTO.ToAccountId, transferDTO.Amount, "SUCESS");
                // UpdateTransactionHistory("DEBITED", transferDTO.FromAccountId,transferDTO.ToAccountId,transferDTO.Amount, "SUCESS");
                // UpdateTransactionHistory("CREDITED", transferDTO.FromAccountId, transferDTO.ToAccountId, transferDTO.Amount, "SUCESS");
            }
            else if (statusDTO.status == "INSUFFICIENT FUNDS")
            {
                UpdateTransactionHistory("TRANSFER", transferDTO.FromAccountId, transferDTO.ToAccountId, transferDTO.Amount, "INSUFFICIENT FUNDS");
            }
            else if (statusDTO.status == "From ID NOT FOUND" || statusDTO.status == "TO ID NOT FOUND")
            {
                UpdateTransactionHistory("TRANSFER", transferDTO.FromAccountId, transferDTO.ToAccountId, transferDTO.Amount, statusDTO.status);
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
           
            try
            {
                _Context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ce) 
            {
                Debug.WriteLine(" --- Error: "+ce.Message);
            }
            catch (DbUpdateException ue)
            {
                Debug.WriteLine(" --- Error: " + ue.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(" --- Error: " + e.Message);
            }


        }

        //public List<TransactionHistory> getTransactions(int customerId)
        //{
        //   // /List<TransactionHistory> transactionHistories = null;
        //   // transactionHistories = _Context.TransactionHistories.Where(i => i.ToAccount == customerId || i.FromAccount == customerId ).ToList();
        //   // return transactionHistories;
        //}

       
      public List<TransactionHistory> getTransactions(int customerId)
        {
            List<AccountDTO> CAccounts = GetAccountIds(customerId);
            List<TransactionHistory> transactionHistories = null;

            //transactionHistories = _Context.TransactionHistories.Where(i => i.ToAccount == customerId || i.FromAccount == customerId ).ToList();
            try
            {
                transactionHistories = _Context.TransactionHistories.Where(i => i.ToAccount == CAccounts[0].AccountId || i.ToAccount == CAccounts[1].AccountId || i.FromAccount == CAccounts[0].AccountId || i.FromAccount == CAccounts[1].AccountId).ToList();
            }
            catch (Exception)
            {

                return null;
            }

            return transactionHistories;
        }

        private List<AccountDTO> GetAccountIds(int customerId)
        {
            List<AccountDTO> CAccounts = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://account-ms.azurewebsites.net/api/"); //http://localhost:49937/ //https://account-ms.azurewebsites.net/api/
                var postTask = client.GetAsync("Account/GetAccounts/"+ customerId);
                postTask.Wait();
                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var data = result.Content.ReadFromJsonAsync<List<AccountDTO>>();
                    data.Wait();
                    CAccounts = data.Result;

                }
            }
            return CAccounts;
        }


        public StatusDTO evaluateMinBal(DepositDTO depositDTO)
        {
            StatusDTO statusDTO = new();
            try
            {
               // string status = "";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://account-ms.azurewebsites.net/api/");
                    var postTask = client.PostAsJsonAsync<DepositDTO>("Transaction/Withdraw", depositDTO);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var data = result.Content.ReadFromJsonAsync<StatusDTO>();
                        data.Wait();
                       // status = data.Result.status;
                        statusDTO = data.Result;

                    }
                }
                UpdateTransactionHistory("WITHDRAW", depositDTO.AccountId, depositDTO.AccountId, depositDTO.Amount, statusDTO.status);
                return statusDTO;
            }
            catch (Exception e)
            {
                statusDTO.status = e.Message;
                return statusDTO;
            }


        }

    }
}
