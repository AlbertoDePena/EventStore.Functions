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
    public class AppendEventsHandler : HttpMiddleware
    {
        private readonly IStreamService _streamService;

        public AppendEventsHandler(IStreamService streamService)
        {
            _streamService = streamService ?? throw new ArgumentNullException(nameof(streamService));
        }

        public override async Task InvokeAsync(IHttpFunctionContext context)
        {
            context.Logger.LogInformation("Appending events...");

            var model = await context.Request.Content.ReadAsAsync<AppendEvents>();

            if (model == null)
            {
                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid payload");

                return;
            }

            await _streamService.AppendEventsAsync(model);

            context.Response = context.Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}
