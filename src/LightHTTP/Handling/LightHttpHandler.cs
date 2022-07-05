using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LightHTTP.Handling
{
    internal class LightHttpHandler : ILightHttpHandler
    {
        private readonly Func<string, bool> _acceptPath;
        private readonly LightHttpAsyncHandleDelegate _handle;

        public LightHttpHandler(Func<string, bool> acceptPath, LightHttpAsyncHandleDelegate handle)
        {
            _acceptPath = acceptPath;
            _handle = handle;
        }

        public bool AcceptsPath(string path)
        {
            return _acceptPath(path);
        }

        public Task HandleAsync(HttpListenerContext context, CancellationToken cancellationToken)
        {
            return _handle(context, cancellationToken);
        }
    }
}
