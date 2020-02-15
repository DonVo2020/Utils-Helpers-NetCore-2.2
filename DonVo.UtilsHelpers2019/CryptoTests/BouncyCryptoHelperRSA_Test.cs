using Crypto.Algorithms.RSA;
using Xunit;

namespace CryptoTests
{
    public class BouncyCryptoHelperTest
    {
        [Fact]
        public void ServerRsaEncryptionDecryptionTest()
        {
            var bouncyCryptoHelper = new BouncyCryptoHelper();
            var keys = bouncyCryptoHelper.GenerateKeyPair();

            var input = "Example message for decryption/encryption! With spec symbol ™";
            var publicKey = keys.Item1;
            var privateKey = keys.Item2;

            var encryptedMessage = bouncyCryptoHelper.EncryptMessage(input, publicKey);

            var output = bouncyCryptoHelper.DecryptMessage(encryptedMessage, privateKey);

            Assert.Equal(input, output);
        }

        [Fact]
        public void ServerRsaVerificationTest()
        {
            var bouncyCryptoHelper = new BouncyCryptoHelper();
            var keys = bouncyCryptoHelper.GenerateKeyPair();

            var input = "Example message for decryption/encryption! With spec symbol ™";
            var publicKey = keys.Item1;
            var privateKey = keys.Item2;

            var encryptedMessage = bouncyCryptoHelper.SignMessage(input, privateKey);
            Assert.True(bouncyCryptoHelper.VerifyMessage(input, encryptedMessage, publicKey));
        }



        [Fact]
        public void MobileReactNativeRsaNativeEncryptionDecryptionTest()
        {
            var bouncyCryptoHelper = new BouncyCryptoHelper();
            var input = "Example message for decryption/encryption! With spec symbol ™";
            var mobilePublicKey =
                "-----BEGIN RSA PUBLIC KEY-----\n" +
                "MIICCgKCAgEApx7x891JbXsJETEQNqzy67Wu3avqK5KDVTwepWrMa+/9tRMeHgka\n" +
                "hYKlFmf7kX3BHXiskWAsyB3y83JernX+jl3sbM9WHwi9AVbjJMnVrP2gCeY/OO33\n" +
                "waKg+D9xQzEdvGa5p9jZ2pMWNV0Bco3lHMM35RhyBGZACzolflUSJfNbqADIEsgz\n" +
                "2pjuz2ulz1zEvzkQgUAnG+uQUayX9W+4icmTnrNh3rj/+UfF7N82fegkK+oNy83A\n" +
                "fl3Os5kgIa3i06uJwFWFb+Tp4HX2vu01e9Kseofzo9nyYX5kDvyhTQl7t/2js/ZX\n" +
                "OJ3amFUQldKeIWzU1ZwTD3IDBVDwtK//Bx3eRtPUq0v53liCVbfhcJ6YZxpROxcY\n" +
                "LFTejtjZvdbq6bI2GYAFiiFSh066PFrFQHfgMv0/sQZ56M7JJWFgV9tQbkDdYQ7Y\n" +
                "uadjhQck040nDxu6fcqaXW9yH1Lr1bZYmv5LZHrFhQo2Ucy8wgnWVKvbWzLwkSJu\n" +
                "Te7hEzRS+bcISYYL8yBndUH3ZJBnn98gDhZBDYILUzNDhBZZB9bKsNxD0I7j/ofK\n" +
                "xamEZKA7SYGOpdqmJVypWMEDUFd49BqYo4/94iVgFUAzg+ypc8lLxC8CFPrunkmL\n" +
                "QfxyoZ50yB3OvKl3pR9Uk7n4+aBBPYRDEqOosZ71ARzW3iBnGSnlsmECAwEAAQ==\n" +
                "-----END RSA PUBLIC KEY-----\n";

            var mobilePrivateKey =
                "-----BEGIN RSA PRIVATE KEY-----\n" +
                "MIIJKgIBAAKCAgEApx7x891JbXsJETEQNqzy67Wu3avqK5KDVTwepWrMa+/9tRMe\n" +
                "HgkahYKlFmf7kX3BHXiskWAsyB3y83JernX+jl3sbM9WHwi9AVbjJMnVrP2gCeY/\n" +
                "OO33waKg+D9xQzEdvGa5p9jZ2pMWNV0Bco3lHMM35RhyBGZACzolflUSJfNbqADI\n" +
                "Esgz2pjuz2ulz1zEvzkQgUAnG+uQUayX9W+4icmTnrNh3rj/+UfF7N82fegkK+oN\n" +
                "y83Afl3Os5kgIa3i06uJwFWFb+Tp4HX2vu01e9Kseofzo9nyYX5kDvyhTQl7t/2j\n" +
                "s/ZXOJ3amFUQldKeIWzU1ZwTD3IDBVDwtK//Bx3eRtPUq0v53liCVbfhcJ6YZxpR\n" +
                "OxcYLFTejtjZvdbq6bI2GYAFiiFSh066PFrFQHfgMv0/sQZ56M7JJWFgV9tQbkDd\n" +
                "YQ7YuadjhQck040nDxu6fcqaXW9yH1Lr1bZYmv5LZHrFhQo2Ucy8wgnWVKvbWzLw\n" +
                "kSJuTe7hEzRS+bcISYYL8yBndUH3ZJBnn98gDhZBDYILUzNDhBZZB9bKsNxD0I7j\n" +
                "/ofKxamEZKA7SYGOpdqmJVypWMEDUFd49BqYo4/94iVgFUAzg+ypc8lLxC8CFPru\n" +
                "nkmLQfxyoZ50yB3OvKl3pR9Uk7n4+aBBPYRDEqOosZ71ARzW3iBnGSnlsmECAwEA\n" +
                "AQKCAgAF2jeHeRd9ZWEoswGHY/iuUIqZrJpq2/VsMSqoE48/f5Ql3hAQZcnMr4nB\n" +
                "OP3mRzjRmRnJo1byIPAH/o3KWYC4eC7OPoJchsbFUsdUe5p3YEWKrzTbKnniNAQj\n" +
                "1BHu9CZ/eWVKl4Aaj3XSw37oWx0ma+o2TuwNk3iY7rh5001rRY757qFoXKPfOvrl\n" +
                "nzZmESXlMJYEwWFqkNi5GXyT/bgg9Xg3/FNtiag80NqsLBgZYj1OXzgMNph6pxA/\n" +
                "jxmrqu5DcsbZn3e+AUPU8StFc022YucYyTpwTEgOCg9uNxfdeHsWsy54oARHgT9S\n" +
                "qsDweY5BgKaL6UyBaRZUvnQYJG/Uym/8BU59NviCdJCXyT49cBfdgt1QGdBuBdf7\n" +
                "SA57sKeg0ovvF2Dla3bVdWbHUqpIMhwBxK9pGFaOwDnYXH5gXLcpykh6mP0QGD4g\n" +
                "T0AK+FPhXTAcNX8Z9dD6W28fkljYvpT8Rw2LRfnAk+M+N5mZByG1NkR5H1cIWykN\n" +
                "d2T9UpBgBNCHfQ3wYtnGfHUghczpcF4CF/gNdbBrrxp5S0/8kmY0eUosiMBkRmXH\n" +
                "vhKl46TBe92kf18tYujc0wPzHSZX8bdWSx5djIS6tChc6MPBtOApe3ePahb2CIvH\n" +
                "PRPlkqVeep/A+UbK8y5+z9TwCD8ZZoVROovRo/sq47yRE606iQKCAQEA0575besm\n" +
                "vCy1ZhR2dD5hZcUhN5wYYN5EiI7Mufp7rJ7/J/oKSGFjtqR4i6legQzcGLvFfVON\n" +
                "R5ZrtiSMAw+3PwNKjVOEvjPBt8TQX/hovtYLsDwvYfUr4qlk9PXIgkDZwjnQSyi3\n" +
                "NV6s6OCghuqjTzsXLZN1q2HftA+blnEA7VL3zN7YnjBvHOr2eP3FvRDhDJjVivfD\n" +
                "zj8+9rVbppNYVSV9YFsKhoIRBIEyEjyzxemXf7FZPGGrd6DBw+6rHsJ9CfDR2pSI\n" +
                "Ndb8MrCWT9MZ4c/R0HqQhNqGf++eTPCKo7N9AVaDYUha2niB6aRT5kSlNybi65ZN\n" +
                "fDESsUTRT0IBOQKCAQEAyirzOAUKBABeBSPdZUFo4Eo6gW/WbJprUOyFQ5UpGUYR\n" +
                "N5yZnA+SVoSNkkyKkegAAxOd3/FLkmvFxfRNVNgkfPRTbz46WdaXjhrqD0GynQfE\n" +
                "d+3Jyeam5k0SDRMhsG/ruPuFyHs5pYaiJXHrU9KrVgckM239n9jtFyhFrH2eIhl1\n" +
                "akV+UDa5UFVKqfiCxN3CE7KIFbKfJ+dJg1L//Kt1UbCtNVT/X0+EJpbAQZbVd4fM\n" +
                "bGO9Qanb0aydfY77wqkeIx3YbVZzuAPWKOL/4ZIVAw6TF4XtmUtzLAjbtUwBOCvd\n" +
                "O7DxqcXLJMlbti0V4UJiOAPO7gAApfZ4TVYHyBbCaQKCAQEAsKQSL94JBymKgRqz\n" +
                "hXRMeFaD05dIAhOFwTbxTIv4j65n7UaZLrGcOegvduR5Ld8+GKcjwejsFMVKQ09e\n" +
                "T2/e1Yo5wJ3mP52UJYDzLih7xk+EkiaqpmmvqBMdYhuGsrLPEYCQjSv/QV01l21o\n" +
                "kTVp+8inIDPmFJgt0m70wC6zR2Xp0ehxiBmFpe26rmOlmptsPseT99u4ZYAFXokj\n" +
                "Z3e2U9xnOvbDYmNPMSNmWDKzHJBPFRdEFVKxfbmCA7pEu/g7xwBtKrawu8An42fj\n" +
                "D951zDdcO1kTKNH/UusAh9iA4QQBZAyCvcFXlOTiC9RO8/Z4sgCTiXJ8Wx3bLHz0\n" +
                "+eNugQKCAQEAxI55pk+P61AIGWtDR70OoHLK6ym3CiQgjq9G6exN5xquhqkk2Axz\n" +
                "tNZvZnwwhadQy6z449AQL63Eva/Gt+TlpO9PZ2rTBKGU/D7Crk6rv+zbYda5SCmO\n" +
                "v37TPA8LxCUsw88XxVG5PGvnngxiRj8fAiczVSVtX0pjSQ6InyoK4xBpjgc+xfJO\n" +
                "vgIO866ARbNtjAUaNi5Se1Ntr9a0uA5jpCboYF/hYeNHktUi2yIxLWPEpuVaDkt9\n" +
                "QyBWhsrLj/kpKe9MixFwtMpWH66EJeYHs88px8cYuOAYfAmow13AGcLb97sTWoLJ\n" +
                "/VwgZMD08uu93sQlHlS2TKWTVW19Z1ZpcQKCAQEAizKF1I/Oji0B8IbKm3SjsIyw\n" +
                "/3SI/XSDKpyyWayBsM0SczBf4lXSSTj7uY4Yn/68V6YJnTLygehUf39rYN154bIE\n" +
                "qrpUMu840LDV6+NVzaNaUKdKIGnDMRarqkjQPc9ZizwAzEr7bbGGtlTwa47F67WY\n" +
                "RfqJ34pf3EOQvoD2Lai8QLLI70+rGDEYmJnnja19j91N+VFwS3jTk8OQAbkI15bp\n" +
                "LSaVPizZ8CvXPmqvEnJrimcla3uOiqB8dpiCLdQMV/IJSVT4M5vmVGN8u3zsj3YJ\n" +
                "J2DfeSix17sc9p0l32RhNHzm0y7buCfySEqap/iiv1rwgYnRgIyNAzfR7N7WdQ==\n" +
                "-----END RSA PRIVATE KEY-----\n";

            var encryptedMessage = bouncyCryptoHelper.EncryptMessage(input, mobilePublicKey);
            var output = bouncyCryptoHelper.DecryptMessage(encryptedMessage, mobilePrivateKey);

            Assert.Equal(input, output);
        }

        [Fact]
        public void MobileReactNativeRsaNativeVerificationTest()
        {
            var bouncyCryptoHelper = new BouncyCryptoHelper(4096, 512);
            var mobilePublicKey = "-----BEGIN RSA PUBLIC KEY-----\n" +
                                  "MIICCgKCAgEAvFv4OwYYDMWUHR0Lijy+9f26i8ULVG5vYkk7NRL2N0aNec8qO9YL\n" +
                                  "LtF7+WS2lXuY8jHDdL/k7VqdQPtBWK50JsOjbQX3jE7HihW1vu6QfoF1GCFBDN9U\n" +
                                  "rwiBnN3ii+LpUSY1K1xuSmBrGPohK3XzUU58HA7gPFwuC5o3pSYS8NxIW6ThDQwo\n" +
                                  "zeKygjVBIZ3CvIG2rxFrhOLrjx1xcKRSdrly8JHtMgbQ7kYMTtK6Eqqa3EIGhuyM\n" +
                                  "oFDfUpImoq5bsa+bOSs1NSfFKBJUXpeH790K+DI1TQm4Fr92fuq6P5nl0F/BTAgD\n" +
                                  "2hVewfH7N/DctVB6sGgAgwf0rTZN4aiPe2B8q0Uaes4wLXBaY5YxRJ5Ob6jb1+ba\n" +
                                  "taYBG0nxwkMxZz/c3xF5Tcfo4iGQBcXhI1i+VwC+G4NOe0L2vtxlmcnAZ4gQVxhn\n" +
                                  "W+j6c36cZ8RIRL5vs3l/UPrdMoIsOLKg+AlI3CM4hjJZDhW+oi+YFeptPoAuz3Wf\n" +
                                  "I6MyCVtWO2Wq626CUL/NNw38HAsNKEmzqgcgL47iATbapN1BEPNqo0A/I9oPGfVb\n" +
                                  "ax675QUDMieuEv9D2YU1BELovnE/4pli2svYyJDUKkPtIYbOrxKdxvp2tymQkujr\n" +
                                  "RYoHDJSMtLD13BBmI4iEKUeFGsf4reuSKwU7AlgXllZ879X5ZK5Yw5kCAwEAAQ==\n" +
                                  "-----END RSA PUBLIC KEY-----\n";



            var mobileMessage = "The message was encrypted by RSA only!!! With spec symbol ™";
            var mobileSignature = "aDxQ11bzIOc4WsUnwxVUpdh3OoLex2h6ICVBL01Ine75URIoqXRqosD5PE0Z49IZkw4+76fLIO+3" +
                                  "9UA+9VoVTZ6vZ7EIgh2hDwJ1TFbM4ecCQxEkH7V+IczWLB3q7oPSM8+R8UxfX8Xkkz+HwwsDGEiQ" +
                                  "pKcv6lDFhK7NbvWNPfpV5lP3nNi2r4P+/MTi98mPAcPu3411Fa5k9rQHLsFXRovzFUaKR/KThNn2" +
                                  "NRMpfHy4A1EjqTuX4o3rqZxSe3CIlzA3W2P0+FA/w1hpM6tu7VthxbYYCu4v0GrnrdgF8vhH7JC4" +
                                  "a/pxTLRBFdPRnicjEUiQDUYAzpqWgMeuSP52w7sw27DjC+eNj26kGLNY0W3N8h5AhFWe3BF95leE" +
                                  "ZrRHWLlt65V8SLBc1WA0IJKby9HBJSOHyk7oNNrD+ue5nJ8rq3UIR3NWOraaDo4W3xFvJb+j4ghh" +
                                  "i/9ZD2bBb56119Doo4mQog5ouYuwVVaY54ya/WY6Uo6tog4t0gigFmXAYV5YkKi+tvproqScGeX5" +
                                  "yfekWF3UMMmI5gas1fPNIIUuCRHb8W0ESfnSsUVzBS3M01fwkg2sSsZfCJuf026gl9LlleudqKHh" +
                                  "aN7NPajLnJh57rZTLOLuK3CWtwn5/sN2Dw7mgxB98N5h0uyFiMUd3a6HR6iNvKBHGB/eXow4KbU=";

            Assert.True(bouncyCryptoHelper.VerifyMessage(mobileMessage, mobileSignature, mobilePublicKey));
        }

        [Fact]
        public void MobileReactNativeBiometricsVerificationTest()
        {
            var bouncyCryptoHelper = new BouncyCryptoHelper(4096, 256);
            var mobilePublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA0/WxH1V1ZBbFfLIjJY4jDvvI0cXWpaJT4B/EEd+A1cTTy4R2nn9LnznKXJTSrQABAWcMLyvqDI9aFHXsAx5y6mVwf8zXuHEd+lkIV5/YCDrtpcNEeQIFbd8OrCP/zPEXnKpZ9Zcjs2baOsHhwX4g+39qJDMnUMn/fZGz4zoM2JC81TsF3AcVFnqIKcnF81gGupo5eOfYevTAEf1CAADdEmwYypPA6sHp+ktTte4bcz62HEXaGBAWda2wVW6CpIXjMnzboXLHKIDTIPUd383NNEbep+n7RfEYOH+iMRSM6KaRiJW+WAuHTnSJ+eWYxZIu96D3VkSKUF1hl1MF4UDOawIDAQAB";
            var mobileMessage = "The message was encrypted by biometric data!!! With spec symbol ™";
            var mobileSignature = "P2TGa0ZhrZQozzGl2rN5zFWlS/6/46rIcphbYUUON1SRzUoYDK1ODmnEc5nT0kRw+4YbxMY1FFlqWuTVCwvuemacjoRZejGUb4DP5eQplzBVn/DB45/zZlmFEvqTl5O860AymEHUREjZtbBOfKQiQJFuhYRpbTvoSAlR2aQWUM++A4MGBL+BzFbI7PBYR9abwT7b8m1tVqqgGbrnk4RX5mWu1DczR1rNfIVnn9HshumQmE9ZQV+VIX8W4ZB8lHXqmu8XzTuZEwlaakEeJSbX6J3qHVqd28uxHTIccKWl9LfgmCbi/AOH01pDYEwAwXLKXnE5fejtBnOClLYKt+hdqw==";

            Assert.True(bouncyCryptoHelper.VerifyMessage(mobileMessage, mobileSignature, mobilePublicKey));
        }
    }
}
