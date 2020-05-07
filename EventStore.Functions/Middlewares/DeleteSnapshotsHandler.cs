using EventStore.Core.Contracts;
using Numaka.Functions.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace EventStore.Functions.Middlewares
{
    public class DeleteSnapshotsHandler : HttpMiddleware
    {
        private readonly IStreamService _streamService;

        public DeleteSnapshotsHandler(IStreamService streamService)
        {
            _streamService = streamService ?? throw new ArgumentNullException(nameof(streamService));
        }

        public override async Task InvokeAsync(IHttpFunctionContext context)
        {
            context.Request.Query.TryGetValue("streamName", out StringValues streamName);

            await _streamService.DeleteSnapshotsAsync(streamName);

            context.ActionResult = new NoContentResult();

            context.Logger.LogInformation("Deleted snapshots for stream {streamName}", streamName);
        }
    }
}
