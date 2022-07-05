using System;
using System.Collections.Generic;
using System.Text;

namespace LightHTTP.Handling
{
    /// <summary>
    /// Describes an object that registers handlers.
    /// </summary>
    public interface ILightHttpHandlerRegistry
    {
        /// <summary>
        /// Registers the <paramref name="handler"/>.
        /// </summary>
        void Handles(ILightHttpHandler handler);
    }
}
