using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransactionMS.Models;
using TransactionMS.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TransactionMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionHistoryController : ControllerBase
    {
        private readonly TransactionService _service;

        public TransactionHistoryController(TransactionService transactionService)
        {
            _service = transactionService;

        }
        //// GET: api/<TransactionHistoryController>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<TransactionHistoryController>/5
        [HttpGet("getTransactions/{id}")]
        public async Task<ActionResult<IList<TransactionHistory>>> Get(int id)
        {
            return _service.getTransactions(id);
        }


        // POST api/<TransactionHistoryController>
        [Route("Deposit")]
        [HttpPost]
        public async Task<ActionResult<string>> Deposit([FromBody] DepositDTO value)
        {
            return _service.DepositMoney(value);
        }

        [Route("Withdraw")]
        [HttpPost]
        public async Task<ActionResult<string>> Withdraw([FromBody] DepositDTO value)
        {
            return _service.WithdrawMoney(value);
        }

        [Route("Transfer")]
        [HttpPost]
        public async Task<ActionResult<StatusDTO>> Transfer([FromBody] TransferDTO value)
        {
            return _service.Transfer(value);
        }

        //// PUT api/<TransactionHistoryController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<TransactionHistoryController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
