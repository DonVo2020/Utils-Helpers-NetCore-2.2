using AdvancedRelations.Models;
using System;
using System.Globalization;

namespace AdvancedRelations.Initializers
{
    internal class CreditCardInitializer
    {
        public static CreditCard[] GetCreditCards()
        {
            CreditCard[] creditCards = new CreditCard[]
            {
                new CreditCard() {Limit = 58394502.324m,MoneyOwed = 3234412.35435m,ExpirationDate = DateTime.ParseExact("18.1.2019","dd.M.yyyy",CultureInfo.InvariantCulture)},
                new CreditCard() {Limit = 324234234.324m,MoneyOwed = 123213.35435m,ExpirationDate = DateTime.ParseExact("19.1.2019","dd.M.yyyy",CultureInfo.InvariantCulture)},
                new CreditCard() {Limit = 234234234.324m,MoneyOwed = 2343242.35435m,ExpirationDate = DateTime.ParseExact("20.1.2019","dd.M.yyyy",CultureInfo.InvariantCulture)}
            };

            return creditCards;
        }
    }
}
