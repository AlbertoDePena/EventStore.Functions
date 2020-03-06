using Numaka.Functions.Infrastructure;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using EventStore.Core.Contracts;

namespace EventStore.Functions.Middlewares
{
    public class StreamsMiddleware : HttpMiddleware
    {
        private readonly IStreamService _streamService;

        public StreamsMiddleware(IStreamService streamService)
        {
            _streamService = streamService ?? throw new ArgumentNullException(nameof(streamService));
        }

        public override async Task InvokeAsync(IHttpFunctionContext context)
        {
            context.Logger.LogInformation("Handling streams request...");

            var dtos = await _streamService.GetAllStreamsAsync();
            
            context.Response = context.Request.CreateResponse(HttpStatusCode.OK, dtos);
        }
    }
}