using System;
using Numaka.Functions.Infrastructure;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using EventStore.Functions.Middlewares;
using EventStore.Core.Contracts;
using EventStore.Core;
using Numaka.Common.Contracts;
using FluentValidation;
using EventStore.Core.Validators;
using Numaka.Common;
using EventStore.Repository;
using EventStore.Repository.Contracts;
using EventStore.Models;
using Microsoft.OpenApi.Models;

[assembly: FunctionsStartup(typeof(EventStore.Functions.Startup))]

namespace EventStore.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var openIdConnectMetadataAddress = Environment.GetEnvironmentVariable("OPEN_ID_CONNECT_METADATA_ADDRESS");
            var clientId = Environment.GetEnvironmentVariable("CLIENT_ID");
            var dbConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

            builder.Services.AddSingleton(new OpenApiAppSettings(openApiInfo: GetOpenApiInfo()));

            builder.Services.AddSingleton<IValidator<AddSnapshot>, AddSnapshotValidator>();
            builder.Services.AddSingleton<IValidator<AppendEvents>, AppendEventsValidator>();
            builder.Services.AddSingleton<IValidator<NewEvent>, NewEventValidator>();
            builder.Services.AddSingleton<IValidator<QueryParameters>, QueryParametersValidator>();
 
            builder.Services.AddSingleton<CorsMiddleware>();
            builder.Services.AddSingleton<ExceptionMiddleware>();   
            builder.Services.AddSingleton<SecurityMiddleware>();           
            builder.Services.AddSingleton<ITokenValidator>(new TokenValidator(openIdConnectMetadataAddress, clientId));

            builder.Services.AddSingleton<FindStreamHandler>();
            builder.Services.AddSingleton<GetAllStreamsHandler>();
            builder.Services.AddSingleton<GetEventsHandler>();
            builder.Services.AddSingleton<GetSnapshotsHandler>();
            builder.Services.AddSingleton<AppendEventsHandler>();
            builder.Services.AddSingleton<AddSnapshotHandler>();
            builder.Services.AddSingleton<DeleteSnapshotsHandler>();

            builder.Services.AddSingleton<IHttpFunctionContextBootstrapper, HttpFunctionContextBootstrapper>();

            builder.Services.AddTransient<ISqlServerGuidFactory, GuidFactory>();
            builder.Services.AddTransient<IDbConnectionFactory>(_ => new SqlDbConnectionFactory(dbConnectionString));
            builder.Services.AddTransient<Core.Contracts.IValidatorFactory, ValidatorFactory>();
            builder.Services.AddTransient<IStreamService, StreamService>();
            builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
            builder.Services.AddTransient<IMiddlewarePipeline, MiddlewarePipeline>();

            static OpenApiInfo GetOpenApiInfo() => new OpenApiInfo()
            {
                Title = "Event Store",
                Description = "Event Store web API",
                Version = "1.0"
            };
        }
    }
}