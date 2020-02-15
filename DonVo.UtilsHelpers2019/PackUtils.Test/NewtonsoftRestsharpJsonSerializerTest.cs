using RestSharp;
using RestSharp.Authenticators;
using Xunit;

namespace PackUtils.Test
{
    public class NewtonsoftRestsharpJsonSerializerTest
    {
        public static NewtonsoftRestsharpJsonSerializer newtonsoftRestsharpJsonSerializer = new NewtonsoftRestsharpJsonSerializer(new Newtonsoft.Json.JsonSerializer());

        [Fact]
        public static void Serialize_Should_Return_Parsed_Object()
        {
            newtonsoftRestsharpJsonSerializer.DateFormat = "MM/DD/YY";
            newtonsoftRestsharpJsonSerializer.ContentType = "*+json";
            newtonsoftRestsharpJsonSerializer.RootElement = "MyRoot";
            newtonsoftRestsharpJsonSerializer.Namespace = "JsonRestsharp";

            var result1 = newtonsoftRestsharpJsonSerializer.Serialize(newtonsoftRestsharpJsonSerializer);
            Assert.Contains("application/json", result1);
            Assert.Contains("MyRoot", result1);
        }

        [Fact]
        public static void Deserialize_Should_Return_Parsed_Object()
        {

            //newtonsoftRestsharpJsonSerializer.DateFormat = "MM/DD/YY";
            //newtonsoftRestsharpJsonSerializer.ContentType = "*+json";
            //newtonsoftRestsharpJsonSerializer.RootElement = "MyRoot";
            //newtonsoftRestsharpJsonSerializer.Namespace = "JsonRestsharp";

            //var content = newtonsoftRestsharpJsonSerializer.Serialize(newtonsoftRestsharpJsonSerializer);

            var client = new RestClient("http://example.com");
            client.Authenticator = new HttpBasicAuthenticator("test", "123");

            var request = new RestRequest("resource/{id}");
            request.AddParameter("Namespace", "JsonRestsharp"); // adds to POST or URL querystring based on Method
            request.AddUrlSegment("RootElement", "MyRoot"); // replaces matching token in request.Resource

            // easily add HTTP Headers
            request.AddHeader("header", "value");

            // add files to upload (works with compatible verbs)
            request.AddFile("TEST", @"C:\DEVELOPMENT\TEST.txt");

            // execute the request
            var response = client.Post(request);
            response.Content = newtonsoftRestsharpJsonSerializer.Serialize(response.Content);
            //response.Content = content;

            var result = newtonsoftRestsharpJsonSerializer.Deserialize<object>(response).ToString();
            Assert.Contains("404 - Not Found", result);
        }


        [Fact]
        [System.Obsolete]
        public static void AddNewtonsoftResponseHandler_Should_Pass()
        {
            newtonsoftRestsharpJsonSerializer.DateFormat = "MM/DD/YY";
            newtonsoftRestsharpJsonSerializer.ContentType = "*+json";
            newtonsoftRestsharpJsonSerializer.RootElement = "MyRoot";
            newtonsoftRestsharpJsonSerializer.Namespace = "JsonRestsharp";

            var client = new RestClient("http://example.com");
            client.Authenticator = new HttpBasicAuthenticator("test", "123");

            try
            {
                NewtonsoftRestsharpJsonSerializerExtension.AddNewtonsoftResponseHandler(client, newtonsoftRestsharpJsonSerializer);
                Assert.True(true);
            }
            catch
            {
                Assert.True(false);
            }
        }
    }
}
