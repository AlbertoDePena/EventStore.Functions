using Numaka.Functions.Infrastructure;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using FluentValidation;
using System.Linq;
using Newtonsoft.Json;
using Numaka.Common.Exceptions;

namespace EventStore.Functions.Middlewares
{
    public class ExceptionMiddleware : HttpMiddleware
    {
        public override async Task InvokeAsync(IHttpFunctionContext context)
        {
            try
            {
                if (Next != null)
                {
                    await Next.InvokeAsync(context);
                }
            }
            catch (EntityNotFoundException entityNotFoundEx)
            {
                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.NotFound, entityNotFoundEx.Message);
            }
            catch (ValidationException validationEx)
            {
                var errors = validationEx.Errors.Select(e => new { e.ErrorMessage, e.PropertyName });

                var model = new { Description = "Entity validation errors. See 'errors' property for more details", Errors = errors };

                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(model));
            }
            catch (ConcurrencyException concurrencyEx)
            {
                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.Conflict, concurrencyEx.Message);
            }
            catch (RepositoryException repositoryEx)
            {
                context.Logger.LogError(repositoryEx, repositoryEx.Message);

                const string message = "Something went terribly wrong. Please contact the system administrator.";

                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, message);
            }
        }
    }
}