using EventStore.Core.Contracts;
using Numaka.Functions.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using EventStore.Models;

namespace EventStore.Functions.Middlewares
{
    public class DeleteSnapshotsMiddleware : HttpMiddleware
    {
        private readonly IStreamService _streamService;

        public DeleteSnapshotsMiddleware(IStreamService streamService)
        {
            _streamService = streamService ?? throw new ArgumentNullException(nameof(streamService));
        }

        public override async Task InvokeAsync(IHttpFunctionContext context)
        {
            context.Logger.LogInformation("Deleting snapshots...");

            var values = context.Request.RequestUri.ParseQueryString();

            var streamName = values.Get("streamName");

            var query = new QueryParameters { StreamName = streamName };

            await _streamService.DeleteSnapshotsAsync(query);

            context.Response = context.Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}
