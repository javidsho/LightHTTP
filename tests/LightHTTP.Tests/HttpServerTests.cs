using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LightHTTP.Tests
{
    public sealed class HttpServerTests : IDisposable
    {
        private readonly string TestUrl;
        private readonly LightHttpServer _server;
        private readonly HttpClient _client;

        public HttpServerTests()
        {
            _server = new LightHttpServer();
            TestUrl = _server.AddAvailableLocalPrefix();
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

        [Fact]
        public async Task RegisterNoHandler_ExpectFallbackHandler()
        {
            using var response = await _client.GetAsync("/test");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
