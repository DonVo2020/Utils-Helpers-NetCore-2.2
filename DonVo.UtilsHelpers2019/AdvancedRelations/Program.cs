using AdvancedRelations.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AdvancedRelations
{
    class Program
    {
        static void Main(string[] args)
        {
            using (BillsPaymentSystemContext context = new BillsPaymentSystemContext())
            {
                //context.Database.EnsureDeleted();
                //context.Database.EnsureCreated();

                //Initialize.Seed(context);

                try
                {
                    User currentUser = GetUser(context);

                    PrintUserData(currentUser);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                Console.Write("\n\nPlease key UserId (such as 1, 2, or 3) and hit Enter: ");
                int userId = int.Parse(Console.ReadLine());
                Console.Write("\n\nPlease key amount (such as 20936) and hit Enter: ");
                decimal amount = decimal.Parse(Console.ReadLine());

                PayBills(userId, amount, context);

                context.SaveChanges();
            }
        }

        private static void PayBills(int userId, decimal amount, BillsPaymentSystemContext context)
        {
            User user = context.Users
                .Where(x => x.UserId == userId)
                .Include(x => x.PaymentMethods).ThenInclude(x => x.BankAccount)
                .Include(x => x.PaymentMethods).ThenInclude(x => x.CreditCard)
                .FirstOrDefault();

            decimal userMoney = CalculateMoney(user);

            if (userMoney >= amount)
            {
                var bankAccounts = user.PaymentMethods.Where(x => x.BankAccount != null).Select(x => x.BankAccount).ToArray();

                foreach (var ba in bankAccounts.OrderBy(x => x.BankAccountId))
                {
                    if (ba.Balance >= amount)
                    {
                        ba.Withdraw(amount);
                        amount = 0;
                    }
                    else
                    {
                        ba.Withdraw(ba.Balance);
                        amount -= ba.Balance;
                    }

                    if (amount == 0)
                    {
                        break;
                    }
                }

                var creditCards = user.PaymentMethods.Where(x => x.CreditCard != null).Select(x => x.CreditCard).ToArray();

                foreach (var cc in creditCards.OrderBy(x => x.CreditCardId))
                {
                    if (cc.LimitLeft >= amount)
                    {
                        cc.Withdraw(amount);
                        amount = 0;
                    }
                    else
                    {
                        cc.Withdraw(cc.LimitLeft);
                        amount -= cc.LimitLeft;
                    }

                    if (amount == 0)
                    {
                        break;
                    }
                }
            }
            else
            {
                Console.WriteLine($"Insufficient amount");
            }
        }

        private static decimal CalculateMoney(User user)
        {
            var bankAccountsMoney = user.PaymentMethods.Where(x => x.BankAccount != null).Sum(x => x.BankAccount.Balance);
            var creditCardsMoney = user.PaymentMethods.Where(x => x.CreditCard != null).Sum(x => x.CreditCard.LimitLeft);

            return bankAccountsMoney + creditCardsMoney;
        }

        private static void PrintUserData(User currentUser)
        {
            Console.WriteLine($"User: {currentUser.FirstName} {currentUser.LastName}");

            Console.WriteLine("Bank Accounts:");

            var bankAccounts = currentUser.PaymentMethods.Where(x => x.BankAccount != null).Select(x => x.BankAccount).ToArray();

            foreach (var ba in bankAccounts)
            {
                Console.WriteLine($"-- ID: {ba.BankAccountId}");
                Console.WriteLine($"--- Balance: {ba.Balance}");
                Console.WriteLine($"--- Bank: {ba.BankName}");
                Console.WriteLine($"--- SWIFT: {ba.SWIFTCode}");
            }

            var creditCards = currentUser.PaymentMethods.Where(x => x.CreditCard != null).Select(x => x.CreditCard).ToArray();

            Console.WriteLine($"Credit Cards:");
            foreach (var c in creditCards)
            {
                Console.WriteLine($"-- ID: {c.CreditCardId}");
                Console.WriteLine($"--- Limit: {c.Limit}");
                Console.WriteLine($"--- Money Owed: {c.MoneyOwed}");
                Console.WriteLine($"--- Limit Left: {c.LimitLeft}");
                Console.WriteLine($"--- Expiration Date: {c.ExpirationDate}");
            }
        }

        private static User GetUser(BillsPaymentSystemContext context)
        {
            Console.Write("\n\nPlease key UserId (such as 1, 2, or 3) and hit Enter: ");
            int userId = int.Parse(Console.ReadLine());

            var currentUser = context.Users
                .Include(x => x.PaymentMethods).ThenInclude(x => x.BankAccount).
                Include(x => x.PaymentMethods).ThenInclude(x => x.CreditCard)
                .FirstOrDefault(x => x.UserId == userId);

            if (currentUser == null)
            {
                throw new ArgumentException($"User with Id {userId} not found!");
            }

            return currentUser;
        }
    }
}
