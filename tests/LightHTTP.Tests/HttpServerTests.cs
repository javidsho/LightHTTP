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

        [Fact]
        public async Task ThrowInHandler_Expect500()
        {
            _server.HandlesPath("/test", context =>
            {
                throw new Exception("Some random exception");
            });
            using var response = await _client.GetAsync("/test");
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task ThrowInHandler_ShouldBeAbleToKeepHandling()
        {
            _server.HandlesPath("/test", context =>
            {
                throw new Exception("Some random exception");
            });
            using var firstResponse = await _client.GetAsync("/test");
            Assert.Equal(HttpStatusCode.InternalServerError, firstResponse.StatusCode);

            _server.HandlesPath("/test", context =>
            {
                throw new Exception("Some random exception");
            });
            using var secondResponse = await _client.GetAsync("/test2");
            Assert.Equal(HttpStatusCode.NotFound, secondResponse.StatusCode);
        }

        [Fact]
        public async Task ThrowInHandler_ExpectUnhandledExceptionThrownEventToBeInvoked()
        {
            _server.HandlesPath("/test", context =>
            {
                throw new Exception("Some random exception");
            });
            _server.UnhandledExceptionThrown += (context, exception) =>
            {
                context.Response.StatusCode = 503;
            };
            using var response = await _client.GetAsync("/test");
            Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        }
    }
}
