using LightHTTP.Handling.Handlers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LightHTTP.Handling
{
    internal class LightHttpHandlerManager : ILightHttpHandlerRegistry
    {
        private readonly List<ILightHttpHandler> _handlers = new();

        public LightHttpHandlerManager()
        {
            FallbackHandler = NotFoundHandler.Instance;
        }

        /// <summary>
        /// Gets or sets the handler that is used when there are no registered handlers that would accept the request.
        /// </summary>
        public ILightHttpHandler FallbackHandler { get; set; }

        public async Task HandleAsync(HttpListenerContext context, CancellationToken cancellationToken)
        {
            foreach (var handler in _handlers)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!handler.AcceptsPath(context.Request.Url.LocalPath))
                    continue;
                await handler.HandleAsync(context, cancellationToken).ConfigureAwait(false);
                return;
            }
            await FallbackHandler.HandleAsync(context, cancellationToken).ConfigureAwait(false);
        }

        public void Handles(ILightHttpHandler handler)
        {
            _handlers.Add(handler);
        }
    }
}
