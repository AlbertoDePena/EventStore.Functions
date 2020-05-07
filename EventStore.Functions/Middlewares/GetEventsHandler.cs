using Numaka.Functions.Infrastructure;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;
using EventStore.Core.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace EventStore.Functions.Middlewares
{
    public class GetEventsHandler : HttpMiddleware
    {
        private readonly IStreamService _streamService;

        public GetEventsHandler(IStreamService streamService)
        {
            _streamService = streamService ?? throw new ArgumentNullException(nameof(streamService));
        }

        public override async Task InvokeAsync(IHttpFunctionContext context)
        {
            context.Logger.LogInformation("Getting events...");

            context.Request.Query.TryGetValue("streamName", out StringValues streamName);

            context.Request.Query.TryGetValue("startAtVersion", out StringValues startAtVersionAsString);

            int.TryParse(startAtVersionAsString, out int startAtVersion);

            var models = await _streamService.GetEventsAsync(streamName, startAtVersion);

            context.ActionResult = new OkObjectResult(models);
        }
    }
}