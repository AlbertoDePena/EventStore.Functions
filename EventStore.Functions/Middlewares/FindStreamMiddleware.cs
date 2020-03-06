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
    public class FindStreamMiddleware : HttpMiddleware
    {
        private readonly IStreamService _streamService;

        public FindStreamMiddleware(IStreamService streamService)
        {
            _streamService = streamService ?? throw new ArgumentNullException(nameof(streamService));
        }

        public override async Task InvokeAsync(IHttpFunctionContext context)
        {
            context.Logger.LogInformation("Handling stream request...");

            var values = context.Request.RequestUri.ParseQueryString();

            var streamName = values.Get("streamName");

            var model = await _streamService.GetStreamAsync(new QueryParameters { StreamName = streamName });

            context.Response = model == null ?
                context.Request.CreateErrorResponse(HttpStatusCode.NotFound, $"Stream with name '{streamName}' not found") :
                context.Request.CreateResponse(HttpStatusCode.OK, model);
        }
    }
}
