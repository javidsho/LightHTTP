using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LightHTTP.Handling
{
    /// <summary>
    /// When implemented, handles routed HTTP requests.
    /// </summary>
    public interface ILightHttpHandler
    {
        /// <summary>
        /// Determines whether or not this handler accepts the specified <paramref name="path"/>.
        /// </summary>
        bool AcceptsPath(string path);

        /// <summary>
        /// Handles the request.
        /// </summary>
        Task HandleAsync(HttpListenerContext context, CancellationToken cancellationToken);
    }
}
