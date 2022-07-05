using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LightHTTP.Handling
{
    /// <summary>
    /// Defines the signature of <see cref="ILightHttpHandler.HandleAsync(HttpListenerContext, CancellationToken)"/>.
    /// </summary>
    public delegate Task LightHttpAsyncHandleDelegate(HttpListenerContext context, CancellationToken cancellationToken);

    /// <summary>
    /// Defines the synchronous version of <see cref="LightHttpAsyncHandleDelegate"/>.
    /// </summary>
    public delegate void LightHttpHandleDelegate(HttpListenerContext context);
}
