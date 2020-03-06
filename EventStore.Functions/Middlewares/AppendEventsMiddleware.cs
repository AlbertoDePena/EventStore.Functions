using EventStore.Core.Contracts;
using Numaka.Functions.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Linq;
using EventStore.Core.Commands;
using System.Collections.Generic;

namespace EventStore.Functions.Middlewares
{
    public class AppendEventsMiddleware : HttpMiddleware
    {
        private readonly IStreamService _streamService;

        public AppendEventsMiddleware(IStreamService streamService)
        {
            _streamService = streamService ?? throw new ArgumentNullException(nameof(streamService));
        }

        public override async Task InvokeAsync(IHttpFunctionContext context)
        {
            context.Logger.LogInformation("Handling stream request...");

            var dto = await context.Request.Content.ReadAsAsync<AppendEvents>();

            if (dto == null)
            {
                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid payload");

                return;
            }

            var events = dto.Events?.Select(e => new NewEventCommand(e.Type, e.Data));

            var command = new AppendEventsCommand(dto.StreamName, dto.ExpectedVersion, events);

            await _streamService.AppendEventsAsync(command);

            context.Response = context.Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }

    public class AppendEvents
    {
        public int ExpectedVersion { get; set; }

        public IEnumerable<NewEvent> Events { get; set; } = Enumerable.Empty<NewEvent>();

        public string StreamName { get; set; }
    }

    public class NewEvent
    { 
        public string Type { get; set; }

        public string Data { get; set; }
    }
}
