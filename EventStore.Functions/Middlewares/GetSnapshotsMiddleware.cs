using Numaka.Functions.Infrastructure;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;
using EventStore.Core.Contracts;
using EventStore.Models;

namespace EventStore.Functions.Middlewares
{
    public class GetSnapshotsMiddleware : HttpMiddleware
    {
        private readonly IStreamService _streamService;

        public GetSnapshotsMiddleware(IStreamService streamService)
        {
            _streamService = streamService ?? throw new ArgumentNullException(nameof(streamService));
        }

        public override async Task InvokeAsync(IHttpFunctionContext context)
        {
            context.Logger.LogInformation("Handling snapshots request...");

            var values = context.Request.RequestUri.ParseQueryString();

            var streamName = values.Get("streamName");

            var query = new QueryParameters { StreamName = streamName };

            var models = await _streamService.GetSnapshotsAsync(query);

            context.Response = context.Request.CreateResponse(HttpStatusCode.OK, models);
        }
    }
}