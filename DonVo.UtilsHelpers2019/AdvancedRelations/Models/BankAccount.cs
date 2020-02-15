using System;

namespace AdvancedRelations.Models
{
    public class BankAccount
    {
        public int BankAccountId { get; set; }

        public decimal Balance { get; set; }

        public string BankName { get; set; }

        public string SWIFTCode { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public void Deposit(decimal depositAmount)
        {
            this.Balance += depositAmount;
        }

        public void Withdraw(decimal withdrawAmount)
        {
            if (Balance - withdrawAmount < 0)
            {
                throw new ArgumentException($"Insufficient funds!");
            }

            this.Balance -= withdrawAmount;
        }
    }
}
