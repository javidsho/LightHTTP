using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LightHTTP.Tests
{
    public sealed class HttpHeadersTests : IDisposable
    {
        private const string TestUrl = "http://localhost:9999/";
        private readonly LightHttpServer _server;
        private readonly HttpClient _client;

        public HttpHeadersTests()
        {
            _server = new LightHttpServer();
            _server.Listener.Prefixes.Add(TestUrl);
            _server.Start();
            _client = new HttpClient
            {
                BaseAddress = new Uri(TestUrl),
            };
        }

        public void Dispose()
        {
            _server.Dispose();
        }

        [Theory]
        [InlineData(200)]
        [InlineData(206)]
        public async Task ExpectCorrectStatusCode(int statusCode)
        {
            _server.HandlesPath("/code", context => context.Response.StatusCode = statusCode);
            using var response = await _client.GetAsync("/code");
            Assert.Equal((HttpStatusCode)statusCode, response.StatusCode);
        }
    }
}
