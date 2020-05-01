using Microsoft.OpenApi.Models;
using System;

namespace EventStore.Functions
{
    public class OpenApiAppSettings
    {
        public OpenApiAppSettings(OpenApiInfo openApiInfo, string swaggerAuthKey = "")
        {
            OpenApiInfo = openApiInfo ?? throw new ArgumentNullException(nameof(openApiInfo));
            SwaggerAuthKey = swaggerAuthKey ?? throw new ArgumentNullException(nameof(swaggerAuthKey)); ;
        }

        public OpenApiInfo OpenApiInfo { get; }

        public string SwaggerAuthKey { get; }
    }
}
