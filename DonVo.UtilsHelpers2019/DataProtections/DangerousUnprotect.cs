using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;

namespace DataProtections
{
    public class DangerousUnprotect
    {
        public static void Run()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDataProtection()
                // point at a specific folder and use DPAPI to encrypt keys
                .PersistKeysToFileSystem(new DirectoryInfo(@"c:\temp\temp-keys\"))
                .ProtectKeysWithDpapi();
            var services = serviceCollection.BuildServiceProvider();

            // get a protector and perform a protect operation
            var protector = services.GetDataProtector("Sample.DangerousUnprotect");
            Console.Write("Input: ");
            byte[] input = Encoding.UTF8.GetBytes(Console.ReadLine());
            var protectedData = protector.Protect(input);
            Console.WriteLine($"Protected payload: {Convert.ToBase64String(protectedData)}");

            // demonstrate that the payload round-trips properly
            var roundTripped = protector.Unprotect(protectedData);
            Console.WriteLine($"Round-tripped payload: {Encoding.UTF8.GetString(roundTripped)}");

            // get a reference to the key manager and revoke all keys in the key ring
            var keyManager = services.GetService<IKeyManager>();
            Console.WriteLine("Revoking all keys in the key ring...");
            keyManager.RevokeAllKeys(DateTimeOffset.Now, "Sample revocation.");

            // try calling Protect - this should throw
            Console.WriteLine("Calling Unprotect...");
            try
            {
                var unprotectedPayload = protector.Unprotect(protectedData);
                Console.WriteLine($"Unprotected payload: {Encoding.UTF8.GetString(unprotectedPayload)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.GetType().Name}: {ex.Message}");
            }

            // try calling DangerousUnprotect
            Console.WriteLine("Calling DangerousUnprotect...");
            try
            {
                IPersistedDataProtector persistedProtector = protector as IPersistedDataProtector;
                if (persistedProtector == null)
                {
                    throw new Exception("Can't call DangerousUnprotect.");
                }

                bool requiresMigration, wasRevoked;
                var unprotectedPayload = persistedProtector.DangerousUnprotect(
                    protectedData: protectedData,
                    ignoreRevocationErrors: true,
                    requiresMigration: out requiresMigration,
                    wasRevoked: out wasRevoked);
                Console.WriteLine($"Unprotected payload: {Encoding.UTF8.GetString(unprotectedPayload)}");
                Console.WriteLine($"Requires migration = {requiresMigration}, was revoked = {wasRevoked}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
