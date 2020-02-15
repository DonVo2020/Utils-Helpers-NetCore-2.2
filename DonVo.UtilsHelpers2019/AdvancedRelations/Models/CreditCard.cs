using System;

namespace AdvancedRelations.Models
{
    public class CreditCard
    {
        public int CreditCardId { get; set; }

        public decimal Limit { get; set; }

        public decimal MoneyOwed { get; set; }

        public decimal LimitLeft => Limit - MoneyOwed;

        public DateTime ExpirationDate { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public void Deposit(decimal depositAmount)
        {
            this.MoneyOwed -= depositAmount;
        }

        public void Withdraw(decimal withdrawAmount)
        {
            this.MoneyOwed += withdrawAmount;
        }
    }
}
