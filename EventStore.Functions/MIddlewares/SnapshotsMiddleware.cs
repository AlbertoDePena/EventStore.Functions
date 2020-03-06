using Numaka.Functions.Infrastructure;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;

namespace EventStore.Functions.Middlewares
{
    public class SnapshotsMiddleware : HttpMiddleware
    {
        public override async Task InvokeAsync(IHttpFunctionContext context)
        {
            context.Logger.LogInformation("Handling snapshots request...");
            
            context.Response = context.Request.CreateResponse(HttpStatusCode.OK, "Snapshots");
        }
    }
}