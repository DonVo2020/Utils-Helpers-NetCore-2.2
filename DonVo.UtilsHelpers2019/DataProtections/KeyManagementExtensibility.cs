﻿using System;
using System.IO;
using System.Xml.Linq;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DataProtections
{
    public class KeyManagementExtensibility
    {
        public static void Run()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(@"c:\temp\temp-keys"))
                .ProtectKeysWithDpapi()
                .AddKeyEscrowSink(sp => new MyKeyEscrowSink(sp));
            var services = serviceCollection.BuildServiceProvider();

            // get a reference to the key manager and force a new key to be generated
            Console.WriteLine("Generating new key...");
            var keyManager = services.GetService<IKeyManager>();
            keyManager.CreateNewKey(
                activationDate: DateTimeOffset.Now,
                expirationDate: DateTimeOffset.Now.AddDays(7));
        }

        // A key escrow sink where keys are escrowed such that they
        // can be read by members of the CONTOSO\Domain Admins group.
        private class MyKeyEscrowSink : IKeyEscrowSink
        {
            private readonly IXmlEncryptor _escrowEncryptor;

            public MyKeyEscrowSink(IServiceProvider services)
            {
                // Assuming I'm on a machine that's a member of the CONTOSO
                // domain, I can use the Domain Admins SID to generate an
                // encrypted payload that only they can read. Sample SID from
                // https://technet.microsoft.com/library/cc778824(v=ws.10).aspx.
                _escrowEncryptor = new DpapiNGXmlEncryptor(
                    "SID=S-1-5-21-1063312426-1441049234-2481596400-1001",
                    DpapiNGProtectionDescriptorFlags.None,
                    new LoggerFactory());
            }

            public void Store(Guid keyId, XElement element)
            {
                // Encrypt the key element to the escrow encryptor.
                var encryptedXmlInfo = _escrowEncryptor.Encrypt(element);

                // A real implementation would save the escrowed key to a
                // write-only file share or some other stable storage, but
                // in this sample we'll just write it out to the console.
                Console.WriteLine($"Escrowing key {keyId}");
                Console.WriteLine(encryptedXmlInfo.EncryptedElement);

                // Note: We cannot read the escrowed key material ourselves.
                // We need to get a member of CONTOSO\Domain Admins to read
                // it for us in the event we need to recover it.
            }
        }
    }
}
