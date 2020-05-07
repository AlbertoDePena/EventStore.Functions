using Numaka.Functions.Infrastructure;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;
using EventStore.Core.Contracts;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Mvc;

namespace EventStore.Functions.Middlewares
{
    public class GetSnapshotsHandler : HttpMiddleware
    {
        private readonly IStreamService _streamService;

        public GetSnapshotsHandler(IStreamService streamService)
        {
            _streamService = streamService ?? throw new ArgumentNullException(nameof(streamService));
        }

        public override async Task InvokeAsync(IHttpFunctionContext context)
        {
            context.Logger.LogInformation("Getting snapshots...");

            context.Request.Query.TryGetValue("streamName", out StringValues streamName);

            var models = await _streamService.GetSnapshotsAsync(streamName);

            context.ActionResult = new OkObjectResult(models);
        }
    }
}