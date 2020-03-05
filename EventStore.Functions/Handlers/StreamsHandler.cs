using Numaka.Functions.Infrastructure;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using EventStore.Core.Contracts;

namespace EventStore.Functions.Handlers
{
    public class StreamsHandler : HttpMiddleware
    {
        private readonly IStreamService _streamService;

        public StreamsHandler(IStreamService streamService)
        {
            _streamService = streamService ?? throw new ArgumentNullException(nameof(streamService));
        }

        public override async Task InvokeAsync(IHttpFunctionContext context)
        {
            var dtos = await _streamService.GetAllStreamsAsync();
            
            context.Response = context.Request.CreateResponse(HttpStatusCode.OK, dtos);
        }
    }
}