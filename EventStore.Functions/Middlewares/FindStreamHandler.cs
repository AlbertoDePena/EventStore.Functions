using EventStore.Core.Contracts;
using Numaka.Functions.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Numaka.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace EventStore.Functions.Middlewares
{
    public class FindStreamHandler : HttpMiddleware
    {
        private readonly IStreamService _streamService;

        public FindStreamHandler(IStreamService streamService)
        {
            _streamService = streamService ?? throw new ArgumentNullException(nameof(streamService));
        }

        public override async Task InvokeAsync(IHttpFunctionContext context)
        {
            context.Logger.LogInformation("Finding stream...");

            context.Request.Query.TryGetValue("streamName", out StringValues streamName);

            var model = await _streamService.GetStreamAsync(streamName);

            if (model == null) throw new EntityNotFoundException($"Stream with name '{streamName}' not found");
                
            context.ActionResult = new OkObjectResult(model);
        }
    }
}
