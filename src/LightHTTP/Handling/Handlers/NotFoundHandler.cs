using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LightHTTP.Handling.Handlers
{
    /// <summary>
    /// Responds any request with 404 Not Found.
    /// </summary>
    public class NotFoundHandler : ILightHttpHandler
    {
        /// <summary>
        /// Gets the singleton instance of <see cref="NotFoundHandler"/>.
        /// </summary>
        public static readonly NotFoundHandler Instance;

        static NotFoundHandler()
        {
            Instance = new();
        }

        private NotFoundHandler() { }

        /// <inheritdoc />
        public bool AcceptsPath(string path)
        {
            return true;
        }

        /// <inheritdoc />
        public Task HandleAsync(HttpListenerContext context, CancellationToken cancellationToken)
        {
            context.Response.StatusCode = 404;
            return Task.CompletedTask;
        }
    }
}
