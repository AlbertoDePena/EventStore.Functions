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

            builder.Services.AddSingleton<IValidator<AddSnapshot>, AddSnapshotValidator>();
            builder.Services.AddSingleton<IValidator<AppendEvents>, AppendEventsValidator>();
            builder.Services.AddSingleton<IValidator<NewEvent>, NewEventValidator>();
            builder.Services.AddSingleton<IValidator<QueryParameters>, QueryParametersValidator>();

            builder.Services.AddSingleton<IGuidFactory, GuidFactory>();
            builder.Services.AddSingleton<IDbConnectionFactory>(new SqlDbConnectionFactory(dbConnectionString));            
            builder.Services.AddSingleton<CorsMiddleware>();
            builder.Services.AddSingleton<ExceptionMiddleware>();   
            builder.Services.AddSingleton<SecurityMiddleware>();           
            builder.Services.AddSingleton<ITokenValidator>(new TokenValidator(openIdConnectMetadataAddress, clientId));

            builder.Services.AddSingleton<FindStreamMiddleware>();
            builder.Services.AddSingleton<GetAllStreamsMiddleware>();
            builder.Services.AddSingleton<GetEventsMiddleware>();
            builder.Services.AddSingleton<GetSnapshotsMiddleware>();
            builder.Services.AddSingleton<AppendEventsMiddleware>();
            builder.Services.AddSingleton<AddSnapshotMiddleware>();
            builder.Services.AddSingleton<DeleteSnapshotsMiddleware>();

            builder.Services.AddSingleton<IHttpFunctionContextBootstrapper, HttpFunctionContextBootstrapper>();

            builder.Services.AddTransient<Core.Contracts.IValidatorFactory, ValidatorFactory>();
            builder.Services.AddTransient<IStreamService, StreamService>();
            builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
            builder.Services.AddTransient<IMiddlewarePipeline, MiddlewarePipeline>();
        }
    }
}