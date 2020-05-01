using Microsoft.OpenApi;
using System;
using System.Reflection;

namespace EventStore.Functions
{
    public class RenderOpenApiDocumentOptions
    {
        public RenderOpenApiDocumentOptions(string version, string format, Assembly assembly, string routePrefix = "api")
        {
            Version = GetVersion(version);
            Format = GetFormat(format);
            Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
            RoutePrefix = routePrefix ?? throw new ArgumentNullException(nameof(routePrefix));
        }

        public OpenApiSpecVersion Version { get; }

        public OpenApiFormat Format { get; }

        public Assembly Assembly { get; }

        public string RoutePrefix { get; }

        private OpenApiSpecVersion GetVersion(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                throw new ArgumentNullException(nameof(version));
            }

            if (version.Equals("v2", StringComparison.CurrentCultureIgnoreCase))
            {
                return OpenApiSpecVersion.OpenApi2_0;
            }

            if (version.Equals("v3", StringComparison.CurrentCultureIgnoreCase))
            {
                return OpenApiSpecVersion.OpenApi3_0;
            }

            throw new InvalidOperationException("Invalid Open API version");
        }

        private OpenApiFormat GetFormat(string format)
        {
            if (string.IsNullOrWhiteSpace(format))
            {
                throw new ArgumentNullException(nameof(format));
            }

            return Enum.TryParse(format, true, out OpenApiFormat result)
                       ? result
                       : throw new InvalidOperationException("Invalid Open API format");
        }
    }
}
