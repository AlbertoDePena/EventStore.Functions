using System;
using Numaka.Functions.Infrastructure;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using EventStore.Functions.Handlers;
using EventStore.Core.Contracts;
using EventStore.Core;
using Numaka.Common.Contracts;
using FluentValidation;
using EventStore.Core.Commands;
using EventStore.Core.Validators;
using EventStore.Core.Queries;
using Numaka.Common;
using EventStore.Repository;
using EventStore.Repository.Contracts;

[assembly: FunctionsStartup(typeof(EventStore.Functions.Startup))]

namespace EventStore.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var metadataAddress = Environment.GetEnvironmentVariable("METADATA_ADDRESS");
            var clientId = Environment.GetEnvironmentVariable("CLIENT_ID");
            var dbConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

            builder.Services.AddSingleton<IValidator<AddSnapshotCommand>, AddSnapshotCommandValidator>();
            builder.Services.AddSingleton<IValidator<AppendEventsCommand>, AppendEventsCommandValidator>();
            builder.Services.AddSingleton<IValidator<DeleteSnapshotsCommand>, DeleteSnapshotsCommandValidator>();
            builder.Services.AddSingleton<IValidator<NewEventCommand>, NewEventCommandValidator>();

            builder.Services.AddSingleton<IValidator<EventsQuery>, EventsQueryValidator>();
            builder.Services.AddSingleton<IValidator<SnapshotsQuery>, SnapshotsQueryValidator>();
            builder.Services.AddSingleton<IValidator<StreamQuery>, StreamQueryValidator>();

            builder.Services.AddSingleton<IGuidFactory, GuidFactory>();
            builder.Services.AddSingleton<IDbConnectionFactory>(new SqlDbConnectionFactory(dbConnectionString));            
            builder.Services.AddSingleton<CorsMiddleware>();
            builder.Services.AddSingleton<SecurityMiddleware>();           
            builder.Services.AddSingleton<ITokenValidator>(new TokenValidator(metadataAddress, clientId));
            builder.Services.AddSingleton<StreamsHandler>();
            builder.Services.AddSingleton<EventsHandler>();
            builder.Services.AddSingleton<SnapshotsHandler>();      
            builder.Services.AddSingleton<IHttpFunctionContextBootstrapper, HttpFunctionContextBootstrapper>();
            builder.Services.AddSingleton<IHandlerFactory, HandlerFactory>();

            builder.Services.AddTransient<Core.Contracts.IValidatorFactory, ValidatorFactory>();
            builder.Services.AddTransient<IStreamService, StreamService>();
            builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
            builder.Services.AddTransient<IMiddlewarePipeline>(provider =>
            {
                var pipeline = new MiddlewarePipeline();

                // Order of middleware matters!!!
                pipeline.Register(provider.GetService<CorsMiddleware>());
                pipeline.Register(provider.GetService<SecurityMiddleware>());

                return pipeline;
            });
        }
    }
}