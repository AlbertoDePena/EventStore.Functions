using Numaka.Functions.Infrastructure;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;
using EventStore.Core.Contracts;

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

            var values = context.Request.RequestUri.ParseQueryString();

            var streamName = values.Get("streamName");

            int.TryParse(values.Get("startAtVersion"), out int startAtVersion);

            var models = await _streamService.GetEventsAsync(streamName, startAtVersion);

            context.Response = context.Request.CreateResponse(HttpStatusCode.OK, models);
        }
    }
}