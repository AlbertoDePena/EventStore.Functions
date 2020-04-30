using Numaka.Functions.Infrastructure;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;
using EventStore.Core.Contracts;

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

            var values = context.Request.RequestUri.ParseQueryString();

            var streamName = values.Get("streamName");

            var models = await _streamService.GetSnapshotsAsync(streamName);

            context.Response = context.Request.CreateResponse(HttpStatusCode.OK, models);
        }
    }
}