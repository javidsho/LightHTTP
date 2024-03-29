﻿using LightHTTP.Handling;
using LightHTTP.Internal;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace LightHTTP
{
    /// <summary>
    /// Serves HTTP requests.
    /// </summary>
    public class LightHttpServer : ILightHttpHandlerRegistry, IDisposable
    {
        private readonly bool _ownsListener;
        private readonly LightHttpHandlerManager _handlerManager = new();
        private CancellationTokenSource? _cancellationTokenSource;

        /// <summary>
        /// Creates a new HTTP server wrapping <paramref name="listener"/>.
        /// </summary>
        public LightHttpServer(HttpListener listener, bool ownsListener = true)
        {
            Listener = listener;
            _ownsListener = ownsListener;
        }

        /// <summary>
        /// Creates a new HTTP server.
        /// </summary>
        public LightHttpServer() : this(new HttpListener())
        {
        }

        /// <summary>
        /// Invoked when an unhandled exception occurs during handling an HTTP request.
        /// </summary>
        public event Action<HttpListenerContext, Exception>? UnhandledExceptionThrown;

        /// <summary>
        /// Invoked when a new HTTP request is accepted.
        /// </summary>
        public event Action<HttpListenerContext>? RequestAccepted;

        /// <summary>
        /// Invoked when an HTTP request is served.
        /// </summary>
        public event Action<HttpListenerContext>? RequestServed;

        /// <summary>
        /// Gets the <see cref="HttpListener"/> instance.
        /// </summary>
        public HttpListener Listener { get; }

        /// <summary>
        /// Adds an automatically generated prefix to the HTTP listener.
        /// This method is useful for testing scenarios.
        /// </summary>
        /// <returns>
        /// The prefix in the following format: 'http://host:port/'
        /// </returns>
        public string AddAvailableLocalPrefix()
        {
            var port = TcpHelpers.GetOpenPort();
            var prefix = $"http://localhost:{port}/";
            Listener.Prefixes.Add(prefix);
            return prefix;
        }

        /// <inheritdoc />
        public void Handles(ILightHttpHandler handler)
        {
            _handlerManager.Handles(handler);
        }

        /// <summary>
        /// Starts handling requests.
        /// </summary>
        public void Start()
        {
            if (_cancellationTokenSource != null)
                return;
            _cancellationTokenSource = new();
            Listener.Start();
            _ = Task.Run(() => AcceptConnectionsAsync(_cancellationTokenSource.Token));
        }

        /// <summary>
        /// Stops handling requests.
        /// </summary>
        public void Stop()
        {
            if (_cancellationTokenSource == null)
                return;
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
            Listener.Stop();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Stop();
            if (_ownsListener)
                Listener.Abort();
        }

        private async Task AcceptConnectionsAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var context = await Listener.GetContextAsync().ConfigureAwait(false);
                _ = Task.Run(() => HandleConnectionAsync(context, cancellationToken));
            }
        }

        private async Task HandleConnectionAsync(HttpListenerContext context, CancellationToken cancellationToken)
        {
            try
            {
                RequestAccepted?.Invoke(context);
                await _handlerManager.HandleAsync(context, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                context.Response.StatusCode = 500;
                if (UnhandledExceptionThrown == null)
                    throw;
                UnhandledExceptionThrown?.Invoke(context, exception);
            }
            finally
            {
                context.Response.Close();
            }
            RequestServed?.Invoke(context);
        }
    }
}
