using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net;

namespace Workshop1APITesting
{
    [TestFixture]
    public class Products : IDisposable
    {

        private RestClient client;
        private string token;

        [SetUp]
        public void Setup()
        {
            client = new RestClient(GlobalConstants.BaseUrl);
            token = GlobalConstants.AuthenticateUser("admin@gmail.com", "admin@gmail.com");

            Assert.That(token, Is.Not.Null.Or.Empty, "token is null or empty");
        }

        public void Dispose()
        {
            client?.Dispose();
        }

        [Test]
        public void Test_GetAllProducts()
        {
            var request = new RestRequest("product", Method.Get);
            var response = client.Execute(request);

            Assert.Multiple(() =>{
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response.Content, Is.Not.Empty);

                var content = JArray.Parse(response.Content);

                var productTitle = new[]
                {
                    "Smartphone Alpha", "Wireless Headphones", "Gaming Laptop", "4K Ultra HD TV",
                    "Smartwatch Pro"
                };

                foreach (var title in productTitle)
                {
                    Assert.That(content.ToString(), Does.Contain(title));
                };

                var expectedPrices = new Dictionary<string, decimal> {
                    { "Smartphone Alpha", 999},
                    { "Wireless Headphones",199},
                    { "Gaming Laptop",1499},
                    { "4K Ultra HD TV",899},
                    { "Smartwatch Pro",299}
                };

                foreach (var product in content)
                {
                    var title = product["title"].ToString();
                    if (expectedPrices.ContainsKey(title))
                    {
                        Assert.That(product["price"].Value<decimal>(), Is.EqualTo(expectedPrices[title]));
                    }
                }
            });
        }
    }
}
