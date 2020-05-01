using System;

namespace EventStore.Functions
{
    public class RenderSwaggerUIOptions
    {
        public RenderSwaggerUIOptions(string endpoint = "swagger.json", string routePrefix = "api")
        {
            Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
            RoutePrefix = routePrefix ?? throw new ArgumentNullException(nameof(routePrefix));
        }

        public string Endpoint { get; }

        public string RoutePrefix { get; }
    }
}
