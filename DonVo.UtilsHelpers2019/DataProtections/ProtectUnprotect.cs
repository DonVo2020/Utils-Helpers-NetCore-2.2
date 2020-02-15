using System;
using Microsoft.AspNetCore.DataProtection;

namespace DataProtections
{
    public class ProtectUnprotect
    {
        IDataProtector _protector;

        // the 'provider' parameter is provided by DI
        public ProtectUnprotect(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("DonVo.ProtectUnprotect.v1");
        }

        public void RunSample()
        {
            Console.Write("Enter input: ");
            string input = Console.ReadLine();

            // protect the payload
            string protectedPayload = _protector.Protect(input);
            Console.WriteLine($"Protect returned: {protectedPayload}");

            // unprotect the payload
            string unprotectedPayload = _protector.Unprotect(protectedPayload);
            Console.WriteLine($"Unprotect returned: {unprotectedPayload}");
        }
    }
}
