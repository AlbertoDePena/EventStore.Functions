using EventStore.Core.Contracts;
using Numaka.Functions.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;

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
            var values = context.Request.RequestUri.ParseQueryString();

            var streamName = values.Get("streamName");

            await _streamService.DeleteSnapshotsAsync(streamName);

            context.Response = context.Request.CreateResponse(HttpStatusCode.NoContent);

            context.Logger.LogInformation("Deleted snapshots for stream {streamName}", streamName);
        }
    }
}
