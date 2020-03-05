using Numaka.Functions.Infrastructure;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;

namespace EventStore.Functions.Handlers
{
    public class EventsHandler : HttpMiddleware
    {
        public override async Task InvokeAsync(IHttpFunctionContext context)
        {
            context.Logger.LogInformation("Handling events request...");

            context.Response = context.Request.CreateResponse(HttpStatusCode.OK, "Events");
        }
    }
}