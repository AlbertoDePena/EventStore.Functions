using EventStore.Core.Contracts;
using Numaka.Functions.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using EventStore.Models;
using Numaka.Common.Exceptions;

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
            context.Logger.LogInformation("Finding stream...");

            var values = context.Request.RequestUri.ParseQueryString();

            var streamName = values.Get("streamName");

            var model = await _streamService.GetStreamAsync(new QueryParameters { StreamName = streamName });

            if (model == null) throw new EntityNotFoundException($"Stream with name '{streamName}' not found");
                
            context.Response = context.Request.CreateResponse(HttpStatusCode.OK, model);
        }
    }
}
