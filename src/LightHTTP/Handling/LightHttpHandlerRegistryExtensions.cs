using LightHTTP.Handling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace LightHTTP
{
    /// <summary>
    /// Defines extension methods on <see cref="ILightHttpHandlerRegistry"/>.
    /// </summary>
    public static class LightHttpHandlerRegistryExtensions
    {
        /// <summary>
        /// Registers a handler with the specified delegates.
        /// </summary>
        /// <param name="registry">The handler registry</param>
        /// <param name="acceptsPath">The method that given a path, determines whether the request should be handled.</param>
        /// <param name="handle">The handler method</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Handles(this ILightHttpHandlerRegistry registry, Func<string, bool> acceptsPath,
            LightHttpAsyncHandleDelegate handle)
        {
            registry.Handles(new LightHttpHandler(acceptsPath, handle));
        }

        /// <summary>
        /// Registers a handler with the specified delegates.
        /// </summary>
        /// <param name="registry">The handler registry</param>
        /// <param name="acceptsPath">The method that given a path, determines whether the request should be handled.</param>
        /// <param name="handle">The handler method</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Handles(this ILightHttpHandlerRegistry registry, Func<string, bool> acceptsPath,
            LightHttpHandleDelegate handle)
        {
            Task WrapAsyncHandle(HttpListenerContext context, CancellationToken cancellationToken)
            {
                handle(context);
                return Task.CompletedTask;
            }

            registry.Handles(new LightHttpHandler(acceptsPath, WrapAsyncHandle));
        }

        /// <summary>
        /// Instantiates <typeparamref name="THandler"/> using the paramaterless constructor and registers it.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Handles<THandler>(this ILightHttpHandlerRegistry registry)
            where THandler : ILightHttpHandler
        {
            var handler = Activator.CreateInstance<THandler>();
            registry.Handles(handler);
        }

        /// <summary>
        /// Defines a handler for the exact path.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void HandlesPath(this ILightHttpHandlerRegistry registry, string path, LightHttpAsyncHandleDelegate handle,
            StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
        {
            registry.Handles(p => p.Equals(path, comparisonType), handle);
        }

        /// <summary>
        /// Defines a handler for the exact path.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void HandlesPath(this ILightHttpHandlerRegistry registry, string path, LightHttpHandleDelegate handle,
            StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
        {
            registry.Handles(p => p.Equals(path, comparisonType), handle);
        }

        /// <summary>
        /// Defines a handler for any path matched by <paramref name="pathRegex"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void HandlesRegex(this ILightHttpHandlerRegistry registry, Regex pathRegex, LightHttpAsyncHandleDelegate handle)
        {
            registry.Handles(path => pathRegex.IsMatch(path), handle);
        }

        /// <summary>
        /// Defines a handler for any path matched by <paramref name="pathRegex"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void HandlesRegex(this ILightHttpHandlerRegistry registry, Regex pathRegex, LightHttpHandleDelegate handle)
        {
            registry.Handles(path => pathRegex.IsMatch(path), handle);
        }

        /// <summary>
        /// Defines a handler for any path matched by <paramref name="pathRegexPattern"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void HandlesRegex(this ILightHttpHandlerRegistry registry, string pathRegexPattern, LightHttpAsyncHandleDelegate handle)
        {
            var regex = new Regex(pathRegexPattern, RegexOptions.Compiled);
            registry.HandlesRegex(regex, handle);
        }

        /// <summary>
        /// Defines a handler for any path matched by <paramref name="pathRegexPattern"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void HandlesRegex(this ILightHttpHandlerRegistry registry, string pathRegexPattern, LightHttpHandleDelegate handle)
        {
            var regex = new Regex(pathRegexPattern, RegexOptions.Compiled);
            registry.HandlesRegex(regex, handle);
        }

        /// <summary>
        /// Defines a handler for the exact path.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void HandlesStaticFile(this ILightHttpHandlerRegistry registry, string path, string fileName, int bufferSize = 0)
        {
            if (bufferSize <= 0)
                bufferSize = 81920;

            registry.HandlesPath(path, async (context, cancellationToken) =>
            {
                using var fileStream = new FileStream(fileName, FileMode.Open);
                await fileStream.CopyToAsync(context.Response.OutputStream, bufferSize, cancellationToken).ConfigureAwait(false);
            });
        }
    }
}
