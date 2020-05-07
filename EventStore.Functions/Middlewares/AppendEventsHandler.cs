using EventStore.Core.Contracts;
using Numaka.Functions.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using EventStore.Models;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Newtonsoft.Json;

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

            using var reader = new StreamReader(context.Request.Body);

            var json = await reader.ReadToEndAsync();

            var model = JsonConvert.DeserializeObject<AppendEvents>(json);

            if (model == null)
            {
                context.ActionResult = new BadRequestObjectResult("Invalid payload");

                return;
            }

            await _streamService.AppendEventsAsync(model);

            context.ActionResult = new NoContentResult();
        }
    }
}
