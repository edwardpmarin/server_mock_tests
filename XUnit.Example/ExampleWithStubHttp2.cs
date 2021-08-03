using Newtonsoft.Json;
using RestSharp;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;
using Xunit;

namespace XUnit.Example
{
    public class ExampleWithStubHttp2 : IDisposable
    {
        private readonly FluentMockServer stub;
        private readonly string baseUrl;
        private readonly HttpClient _client;

        private static readonly HttpClient client = new HttpClient();
        public ExampleWithStubHttp2()
        {
            
            var port = new Random().Next(5000, 6000);
            baseUrl = "http://localhost:" + port;
            var uri = new Uri("http://localhost:" + port);
            client.BaseAddress = uri;

            stub = FluentMockServer.Start(new FluentMockServerSettings
            {
                Urls = new[] { "http://+:" + port }
            });
        }

        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                stub.Stop();
                stub.Dispose();
            }
        }

        [Fact]
        public async Task TestAsync()
        {
            
        var bodyContent = new[] {
                                new {id = 1, description = "Book A" },
                                new {id = 2, description = "Book B" }
                            };

            stub.Given(
                Request
                .Create()
                    .WithPath("/media/test/Complete"))
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "aplication/json")
                        .WithBodyAsJson(bodyContent));

            var response = await client.PatchAsync($"media/test/Complete", null);
            var jsonResponse = response.Content.ReadAsStringAsync().Result;

            //var client = new RestClient(baseUrl);
            //var request = new RestRequest("/api/products");

            //var response = client.Execute(request);
            Assert.Equal(200, (int)response.StatusCode);
            Assert.Equal(JsonConvert.SerializeObject(bodyContent), jsonResponse);
        }


    }
}
