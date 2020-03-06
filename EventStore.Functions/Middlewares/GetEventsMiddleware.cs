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
    public class GetEventsMiddleware : HttpMiddleware
    {
        private readonly IStreamService _streamService;

        public GetEventsMiddleware(IStreamService streamService)
        {
            _streamService = streamService ?? throw new ArgumentNullException(nameof(streamService));
        }

        public override async Task InvokeAsync(IHttpFunctionContext context)
        {
            context.Logger.LogInformation("Getting events...");

            var values = context.Request.RequestUri.ParseQueryString();

            var streamName = values.Get("streamName");

            int.TryParse(values.Get("startAtVersion"), out int startAtVersion);

            var query = new QueryParameters { StreamName = streamName, StartAtVersion = startAtVersion };

            var models = await _streamService.GetEventsAsync(query);

            context.Response = context.Request.CreateResponse(HttpStatusCode.OK, models);
        }
    }
}