using AdvancedRelations.Initializers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdvancedRelations
{
    public class Initialize
    {
        public static void Seed(BillsPaymentSystemContext context)
        {
            InsertUsers(context);
            InsertCreditCards(context);
            InsertBankAccounts(context);
            InsertPaymentMethods(context);
        }

        private static void InsertPaymentMethods(BillsPaymentSystemContext context)
        {
            var paymentMethods = PaymentMethodInitializer.GetPaymentMethods();

            foreach (var pm in paymentMethods)
            {
                if (IsValid(pm))
                {
                    context.PaymentMethods.Add(pm);
                }
            }

            context.SaveChanges();
        }

        private static void InsertBankAccounts(BillsPaymentSystemContext context)
        {
            var bankAccounts = BankAccountInitializer.GetBankAccounts();

            foreach (var ba in bankAccounts)
            {
                if (IsValid(ba))
                {
                    context.BankAccounts.Add(ba);
                }
            }

            context.SaveChanges();
        }

        private static void InsertCreditCards(BillsPaymentSystemContext context)
        {
            var creditCards = CreditCardInitializer.GetCreditCards();

            foreach (var cd in creditCards)
            {
                if (IsValid(cd))
                {
                    context.CreditCards.Add(cd);
                }
            }

            context.SaveChanges();
        }

        private static void InsertUsers(BillsPaymentSystemContext context)
        {
            var users = UserInitializer.GetUsers();

            foreach (var u in users)
            {
                if (IsValid(u))
                {
                    context.Users.Add(u);
                }
            }

            context.SaveChanges();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new ValidationContext(obj);
            var result = new List<ValidationResult>();

            return Validator.TryValidateObject(obj, validationContext, result);
        }
    }
}
