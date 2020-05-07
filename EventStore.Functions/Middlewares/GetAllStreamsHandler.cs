using Numaka.Functions.Infrastructure;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using EventStore.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace EventStore.Functions.Middlewares
{
    public class GetAllStreamsHandler : HttpMiddleware
    {
        private readonly IStreamService _streamService;

        public GetAllStreamsHandler(IStreamService streamService)
        {
            _streamService = streamService ?? throw new ArgumentNullException(nameof(streamService));
        }

        public override async Task InvokeAsync(IHttpFunctionContext context)
        {
            context.Logger.LogInformation("Getting all streams...");

            var models = await _streamService.GetAllStreamsAsync();
            
            context.ActionResult = new OkObjectResult(models);
        }
    }
}